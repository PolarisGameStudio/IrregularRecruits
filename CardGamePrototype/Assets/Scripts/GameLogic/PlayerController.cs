using System;

namespace GameLogic
{
    public class PlayerController : IDeckController
    {
        private Deck ControlledDeck;
        private Action TurnFinished;

        public int PlayerActionsLeft;

        public void SetupDeckActions(Deck deck, Action onfinish)
        {
            if (ControlledDeck != deck)
            {
                ControlledDeck = deck;

                TurnFinished = onfinish;

                Event.OnPlayerAction.AddListener(UsedPlayerAction);
            }

            deck.DrawInitialHand();
        }

        private void UsedPlayerAction(Deck deck)
        {
            if (deck != ControlledDeck)
                return;

            PlayerActionsLeft--;

            if (PlayerActionsLeft <= 0 || ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
            {
                TurnFinished.Invoke(); 
            }

        }

        public void YourTurn()
        {
            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            PlayerActionsLeft = GameSettings.Instance.PlaysPrTurn;

        }

        public bool ActionAvailable()
        {
            return PlayerActionsLeft > 0;
        }
    }
}