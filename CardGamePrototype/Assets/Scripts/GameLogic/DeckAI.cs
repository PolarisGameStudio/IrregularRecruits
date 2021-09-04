using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    public class DeckAI : DeckController
    {
        public DeckAI(Deck controlledDeck, bool enm) : base(controlledDeck)
        {
            Enemy = enm;
        }

        //this could be a more complex evaluation and move mechanics
        public override void YourTurn()
        {
            ControlledDeck.Draw(
                Enemy ? GameSettings.Instance.EnemyDrawsPrTurn : GameSettings.Instance.DrawPrTurn
                );

            ResetActions();

            for (int i = 0; i < GameSettings.Instance.PlaysPrTurn; i++)
            {
                if (ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
                    break;

                ControlledDeck.CreaturesInZone(Deck.Zone.Hand)[0].PlayCard();
            }

            //WithdrawVulnerable();

            ArrangeAttackOrder();

            ActionsLeft = 0;

            OnFinish.Invoke();
        }

        private void ArrangeAttackOrder()
        {
            var orderPreference = new Dictionary<Card, float>();

            foreach (var recruit in ControlledDeck.CreaturesInZone(Deck.Zone.Battlefield))
            {
                //the higher, the more likely to be first. 
                var firstFactor = recruit.CurrentHealth / 20f + Random.value * 0.2f;

                if (recruit.Ranged()) firstFactor++;

                if (recruit.Lifedrain()) firstFactor++;

                if (recruit.Carnage()) firstFactor++;

                if (recruit.Ferocity()) firstFactor++;

                //can be negative
                if (recruit.Ability())
                {
                    firstFactor += recruit.Ability().AttackOrderModifier();
                }

                orderPreference[recruit] = firstFactor;

            }

            var pos = 0;

            foreach (var pair in orderPreference.OrderByDescending((c) => c.Value))
                ControlledDeck.SetPosition(pair.Key, Deck.Zone.Battlefield, pos++);
        }

        private void WithdrawVulnerable()
        {
            var opponent = Battle.GetEnemyDeck(ControlledDeck);

            if (opponent != null)
            {
                var myBattlefield = ControlledDeck.CreaturesInZone(Deck.Zone.Battlefield);


                var opposingBattlefield = opponent.CreaturesInZone(Deck.Zone.Battlefield);

                if (opposingBattlefield.Count < myBattlefield.Count && myBattlefield.Count > 2 && opposingBattlefield.Any())
                {
                    //TODO: should account for ranged.  Maybe by a potential damage method?
                    var scaredCat = myBattlefield.FirstOrDefault(c => c.Damaged() && !c.IsSummon() && opposingBattlefield.Any(opp => opp.Attack * 2 > c.CurrentHealth));
                    if (scaredCat != null)
                    {
                        scaredCat.Withdraw(true);
                    }
                }
            }
        }

        public override void SetupDeckActions(Deck deck, System.Action onFinish)
        {
            ControlledDeck = deck;
            OnFinish = onFinish;

            if (deck.Hero != null)
            {
                //TODO: test that it was the correct hero
                //AI does not level up. Could be an option in the settings
                //Event.OnLevelUp.AddListener(h=> SelectAbility());
            }

            deck.DrawInitialHand(true);
        }


        public override void UsedAction(Deck deck)
        {
            if (deck != ControlledDeck)
                return;

            ActionsLeft--;
        }

        private void SelectAbility()
        {
            var hero = ControlledDeck.Hero;

            var possibleAbilities = hero.GetLevelUpOptions();

            if (possibleAbilities.Count > 0)
                hero.SelectLevelUpAbility(possibleAbilities[UnityEngine.Random.Range(0, possibleAbilities.Count)]);
        }
    }
}