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
            ControlledDeck = deck;

            TurnFinished = onfinish;
            
            Event.OnPlayerAction.AddListener(UsedPlayerAction);

            deck.DrawInitialHand();
        }

        private void UsedPlayerAction(Deck deck)
        {
            if (deck != ControlledDeck)
                return;

            PlayerActionsLeft--;

            if (PlayerActionsLeft <= 0)
            {
                TurnFinished.Invoke(); 
            }

        }

        public void YourTurn()
        {
            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            PlayerActionsLeft = GameSettings.Instance.PlayerPlaysPrTurn;

        }

        public bool ActionAvailable()
        {
            return PlayerActionsLeft > 0;
        }
    }
}