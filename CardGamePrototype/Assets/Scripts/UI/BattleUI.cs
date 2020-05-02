using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;


namespace UI
{
    public class BattleUI : Singleton<BattleUI>
    {

        public CardUI CardPrefab;

        private Dictionary<Card, CardUI> CardUIs = new Dictionary<Card, CardUI>();

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

        public float MoveDuration = 0.2f;
        private Card Attacker;
        private Card AttackTarget;

        void Awake()
        {

            //should be handle by calls to move instead
            Event.OnDraw.AddListener(c => UpdateLibrary());
            Event.OnWithdraw.AddListener(c => UpdateLibrary());


            ViewPlayerDeckButton.onClick.AddListener(() => DeckViewerUI.View(BattleManager.Instance.PlayerDeck));
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
        internal static void Move(Card card, Deck.Zone to, Deck.Zone from)
        {
            CardUI ui;
            
            Debug.Log("Moving card: "+ card + " from: "+ from + "; to:" + to);
        }



        //negative for damage, positive for heal
        internal static void CardHealthChange(Card card, int val)
        {
            if (val < 0)
            {

                Debug.Log($"{card} damaged for {val}");
            }
            else if (val > 0)
            {

                Debug.Log($"{card} healed for {val}");
            }
            else
                Debug.LogError("health change of 0");
        }

        internal static void SetAttacker(Card card)
        {
            Instance.Attacker = card;

            //do ready attack animation
        }

        internal static void SetAttackTarget(Card card)
        {
            Instance.AttackTarget = card;

            Instance.AttackAnimation();
        }

        private void AttackAnimation()
        {
            //null check
            if (AttackTarget == null) Debug.LogError("no attacktarget");
            if (Attacker == null) Debug.LogError("no attacker");

            Debug.Log($"{Attacker} attacking {AttackTarget}");
        }

        internal static void CardStatsModified(Card card, int val)
        {
            if (val < 0)
            {

                Debug.Log($"{card} Stat changes by {val}");
            }
            else if (val > 0)
            {
                Debug.Log($"{card} stat changes by {val}");
            }
            else
                Debug.LogError("stat change change of 0");
        }

        public static int GetZoneRotation(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardRotation;
        }

        public static void UpdateLibrary()
        {
            Instance.PlayerDeckDescription.text = "Deck size: " + BattleManager.Instance.PlayerDeck.CreaturesInZone(Deck.Zone.Library).Count;
            Instance.EnemyDeckDescription.text = "Deck size: " + BattleManager.Instance.EnemyDeck.CreaturesInZone(Deck.Zone.Library).Count;
        }

        public static void Move(CardUI card, Deck.Zone zone, bool player, float delay = 0)
        {
            Instance.StartCoroutine(Instance.MoveCard(card, zone, player, delay));
        }

        private IEnumerator MoveCard(CardUI card, Deck.Zone zone, bool player, float delay)
        {
            if (!card) yield break;

            yield return new WaitForSeconds(delay);

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

            while (Time.time < startTime + MoveDuration)
            {
                yield return null;

                //TODO: use animation curve
                rect.position = Vector3.LerpUnclamped(startPos, endPosition, (Time.time - startTime) / MoveDuration);
            }

            //TODO: hack that should not be needed
            rect.localScale = Vector3.one;
            rect.SetParent(zoneRect);
            rect.SetAsLastSibling();

            if (card)
                card.Interactable = true;
        }


    }

}