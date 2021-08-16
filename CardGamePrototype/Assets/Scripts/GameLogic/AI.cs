using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    public class AI : DeckController
    {
        public AI(Deck controlledDeck) : base(controlledDeck)
        {
        }

        //this could be a more complex evaluation and move mechanics
        public override void YourTurn()
        {
            Debug.Log("my turn");

            ResetActions();

            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            for (int i = 0; i < GameSettings.Instance.PlaysPrTurn; i++)
            {
                if (ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
                    break;

                ControlledDeck.CreaturesInZone(Deck.Zone.Hand)[0].PlayCard();
            }

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
                        scaredCat.Withdraw();
                    }
                }
            }

            ActionsLeft = 0;

            OnFinish.Invoke();
        }


        public override void SetupDeckActions(Deck deck, Action onFinish)
        {
            ControlledDeck = deck;
            OnFinish = onFinish;

            if(deck.Hero!= null)
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

            if(possibleAbilities.Count >0)
                hero.SelectLevelUpAbility(possibleAbilities[UnityEngine.Random.Range(0, possibleAbilities.Count)]);
        }
    }
}