﻿using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;


namespace UI
{
    /// <summary>
    /// Responsible for handling all ui during combat
    /// </summary>
    public class BattleUI : Singleton<BattleUI>
    {

        public CardUI CardPrefab;

        private static Dictionary<Guid, AbilityHolderUI> CardUIs = new Dictionary<Guid, AbilityHolderUI>();

        public UIZone[] PlayerUIZones;
        public UIZone[] EnemyUIZones;

        public Button ViewPlayerDeckButton;

        [Serializable]
        public struct UIZone
        {
            public Deck.Zone Zone;
            public RectTransform RectTransform;
            public CardLayoutGroup CardLayout;
            public int CardRotation;
            public float CardPosAdjust;
        }

        public TextMeshProUGUI PlayerDeckDescription, EnemyDeckDescription;

        [Header("Movement")]
        public float MoveDuration = 0.5f;
        public AnimationCurve MoveAnimationCurve;

        private CardUI Attacker;
        private CardUI AttackTarget;
        public Deck PlayerDeck;
        public Deck EnemyDeck;
        private List<Card> InitialEnemyDeck;
        private List<Card> InitialPlayerDeck;

        public static UnityEvent OnBattleFinished = new UnityEvent();
        public static UnityEvent OnBattleBegin = new UnityEvent();

        //TODO: move these to animation system
        public static UnityEvent OnAbilitySelect = new UnityEvent();
        public static UnityEvent OnLevelUp = new UnityEvent();

        public Button EndTurnButton;


        private int XpAtStartOfBattle;

        void Awake()
        {
            ViewPlayerDeckButton.onClick.AddListener(() => DeckViewerUI.View(BattleManager.Instance.PlayerDeck));

            Event.OnCombatSetup.AddListener(SetupDecks);

            EndTurnButton.onClick.AddListener(EndPlayerTurn);

        }

        private void EndPlayerTurn()
        {
            if (PlayerDeck.DeckController is PlayerController)
                (PlayerDeck.DeckController as PlayerController).FinishTurn();
        }

        private void SetupDecks(Deck playerDeck, Deck opponentDeck)
        {
            MoveDuration = GameSettings.Instance.CombatSpeed / 2;

            SetupUI(playerDeck,true);
            SetupUI(opponentDeck,false);

            if(playerDeck.Hero!= null)
            {
                HeroUI.Instance.SetHero(playerDeck.Hero);
            }

            PlayerDeck = playerDeck;
            EnemyDeck = opponentDeck;

            InitialPlayerDeck = playerDeck.AllCreatures();
            InitialEnemyDeck = opponentDeck.AllCreatures();

            if (playerDeck.Hero != null) XpAtStartOfBattle = playerDeck.Hero.Xp;

            OnBattleBegin.Invoke();
        }

        private void SetupUI(Deck deck,bool playerDeck)
        {
            foreach (var card in deck.AllCreatures())
            {
                CreateCardUI(card,playerDeck);
            }

            if (deck.Hero != null)
            {
                CardUIs[deck.Hero.Guid] = HeroUI.Instance;
            }
            else
                HeroUI.Instance?.Disable();
        }

        private void CreateCardUI(Card card, bool playerDeck)
        {
            var ui = Instantiate(Instance.CardPrefab);

            ui.SetCard(card);

            CardUIs[card.Guid] = ui;

            StartCoroutine(ui.Flip(CardUI.CardState.FaceDown, 0f));

            //Should we just move it to library nomatter what?
            StartCoroutine(MoveCard(ui, card.Location, playerDeck));
        }

        public static IEnumerator CleanUpUI()
        {
            yield return null;
            Instance.EndBattle();
        }

        public static IEnumerator Summon(Card summon,bool playerdekc)
        {
            Instance.CreateCardUI(summon, playerdekc);
            yield return null;
        }
        public static IEnumerator UnSummon(Guid summon)
        {
            CardUI ui = GetCardUI(summon);

            if (!ui) yield break;

            ui.CardAnimation.Highlight();

            //TODO: should the effect be bfore or after
            yield return AnimationSystem.UnsummonFx(ui);


            ui.CardAnimation.TurnOffHighlight();

            Destroy(CardUIs[summon].gameObject);
        }



        private void EndBattle()
        {
            foreach (var kp in CardUIs)
            {
                if (kp.Value is HeroUI || !kp.Value)
                    continue;

                Destroy(kp.Value.gameObject);
            }
            CardUIs.Clear();

            OnBattleFinished.Invoke();

            var endXp = PlayerDeck.Hero != null ? PlayerDeck.Hero. Xp : 0;

            BattleSummary.ShowSummary(InitialPlayerDeck, InitialEnemyDeck, PlayerDeck.AllCreatures(), EnemyDeck.AllCreatures(),XpAtStartOfBattle,endXp,PlayerDeck.Hero);
            
        }

        private CardLayoutGroup GetZoneHolder(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardLayout;
        }

        private float GetZoneAdjust(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardPosAdjust;
        }

        //Handles death/ etb / withdraw / resurrection / draw animation
        internal static IEnumerator Move(Guid card, Deck.Zone to, Deck.Zone from, bool playerDeck)
        {
            yield return Instance.MoveCard(card, to, from, playerDeck);
        }

        private IEnumerator MoveCard(Guid card, Deck.Zone to, Deck.Zone from, bool playerDeck)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            ui.CardAnimation.Highlight();

            //TODO: should the effect be bfore or after
            yield return AnimationSystem.ZoneMoveEffects(ui, from, to);

            yield return MoveCard(ui, to, playerDeck);

            ui.CardAnimation.TurnOffHighlight();
        }

        internal static IEnumerator SetAttacker(Guid card)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            Instance.Attacker = ui;

            yield return AnimationSystem.StartAttack(ui);

            //do ready attack animation
        }

        internal static IEnumerator AbilityTriggered(Ability a, Guid guid, IEnumerable<Guid> ts)
        {
            var ui = GetAbilityHolderUI(guid);

            if (!ui) yield break;

            yield return AnimationSystem.Instance.PlayAbilityFx(a, ui, ts.Select(GetCardUI).ToList(), 0.25f);

        }

        private static CardUI GetCardUI(Guid cardGuid)
        {
            if (!CardUIs.ContainsKey(cardGuid))
            {
                Debug.LogError("ui for guid not instantiated");
                return null;
            }

            var ui = CardUIs[cardGuid];

            if (ui is CardUI)
                return ui as CardUI;
            else
                return null;
        }
        
        private static AbilityHolderUI GetAbilityHolderUI(Guid guid)
        {
            if (!CardUIs.ContainsKey(guid))
            {
                Debug.LogError("ui for guid not instantiated");
                return null;
            }

            var ui = CardUIs[guid];
            return ui;
        }

        internal static IEnumerator SetAttackTarget(Guid card)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            Instance.AttackTarget = ui;

            yield return Instance.AttackAnimation();
        }

        private IEnumerator AttackAnimation()
        {
            //null check
            if (AttackTarget == null) Debug.LogError("no attacktarget");
            if (Attacker == null) Debug.LogError("no attacker");

            AttackTarget.CardAnimation.Highlight();
            Attacker.CardAnimation.Highlight();

            yield return (AnimationSystem.AttackAnimation(Attacker, AttackTarget, 1f));

            AttackTarget.CardAnimation.TurnOffHighlight();
            Attacker.CardAnimation.TurnOffHighlight();
            AttackTarget = Attacker = null;
        }

        //negative for damage, positive for heal
        internal static IEnumerator CardHealthChange(Guid card, int val, int currentHealth, int maxHealth)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            if (val < 0)
            {
                ui.CardAnimation.DamageAnimation.Show(val);
                AnimationSystem.OnDamaged.Invoke();
            }
            else if (val > 0)
            {
                AnimationSystem.OnHeal.Invoke();

            }
            else
                Debug.LogError("health change of 0");

            ui.UpdateHealth(currentHealth, currentHealth < maxHealth);

            yield return null;
        }

        internal static IEnumerator CardStatsModified(Guid card, int val, int currentHealth, int currentAttack, bool damaged)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            ui.StatModifier(val);

            ui.UpdateHealth(currentHealth, damaged);
            ui.UpdateAttack(currentAttack);
            yield return null;
        }

        public static int GetZoneRotation(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardRotation;
        }

        public static void UpdateLibrary()
        {
            //Instance.PlayerDeckDescription.text = "Deck size: " + BattleManager.Instance.PlayerDeck.CreaturesInZone(Deck.Zone.Library).Count;
            //Instance.EnemyDeckDescription.text = "Deck size: " + BattleManager.Instance.EnemyDeck.CreaturesInZone(Deck.Zone.Library).Count;
        }

        //otherwise make an onclick event in CardUI

        private IEnumerator MoveCard(CardUI card, Deck.Zone zone, bool player)
        {
            if (!card) yield break;

            if(zone == Deck.Zone.Graveyard)
                yield return card.Flip(CardUI.CardState.Battle);

            var rect = card.GetComponent<RectTransform>();
            Vector2 startPos = rect.position;
            var zoneHolder = GetZoneHolder(zone, !player);

            if (!zoneHolder) yield break;

            var startTime = Time.time;
            float posAdjust = GetZoneAdjust(zone, !player);
            var rot = GetZoneRotation(zone, !player);
            Vector2 endPosition =  zoneHolder.GetLastPosition();


            endPosition += new Vector2(Random.Range(-posAdjust, posAdjust), Random.Range(-posAdjust, posAdjust));

            rect.Rotate(new Vector3(0, 0, Random.Range(-rot, rot)));

            var adjustDirection = (startPos - endPosition);

            adjustDirection = new Vector2(adjustDirection.y, -adjustDirection.x).normalized;

            while (Time.time < startTime + MoveDuration)
            {
                yield return null;

                float t = (Time.time - startTime) / MoveDuration;

                rect.position = Vector3.LerpUnclamped(startPos, endPosition, t);

                rect.position += (Vector3) (MoveAnimationCurve.Evaluate(t) * adjustDirection);

            }

            zoneHolder.AddChild(card);


            switch (zone)
            {
                case Deck.Zone.Library:
                    yield return card.Flip(CardUI.CardState.FaceDown);
                    break;
                case Deck.Zone.Battlefield:
                    yield return card.Flip(CardUI.CardState.Battle);
                    break;
                case Deck.Zone.Hand:
                    if(!player)
                        yield return card.Flip(CardUI.CardState.FaceDown);
                    else
                        yield return card.Flip(CardUI.CardState.FaceUp);
                    break;
                default:
                    yield return card.Flip(CardUI.CardState.FaceUp);
                    break;
            }

            card.Interactable = true;
        }


    }

}