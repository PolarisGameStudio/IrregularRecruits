using System;

namespace GameLogic
{
    public abstract class DeckController
    {
        protected Deck ControlledDeck;
        protected Action OnFinish;

        public int ActionsLeft;

        public DeckController(Deck controlledDeck)
        {
            ControlledDeck = controlledDeck;

            Event.OnPlayerAction.AddListener(UsedAction);
        }

        public abstract void SetupDeckActions(Deck deck, Action onfinish);
        public abstract void YourTurn();
        public bool ActionAvailable()
        {
            //TODO: should AI actions always be possible?
            return ActionsLeft > 0;
        }
        public abstract void UsedAction(Deck deck);
        public void ResetActions()
        {
            ActionsLeft = GameSettings.Instance.PlaysPrTurn;
        }
    }
}