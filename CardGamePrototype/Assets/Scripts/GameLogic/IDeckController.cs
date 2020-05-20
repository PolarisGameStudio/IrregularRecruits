using System;

namespace GameLogic
{
    public interface IDeckController
    {

        void SetupDeckActions(Deck deck, Action onfinish);
        void YourTurn();
        bool ActionAvailable();
        void UsedAction(Deck deck);
        void ResetActions();
        int ActionsLeft();
    }
}