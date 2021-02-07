using System;
using UnityEngine.Events;

namespace GameLogic
{

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

        //whether the ability can trigger, e.g. the owner if minion, is on the battlefield
        internal abstract bool IsActive();
    }
}