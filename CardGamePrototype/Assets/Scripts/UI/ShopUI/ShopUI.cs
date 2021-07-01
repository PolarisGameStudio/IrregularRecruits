using GameLogic;
using MapLogic;
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
    public class ShopUI : Singleton<ShopUI>, IUIWindow
    {
        public CardUI CardPrefab;
        public GameObject Holder;
        public CanvasGroup FocusGroup;
        public List<GameObject> CardHolders;
        private List<Tuple<CardUI, int>> CurrentOffers ;
        public GameObject CardSelectionHolder;
        public TextMeshProUGUI PrizeText;
        public Button RerollButton;
        private UnityEvent OnReload = new UnityEvent();
        private Shop ShowingShop;
        public Image RaceWatermark;

        [Header("Buying Movement")]
        public float MoveDuration = 0.5f;
        public AnimationCurve MoveAnimationCurve;


        private void Awake()
        {
            Shop.OnShopOpen.AddListener(SetupShop);
            Shop.OnShopReroll.AddListener(ShowNewCards);
            MapController.Instance.OnPlayerGoldUpdate.AddListener(i => UpdatePurchasability());


            RerollButton.onClick.AddListener(RerollPush);

            while (CardHolders.Any() && CardHolders.Count < ShopOptions.Instance.Options.Count)
            {
                var instan = Instantiate(CardHolders.First(), CardHolders.First().transform.parent);

                CardHolders.Add(instan);
            }
            Holder.SetActive(false);
        }

        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder() => Holder;

        public int GetPriority() => 4;

        private void RerollPush()
        {
            Debug.Log("clicked reroll");
            ShowingShop?.Reroll();
        }

        public void SetupShop(Shop shop )
        {
            UIController.Instance.Open(this);

            RaceWatermark.sprite = shop.VillageType?.Icon;

            ShowingShop = shop;

            ShowNewCards(shop);
        }

        public void ShowNewCards(Shop shop)
        {
            
            OnReload.Invoke();
            OnReload.RemoveAllListeners();

            CurrentOffers = new List<Tuple<CardUI, int>>();

            if (shop.OnOffer.Count > CardHolders.Count)
                throw new ArgumentException("too many creatures sold in shop");

            var holder = 0;

            foreach(var card in shop.OnOffer)
            {
                var inst = Instantiate(CardPrefab, CardHolders[holder++].transform);

                inst.SetCreature(card.Item1);               

                if(inst.PriceText)
                    inst.PriceText.text = card.Item2.ToString();

                inst.BuyButton.onClick.AddListener(() => BuyCard(inst));

                OnReload.AddListener(() => Destroy(inst.gameObject));

                CurrentOffers.Add(new Tuple<CardUI, int>(inst,card.Item2));
            }

            PrizeText.text = shop.RerollPrice.ToString();

            UpdatePurchasability();
        }

        //makes the card uis interactable and showing the prize, dependant on whether it is purchasable
        private void UpdatePurchasability()
        {
            if (CurrentOffers == null) return;

            var gold = MapController.Instance.PlayerGold;

            foreach (var item in CurrentOffers)
            {
                var card = item.Item1;
                var able = item.Item2 <= gold;

                card.BuyButton.interactable = able;
                card.PriceText.color = able ? Color.white : Color.red;
            }


            var strategies = ShopRecommendation.GetTopStrategies(BattleManager.Instance.PlayerDeck);

            var recommendation = ShopRecommendation.GetRecommendation(ShowingShop, BattleManager.Instance.PlayerDeck, MapController.Instance.PlayerGold);

            var str = "Current LEading strategies: ";

            foreach (var strat in strategies)
                str += strat.Key +"(" + strat.Value + ")";

            Debug.Log(str); 
            Debug.Log("AI recommending: " +recommendation.Item1 + ": "+recommendation.Item2);
        }

        public void BuyCard(CardUI card)
        {
            var result = ShowingShop.Buy(card.Creature);

            if(result)
            {
                //show result
                
                StartCoroutine(BuyAnimation(card));
            }
        }

        private IEnumerator BuyAnimation(CardUI card)
        {

            if (!card) yield break;

            AnimationSystem.OnCreatureExclamation.Invoke(card, CreatureBark.Grunt);

            StartCoroutine(card.Flip(CardUI.CardState.FaceDown));
            card.Interactable = false;


            var rect = card.GetComponent<RectTransform>();
            Vector2 startPos = rect.position;

            var deckIcon = FindObjectOfType<DeckIcon>();

            if (!deckIcon)
                yield break;

            Vector2 endPosition = deckIcon.transform. position;

            //var posAdjust = 1f;

            //endPosition += new Vector2(Random.Range(-posAdjust, posAdjust), Random.Range(-posAdjust, posAdjust));

            var adjustDirection = (startPos - endPosition);

            adjustDirection = new Vector2(adjustDirection.y, -adjustDirection.x).normalized;

            var startRect = rect.rect;

            var endHeight = deckIcon.CardHeight;
            var endWidth = deckIcon.CardWidth;

            var startTime = Time.time;

            while (Time.time < startTime + MoveDuration)
            {
                yield return null;

                float t = (Time.time - startTime) / MoveDuration;

                rect.position = Vector3.LerpUnclamped(startPos, endPosition, t);

                //rect.position += (Vector3) (MoveAnimationCurve.Evaluate(t) * adjustDirection);

                rect.sizeDelta = new Vector2(Mathf.LerpUnclamped(startRect.width, endWidth, t),Mathf.LerpUnclamped(startRect.height, endHeight, t));
            }

            card.transform.SetParent(deckIcon.transform, true);

        }

        internal static void OpenStandardShop()
        {
            if (Shop.StandardShop == null) Shop.StandardShop = new Shop(null);

            Instance.SetupShop(Shop.StandardShop);
        }
    }
}