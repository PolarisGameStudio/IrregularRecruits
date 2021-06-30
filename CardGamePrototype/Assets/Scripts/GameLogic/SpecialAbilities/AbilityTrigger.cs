using System;
using UnityEngine.Events;

namespace GameLogic
{
    public abstract class AbilityTrigger
    {

        public abstract TriggerType TriggerType { get; }

        internal abstract UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue);

        internal abstract string Description(string instigatorString, bool firstPerson);
        internal abstract float GetValue();
    }
}