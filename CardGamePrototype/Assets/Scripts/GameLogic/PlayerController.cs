using System;

namespace GameLogic
{
    public class PlayerController : IDeckController
    {
        private Deck ControlledDeck;
        private Action TurnFinished;
        private bool MyTurn = false;

        public void SetupDeckActions(Deck deck, Action onfinish)
        {
            ControlledDeck = deck;

            TurnFinished = onfinish;

            Event.OnPlayerAction.AddListener(CheckForTurnfinish);
        }

        private void CheckForTurnfinish()
        {
            if (BattleManager.Instance.PlayerActionsLeft <= 0 && MyTurn)
            {
                MyTurn = false;
                TurnFinished.Invoke(); 
            }

        }

        public void YourTurn()
        {
            MyTurn = true;

            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            BattleManager.Instance.PlayerActionsLeft = GameSettings.Instance.PlayerPlaysPrTurn;

        }
    }
}