using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapLogic
{
    public static class ShopRecommendation
    {

        public static Tuple<ShopChoice,Creature> GetRecommendation(Shop shop, Deck deck, int money)
        {

            var affordables = shop.OnOffer.Where(of => of.Item2 <= money).Select(i => i.Item1).OrderByDescending(o=> o.CR);

            if (!affordables.Any())
                return new Tuple<ShopChoice, Creature>(ShopChoice.NoAction, null);

            var topStrategies = GetTopStrategies(deck);

            //buy the best one, if no strategy yet
            if(!topStrategies.Any())
            {
                return new Tuple<ShopChoice, Creature>(ShopChoice.Buy, affordables.First());
            }

            //assign values to purchasable offers depending on strategies
            var offerValues = new Dictionary<Creature, int>();

            foreach(var creature in affordables)
            {
                var value = 0;

                foreach(var strat in topStrategies)
                {
                    if (creature.Enabling.Contains(strat.Key))
                        value += strat.Value;
                    if (creature.Payoff.Contains(strat.Key))
                        value += strat.Value * 2;
                }

                if(value > 0)
                    offerValues[creature] = value;
            }


            //if you can't afford any you want
            if(!offerValues.Any())
            {
                //reroll if you got enough money and a solid strategy
                var minmoneyToReroll = 40 + shop.RerollPrice;

                if (money >= minmoneyToReroll && topStrategies.Any(s =>s.Value > 10))
                    return new Tuple<ShopChoice, Creature>(ShopChoice.Reroll, null);

                //otherwise buy the best you can afford
                return new Tuple<ShopChoice, Creature>(ShopChoice.Buy, affordables.First());

            }

            //get the best suitable one
            var maxSuit = offerValues.Max(offer => offer.Value);
            return new Tuple<ShopChoice, Creature>(ShopChoice.Buy,offerValues.First(o => o.Value == maxSuit).Key);
        }


        //get strategies that are higher than the threshold and at minimum half the value of the top strategy
        public static Dictionary<DeckStrategy,int> GetTopStrategies(Deck deck, int threshold = 3)
        {
            var strategies = new Dictionary<DeckStrategy,int>();

            for (int i = 0; i < (int)DeckStrategy.COUNT; i++)
            {
                var strat = (DeckStrategy)i;

                int value = GetStrategyValue(deck, strat);

                if (value > threshold)
                    strategies.Add(strat,value);
            }

            if (strategies.Count < 2) return strategies;


            //removing all below half the Top strategies value
            float halfTopValue = strategies.Max(s => s.Value) / 2f;

            Func<KeyValuePair<DeckStrategy, int>, bool> belowHalfTop = stra => stra.Value < halfTopValue;

            while (strategies.Any(belowHalfTop))
                strategies.Remove(strategies.First(belowHalfTop).Key);


            return strategies;
        }

        private static int GetStrategyValue(Deck deck, DeckStrategy strat)
        {
            var creatures = deck.AllCreatures().Select(c=>c.Creature);

            //TODO: should this be count or CR sum? 
            var enablers = creatures.Count(c => c.Enabling.Contains(strat));

            var payOffs = creatures.Count(c => c.Payoff.Contains(strat));

            //payoffs matter more, but enablers also always matter
            return (enablers + 3) * (payOffs + 1);

        }
    }
}