using GameLogic;
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
    public class BattleUI : Singleton<BattleUI>
    {

        public CardUI CardPrefab;

        private static Dictionary<Guid, CardUI> CardUIs = new Dictionary<Guid, CardUI>();

        public UIZone[] PlayerUIZones;
        public UIZone[] EnemyUIZones;

        public Button ViewPlayerDeckButton;

        [Serializable]
        public struct UIZone
        {
            public Deck.Zone Zone;
            public RectTransform RectTransform;
            public int CardRotation;
            public int CardPosAdjust;
        }

        public TextMeshProUGUI PlayerDeckDescription, EnemyDeckDescription;

        [Header("Movement")]
        public float MoveDuration = 0.5f;
        public AnimationCurve MoveAnimationCurve;

        private CardUI Attacker;
        private CardUI AttackTarget;
        private Deck PlayerDeck;
        private Deck EnemyDeck;
        private List<Card> InitialEnemyDeck;
        private List<Card> InitialPlayerDeck;
        internal static UnityEvent OnBattleFinished = new UnityEvent();
        internal static UnityEvent OnBattleBegin = new UnityEvent();

        void Awake()
        {
            //should be handle by calls to move instead
            AnimationSystem.OnDraw.AddListener(UpdateLibrary);
            AnimationSystem.OnWithdraw.AddListener(UpdateLibrary);

            MoveDuration = GameSettings.Instance.CombatSpeed;

            ViewPlayerDeckButton.onClick.AddListener(() => DeckViewerUI.View(BattleManager.Instance.PlayerDeck));

            Event.OnCombatSetup.AddListener(SetupDecks);
        }


        private void SetupDecks(Deck playerDeck, Deck opponentDeck)
        {
            SetupUI(playerDeck);
            SetupUI(opponentDeck);

            PlayerDeck = playerDeck;
            EnemyDeck = opponentDeck;

            InitialPlayerDeck = playerDeck.AllCreatures();
            InitialEnemyDeck = opponentDeck.AllCreatures();

            OnBattleBegin.Invoke();
        }

        private void SetupUI(Deck deck)
        {
            foreach (var card in deck.AllCreatures())
            {
                var ui = Instantiate<CardUI>(BattleUI.Instance.CardPrefab);

                ui.SetCard(card);

                CardUIs[card.Guid] = ui;
                
                //Should we just move it to library nomatter what?
                StartCoroutine(MoveCard(ui,card.Location,deck.PlayerDeck));
            }
        }

        public static IEnumerator CleanUpUI()
        {

            yield return null;
            Instance.EndBattle();
        }

        private  void EndBattle()
        {
            Debug.Log("Destroying all card uis");

            foreach (var kp in CardUIs)
                Destroy(kp.Value.gameObject);

            CardUIs.Clear();

            OnBattleFinished.Invoke();

            BattleSummary.ShowSummary(InitialPlayerDeck, InitialEnemyDeck, PlayerDeck.AllCreatures(),EnemyDeck.AllCreatures());
        }

        public static Transform GetZoneHolder(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).RectTransform;
        }

        public static int GetZoneAdjust(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardPosAdjust;
        }

        //Handles death/ etb / withdraw / resurrection / draw animation
        internal static IEnumerator Move(Guid card, Deck.Zone to, Deck.Zone from,bool playerDeck)
        {
            yield return Instance.MoveCard(card, to, from, playerDeck);
        }

        private IEnumerator MoveCard(Guid card, Deck.Zone to, Deck.Zone from, bool playerDeck)
        {
            CardUI ui = GetCardUI(card);

            //TODO: should the effect be bfore or after
            yield return AnimationSystem.ZoneMoveEffects(ui, from, to);
            
            yield return MoveCard(ui, to, playerDeck);
        }

        internal static IEnumerator SetAttacker(Guid card)
        {
            CardUI ui = GetCardUI(card);

            Instance.Attacker = ui;

            yield return AnimationSystem.StartAttack(ui);

            //do ready attack animation
        }
        
        internal static IEnumerator AbilityTriggered(Ability a, Guid card, IEnumerable<Guid> ts)
        {
            CardUI ui = GetCardUI(card);

            yield return AnimationSystem.Instance.PlayAbilityFx(a, ui, ts.Select(GetCardUI).ToList(), 0.25f);
        }

        private static CardUI GetCardUI(Guid cardGuid)
        {
            if (!CardUIs.ContainsKey(cardGuid))
                Debug.LogError("trying to move card without a ui instantiated");

            CardUI ui = CardUIs[cardGuid];
            return ui;
        }

        internal static IEnumerator SetAttackTarget(Guid card)
        {
            CardUI ui = GetCardUI(card);

            Instance.AttackTarget = ui;

            yield return Instance.AttackAnimation();
        }

        private IEnumerator AttackAnimation()
        {
            //null check
            if (AttackTarget == null) Debug.LogError("no attacktarget");
            if (Attacker == null) Debug.LogError("no attacker");

            Debug.Log($"{Attacker.Creature.name} attacking {AttackTarget.Creature.name}");

            yield return (AnimationSystem.AttackAnimation(Attacker, AttackTarget, 1f));

            AttackTarget = Attacker = null;
        }

        //negative for damage, positive for heal
        internal static IEnumerator CardHealthChange(Guid card, int val, int currentHealth, int maxHealth)
        {
            CardUI ui = GetCardUI(card);

            if (val < 0)
            {
                Debug.Log($"{ui} damaged for {val}");
                ui.CardAnimation.DamageAnimation.Show(val);
                AnimationSystem.OnDamaged.Invoke();
            }
            else if (val > 0)
            {
                Debug.Log($"{ui} healed for {val}");
            }
            else
                Debug.LogError("health change of 0");

            ui.UpdateHealth(currentHealth, currentHealth < maxHealth);

            yield return null;
        }

        internal static IEnumerator CardStatsModified(Guid card, int val,int currentHealth,int currentAttack, bool damaged)
        {
            CardUI ui = GetCardUI(card);

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

            var rect = card.GetComponent<RectTransform>();
            var startPos = rect.position;
            var zoneRect = GetZoneHolder(zone, !player);

            if (!zoneRect) yield break;

            var startTime = Time.time;
            var posAdjust = GetZoneAdjust(zone, !player);
            var rot = GetZoneRotation(zone, !player);
            Vector3 endPosition;
            if (zoneRect.childCount > 0)
            {
                endPosition = zoneRect.GetChild(zoneRect.childCount - 1).position;
            }
            else
                endPosition = zoneRect.position;

            endPosition += new Vector3(Random.Range(-posAdjust, posAdjust), Random.Range(-posAdjust, posAdjust));

            rect.Rotate(new Vector3(0, 0, Random.Range(-rot, rot)));

            //TODO: use lean tween instead
            //LeanTween.move(card.BattleRepresentation.gameObject, endPosition, duration).setEaseInExpo();//.setOnComplete(c => rect.SetParent(zoneRect));

            card.CardAnimation.ChangeLayoutSizeWhileMoving();

            var adjustDirection = (startPos - endPosition);

            adjustDirection = new Vector2(adjustDirection.y, -adjustDirection.x).normalized;

            while (Time.time < startTime + MoveDuration)
            {
                yield return null;

                float t = (Time.time - startTime) / MoveDuration;

                //TODO: use animation curve
                rect.position = Vector3.LerpUnclamped(startPos, endPosition, t);

                rect.position += MoveAnimationCurve.Evaluate(t) * adjustDirection;

            }

            rect.SetParent(zoneRect);
            //TODO: hack that should not be needed
            rect.localScale = Vector3.one;
            rect.SetAsLastSibling();

            
            yield return card.Flip(zone == Deck.Zone.Library || (!player && zone == Deck.Zone.Hand));

            card.Interactable = true;
        }


    }

}