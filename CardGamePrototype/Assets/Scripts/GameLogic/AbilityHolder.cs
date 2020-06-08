using System;
using UnityEngine.Events;

namespace GameLogic
{
    public interface ICharacter
    {
        string GetName();
         Race GetRace();
        //Deck GetDeck();
        //Guid GetGuid();

    }

    public abstract class AbilityHolder : ICharacter
    {
        protected string Name;
        public Deck InDeck;
        public Guid Guid = System.Guid.NewGuid();

        public UnityAction RemoveListenerAction;
        public bool ListenersInitialized = false;

        public abstract Race GetRace();

        public string GetName()
        {
            return Name;
        }

    }
}