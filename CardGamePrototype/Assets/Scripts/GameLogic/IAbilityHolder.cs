using System;

namespace GameLogic
{
    public interface IAbilityHolder
    {
        Race Race();
        Deck InDeck();
        Guid Guid();
    }
}