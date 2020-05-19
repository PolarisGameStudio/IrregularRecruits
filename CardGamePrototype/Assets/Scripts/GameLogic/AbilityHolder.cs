using System;

namespace GameLogic
{
    public abstract class AbilityHolder
    {
        public abstract Race GetRace();
        public abstract Deck GetDeck();
        public abstract Guid GetGuid();
    }
}