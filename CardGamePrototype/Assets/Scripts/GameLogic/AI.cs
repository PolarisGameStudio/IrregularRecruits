using System;

namespace GameLogic
{
    public class AI : IDeckController
    {
        private Deck ControlledDeck;
        private Action OnFinish;

        //this could be a more complex evaluation and move mechanics
        public void YourTurn()
        {
            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            for (int i = 0; i < GameSettings.Instance.EnemyPlaysPrTurn; i++)
            {
                if (ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
                    break;

                ControlledDeck.CreaturesInZone(Deck.Zone.Hand)[0].PlayCard();
            }

            OnFinish.Invoke();
        }

        public void SetupDeckActions(Deck deck, Action onFinish)
        {
            ControlledDeck = deck;
            OnFinish = onFinish;
        }

    }
}