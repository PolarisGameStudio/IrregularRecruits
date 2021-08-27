
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

namespace MapLogic
{
    public class Shop 
    {
        public Race VillageType;
        public Deck Visitor;
        public List<System.Tuple<Creature,int>> OnOffer = new List<System.Tuple<Creature, int>>();
        public int RerollsLeft;
        public int RerollPrice;
        public static Shop StandardShop;
        public int MaxCreatureCR;
        public int PlayerGoldOnReroll;

        //shop Events
        public class ShopEvent : UnityEvent<Shop> { }
        public static ShopEvent OnShopOpen = new ShopEvent();
        public static ShopEvent OnShopReroll = new ShopEvent();
        public static ShopEvent OnShopRemovingOptions = new ShopEvent();

        public class PurchaseEvent : UnityEvent<Creature,Shop> { }
        public static PurchaseEvent OnShopPurchase = new PurchaseEvent();

        public Shop(Race villageType, int cR = int.MaxValue)
        {
            VillageType = villageType;

            MaxCreatureCR =  14 + cR / 6;

            //Debug.Log("max cr in shop: " + MaxCreatureCR);

            RerollsLeft = ShopOptions.Instance.Rerolls;

            RerollPrice += ShopOptions.Instance.RerollInitialCost;

            SetupOptions();

            OnShopOpen.Invoke(this);
        }

        public static void ResetEvents()
        {
            OnShopOpen.RemoveAllListeners();
            OnShopReroll.RemoveAllListeners();
            OnShopRemovingOptions.RemoveAllListeners();
            OnShopPurchase.RemoveAllListeners();
        }

        public void Reroll()
        {
            if (RerollsLeft-- == 0 || RerollPrice > Map.PlayerGold)
                return;

            OnShopRemovingOptions.Invoke(this);

            Map.PlayerGold -= RerollPrice;

            RerollPrice += ShopOptions.Instance.RerollCostIncrease;

            SetupOptions();

            OnShopReroll.Invoke(this);
        }

        private void SetupOptions()
        {
            OnOffer.Clear();

            PlayerGoldOnReroll = Map.PlayerGold;

            foreach (var choice in ShopOptions.Instance.Options)
            {
                SetupChoice(choice.Options[Random.Range(0, choice.Options.Count)]);
            }
        }

        private void SetupChoice(ShopOptionType choice)
        {
            Creature forSale = null;
            int stop = 10;

            int price = 0 ;

            while ( (forSale == null || OnOffer.Any(a=> a.Item1 ==  forSale)) && stop-- > 0)
            {
                if (choice == ShopOptionType.OwnerRace && VillageType)
                    forSale = CreatureLibrary.Instance.GetShopCreature(VillageType,MaxCreatureCR);
                else if (choice == ShopOptionType.FriendRace && VillageType && VillageType.FriendRaces.Length > 0)
                    forSale = CreatureLibrary.Instance.GetShopCreature(VillageType.FriendRaces[Random.Range(0, VillageType.FriendRaces.Length)],MaxCreatureCR);
                else
                    forSale = CreatureLibrary.Instance.GetShopCreature( MaxCreatureCR);

                price = (int)( forSale.CR * Random.Range(0.5f, 1.2f));


                if (forSale.Rarity == Creature.RarityType.Unique) price *= 3;
                if (forSale.Rarity == Creature.RarityType.Rare) price *= 2;
            }

            if(!OnOffer.Any(a => a.Item1 == forSale))
                OnOffer.Add(new System.Tuple<Creature, int>(forSale, price));
        }

        //returns whether or not the player bought it
        public bool Buy(Creature card)
        {
            if (!OnOffer.Any(o => o.Item1 == card))
                return false;

            var sale = OnOffer.Single(o => o.Item1 == card);

            if(Map.PlayerGold >= sale.Item2)
            {
                OnOffer.Remove(sale);

                OnShopPurchase.Invoke(card, this);

                Battle.PlayerDeck.AddCard(new Card(sale.Item1));

                Map.PlayerGold -= sale.Item2;

                return true;
            }

            return false;

        }

        public void Close()
        {
            OnShopRemovingOptions.Invoke(this);
        }

    }
}