using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Event = GameLogic.Event;

namespace UI
{
    public class AnimationSystem : Singleton<AnimationSystem>
    {
        public AnimationCurve AttackAnimationCurve;

        public ZoneMoveAnimation[] ZoneMoveAnimations;

        public ParticleSystem[] WithdrawParticlesPrefab;
        public ParticleSystem[] ETBParticlesPrefab;
        public ParticleSystem[] DamageParticlesPrefab;
        public ParticleSystem[] DeathParticlesPrefab;
        public ParticleSystem AbilitySelectParticles;
        public ParticleSystem LevelUpParticles;
        public AbilityAnimationFX[] AbilityFx;

        public class CardUIEvent : UnityEvent<CardUI,CreatureBark> { }

        public static CardUIEvent OnCreatureExclamation = new CardUIEvent();


        public static UnityEvent OnDraw = new UnityEvent();
        public static UnityEvent OnWithdraw = new UnityEvent();
        public static UnityEvent OnEtb = new UnityEvent();
        public static UnityEvent OnDamaged = new UnityEvent();
        public static UnityEvent OnHeal = new UnityEvent();
        public class AbilityEvent : UnityEvent<EffectType> { }
        public static AbilityEvent OnAbilityTrigger = new AbilityEvent();
        public static AbilityEvent OnAbilityTargetHit = new AbilityEvent();

        [Serializable]
        public struct ZoneMoveAnimation
        {
            public Deck.Zone TargetZone;
            public Deck.Zone? FromZone;
            public ParticleSystem StartParticles;
            public ParticleSystem ArriveParticles;
            public ParticleSystem MoveParticles;
        }

        [Serializable]
        public struct AbilityAnimationFX
        {
            public EffectType ActionType;
            public ParticleSystem[] AbilityIconFX;
            public ParticleSystem[] TargetFX;
            public ParticleSystem[] OwnerFX;
        }

        public static IEnumerator AttackAnimation(CardUI owner, CardUI target, float duration)
        {
            var rect = owner.GetComponent<RectTransform>();
            var startPos = rect.position;
            var endPos = target.GetComponent<RectTransform>();

            duration *= GameSettings.Instance.CombatSpeed;

            var startTime = Time.time;

            OnCreatureExclamation.Invoke(owner, CreatureBark.Attack);

            while (Time.time < startTime + duration)
            {
                yield return null;

                rect.position = Vector3.LerpUnclamped(startPos, endPos.position, AnimationSystem.Instance.AttackAnimationCurve.Evaluate((Time.time - startTime) / duration));
            }
        }

        public void WithdrawParticles(CardUI cardUI)
        {
            StartCoroutine(PlayCardFX(cardUI, WithdrawParticlesPrefab, 0, true));
            OnWithdraw.Invoke();
        }
        //Event.OnWithdraw.AddListener(c => StartCoroutine(PlayCardFX(c, WithdrawParticlesPrefab, 0, true)));
        //Event.OnPlay.AddListener(c => StartCoroutine(PlayCardFX(c, ETBParticlesPrefab, BattleUI.Instance.MoveDuration + 0.1f)));
        public void PlayParticles(CardUI cardUI)
        {
            StartCoroutine(PlayCardFX(cardUI, ETBParticlesPrefab, BattleUI.Instance.MoveDuration + 0.1f));
            OnEtb.Invoke();
        }
        //Event.OnDeath.AddListener(c => StartCoroutine(PlayCardFX(c, DeathParticlesPrefab, 0.1f)));
        public void DeathParticles(CardUI cardUI)
        {
            StartCoroutine(PlayCardFX(cardUI, DeathParticlesPrefab, 0.1f));
            OnCreatureExclamation.Invoke(cardUI,CreatureBark.Death);
        }
        //Event.OnDamaged.AddListener(c => StartCoroutine(PlayCardFX(c, DamageParticlesPrefab)));
        public void DamageParticles(CardUI c)
        {
            StartCoroutine(PlayCardFX(c, DamageParticlesPrefab));

        }

        internal static IEnumerator UnsummonFx(CardUI ui)
        {
            yield return ui.CardAnimation.Dissolve();
        }

        public static void PlayLevelupFX(Vector2 postion)
        {
            PlayFx(new[] { Instance.LevelUpParticles }, postion, null);
        }
        public static void PlayAbilitySelection(Vector2 postion)
        {
            PlayFx(new[] { Instance.AbilitySelectParticles }, postion, null);
        }

        private IEnumerator PlayCardFX(AbilityHolderUI card, ParticleSystem[] fxs, float delay = 0, bool instantiateInWorldSpace = false)
        {
            if (!card) yield break;
            //vector2 to ignore z position to prevent oddities
            Vector2 position = card.transform.position;
            PlayFx(fxs, position, instantiateInWorldSpace ? null : card.transform);

            yield return new WaitForSeconds(delay);
        }

        internal static IEnumerator ZoneMoveEffects(CardUI card, Deck.Zone from, Deck.Zone to)
        {
            switch (to)
            {
                case Deck.Zone.Library:
                    Instance.WithdrawParticles(card);
                    break;
                case Deck.Zone.Battlefield:
                    if (from == Deck.Zone.Graveyard)
                        yield return card.CardAnimation.UnDissolve();
                    Instance.PlayParticles(card);
                    break;
                case Deck.Zone.Graveyard:
                    Instance.DeathParticles(card);

                    if (card.GetCardState() == CardUI.CardState.FaceDown)
                        yield return card.Flip(CardUI.CardState.FaceUp, 0.3f);

                    yield return card.CardAnimation.Dissolve();
                    break;
                case Deck.Zone.Hand:
                    OnDraw.Invoke();
                    break;
            }
        }

        private IEnumerator PlayAbilityIconFx(AbilityHolderUI abilityOwner, ParticleSystem[] fxs, AbilityWithEffect ability, float delay = 0)
        {
            if (!abilityOwner) yield break;

            var image = abilityOwner.GetAbilityImage(ability);

            if (!image) yield break;

            //vector2 to ignore z position to prevent oddities

            //Vector2 position = abilityOwner.SpecialAbilityIcon.transform.position;
            PlayFx(fxs, image.transform.position, image.transform);

            yield return new WaitForSeconds(delay);
        }

        internal static IEnumerator StartAttack(CardUI ui)
        {
            yield return new WaitForSeconds(0.5f);
        }

        private static void PlayFx(ParticleSystem[] fxs, Vector2 position, Transform parent)
        {
            foreach (var fx in fxs)
            {
                if (parent)
                    Instantiate(fx, position, parent.rotation).transform.SetParent(parent);
                else
                    Instantiate(fx, position, fx.transform.localRotation);//.transform.SetParent(parent);

            }
        }

        public IEnumerator PlayAbilityFx(AbilityWithEffect ability, AbilityHolderUI owner, List<CardUI> targets, float delay = 0)
        {
            var abilityFx = AbilityFx.First(a => a.ActionType == ability.ResultingAction.ActionType);

            OnAbilityTrigger.Invoke(ability.ResultingAction.ActionType);

            yield return PlayCardFX(owner, abilityFx.OwnerFX, delay);
            yield return PlayAbilityIconFx(owner, abilityFx.AbilityIconFX, ability, delay);

            foreach (var t in targets)
            {
                OnAbilityTargetHit.Invoke(ability.ResultingAction.ActionType);
                yield return PlayCardFX(t, abilityFx.TargetFX, delay / targets.Count());
            }
        }
    }
}