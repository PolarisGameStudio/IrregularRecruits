using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class AI : IDeckController
    {
        private Deck ControlledDeck;
        private Action OnFinish;
        private bool MyTurn;

        //this could be a more complex evaluation and move mechanics
        public void YourTurn()
        {
            MyTurn = true;

            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            for (int i = 0; i < GameSettings.Instance.PlaysPrTurn; i++)
            {
                if (ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
                    break;

                ControlledDeck.CreaturesInZone(Deck.Zone.Hand)[0].PlayCard();
            }

            var opponent = BattleManager.Instance.GetEnemyDeck(ControlledDeck);

            var myBattlefield = ControlledDeck.CreaturesInZone(Deck.Zone.Battlefield);
            var opposingBattlefield = opponent.CreaturesInZone(Deck.Zone.Battlefield);

            if (opposingBattlefield.Count < myBattlefield.Count && myBattlefield.Count > 2 && opposingBattlefield.Any())
            {
                //TODO: should account for ranged.  Maybe by a potential damage method?
                var scaredCat = myBattlefield.FirstOrDefault(c => c.Damaged() && opposingBattlefield.Any(opp => opp.Attack * 2 > c.CurrentHealth));
                if (scaredCat != null)
                {
                    scaredCat.Withdraw();
                }
            }

            MyTurn = false;

            OnFinish.Invoke();
        }

        public void SetupDeckActions(Deck deck, Action onFinish)
        {
            ControlledDeck = deck;
            OnFinish = onFinish;

            deck.DrawInitialHand(true);
        }

        public bool ActionAvailable()
        {
            //TODO: should AI actions always be possible?
            return MyTurn;
        }

        public Hero GetHero()
        {
            return null;
        }

        public void UsedAction(Deck deck)
        {

        }
    }
}