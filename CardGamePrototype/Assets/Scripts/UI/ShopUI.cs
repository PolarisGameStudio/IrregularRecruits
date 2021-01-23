using GameLogic;
using MapLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class ShopUI : Singleton<ShopUI>
    {
        public CardUI CardPrefab;
        public GameObject Holder;
        public GameObject CardSelectionHolder;
        public TextMeshProUGUI PrizeText;
        public Button RerollButton;
        private UnityEvent OnReload = new UnityEvent();
        private Shop ShowingShop;

        private void Awake()
        {
            Shop.OnShopOpen.AddListener(SetupShop);
            Shop.OnShopReroll.AddListener(ShowNewCards);

            RerollButton.onClick.AddListener(RerollPush);
        }

        private void RerollPush()
        {
            Debug.Log("clicked reroll");
            ShowingShop?.Reroll();
        }

        public void SetupShop(Shop shop )
        {
            ShowingShop = shop;

            ShowNewCards(shop);
        }

        public void ShowNewCards(Shop shop)
        {
            Debug.Log("Showing new cards");
            
            OnReload.Invoke();
            OnReload.RemoveAllListeners();

            foreach(var card in shop.OnOffer)
            {
                var inst = Instantiate(CardPrefab, CardSelectionHolder.transform);

                inst.SetCreature(card.Item1);               

                if(inst.PriceText)
                    inst.PriceText.text = card.Item2.ToString();

                inst.OnClick.AddListener(() => BuyCard(card.Item1));

                OnReload.AddListener(() => Destroy(inst.gameObject));
            }

            PrizeText.text = shop.RerollPrice.ToString();
        }

        public void BuyCard(Creature card)
        {
            var result = ShowingShop.Buy(card);

            if(result)
            {
                //show result
            }

            Debug.Log($"buygin {card}, success: {result}");
        }

    }
}