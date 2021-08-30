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

        //TODO: make an enum of fx type 
        public ParticleSystem[] WithdrawParticlesPrefab;
        public ParticleSystem[] ETBParticlesPrefab;
        public ParticleSystem[] DamageParticlesPrefab;
        public ParticleSystem[] WardParticlesPrefab;

        public ParticleSystem[] DeathParticlesPrefab;
        public ParticleSystem AbilitySelectParticles;
        public ParticleSystem LevelUpParticles;
        public ParticleSystem[] SummoningParticles;
        public AbilityAnimationFX[] AbilityFx;


        public  UnityEvent OnDraw = new UnityEvent();
        public  UnityEvent OnWithdraw = new UnityEvent();

        public class CardUIEvent : UnityEvent<CardUI, CreatureBark> { }
        public  CardUIEvent OnEtb = new CardUIEvent();
        public  UnityEvent OnDamaged = new UnityEvent();
        public  UnityEvent OnHeal = new UnityEvent();
        public  UnityEvent OnWardTrigger = new UnityEvent();
        public class AbilityEvent : UnityEvent<EffectType> { }
        public  AbilityEvent OnAbilityTrigger = new AbilityEvent();
        public  AbilityEvent OnAbilityTargetHit = new AbilityEvent();


        public CardUIEvent OnCreatureExclamation = new CardUIEvent();


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

            duration *= GameSettings.Speed();

            var startTime = Time.time;


            while (Time.time < startTime + duration )
            {
                yield return null;

                rect.position = Vector3.LerpUnclamped(startPos, endPos.position, Instance.AttackAnimationCurve.Evaluate((Time.time - startTime) / duration));
            }
        }

        public void WithdrawParticles(CardUI cardUI)
        {
            StartCoroutine(PlayCardFX(cardUI, WithdrawParticlesPrefab, 0, true));
            OnWithdraw.Invoke();
        }
        //Event.OnWithdraw.AddListener(c => StartCoroutine(PlayCardFX(c, WithdrawParticlesPrefab, 0, true)));
        //Event.OnPlay.AddListener(c => StartCoroutine(PlayCardFX(c, ETBParticlesPrefab, BattleUI.Instance.MoveDuration + 0.1f)));
        public void ETBParticles(CardUI cardUI)
        {
            StartCoroutine(PlayCardFX(cardUI, ETBParticlesPrefab, BattleUI.Instance.MoveDuration + 0.1f));
            OnEtb.Invoke(cardUI,CreatureBark.Grunt);

            //TODO: scale animation and sound with strength of the characters

        }
        
        public void MapNodeSpawn(MapUI.MapNodeIcon node)
        {
            PlayFx(ETBParticlesPrefab,node.transform.position,null);

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

        public IEnumerator WardPopParticles(CardUI c)
        {
            c.CardAnimation.SetWarded(false,false);

            OnWardTrigger.Invoke();

            yield return PlayCardFX(c, WardParticlesPrefab,0.3f,true);
        }

        public void SummonParticles(CardUI c)
        {
            StartCoroutine(c.CardAnimation.UnDissolve(false));

            StartCoroutine(PlayCardFX(c, SummoningParticles));


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
            yield return null;

            if (!card) yield break;

            //vector2 to ignore z position to prevent oddities
            Vector3 position =  card.transform.position + new Vector3(0,0,-0.5f);
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
                    Instance.ETBParticles(card);
                    break;
                case Deck.Zone.Graveyard:
                    Instance.DeathParticles(card);

                    if (card.GetCardState() == CardUI.CardState.FaceDown)
                        yield return card.Flip(CardUI.CardState.FaceUp, 0.3f);

                    yield return card.CardAnimation.Dissolve();
                    break;
                case Deck.Zone.Hand:
                    Instance.OnDraw.Invoke();
                    break;
            }
        }

        private IEnumerator PlayAbilityIconFx(AbilityHolderUI abilityOwner, ParticleSystem[] fxs, SpecialAbility ability, float delay = 0)
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
            float time = 1f * GameSettings.Speed();

            ui.transform.LeanScale(Vector3.one * 1.15f, time);

            Instance.OnCreatureExclamation.Invoke(ui, CreatureBark.Attack);

            yield return new WaitForSeconds(time / 2);
        }

        private static void PlayFx(ParticleSystem[] fxs, Vector3 position, Transform parent)
        {
            position += new Vector3(0, 0, -1);

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

        internal IEnumerator PlayDoublerFx(SpecialAbility ability, AbilityHolderUI owner, float delay = 0)
        {
            var abilityFx = AbilityFx.First(a => a.ActionType == EffectType.Doubler);

            OnAbilityTrigger.Invoke(EffectType.Doubler);

            yield return PlayCardFX(owner, abilityFx.OwnerFX, delay);
            yield return PlayAbilityIconFx(owner, abilityFx.AbilityIconFX, ability, delay);
        }

        public void Vibrate()
        {
            if(GameSettings.Instance.VibrateEnabled.Value)
                Handheld.Vibrate();
        }
    }
}