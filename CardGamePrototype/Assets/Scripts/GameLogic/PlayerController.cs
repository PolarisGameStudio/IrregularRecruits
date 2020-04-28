using System;

namespace GameLogic
{
    public class PlayerController : IDeckController
    {
        private Deck ControlledDeck;
        private Action OnFinish;

        public void SetupDeckActions(Deck deck, Action onfinish)
        {
            throw new NotImplementedException();
        }

        public void YourTurn()
        {
            throw new NotImplementedException();
        }
    }
}