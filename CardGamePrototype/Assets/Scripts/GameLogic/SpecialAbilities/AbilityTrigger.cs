using System;
using UnityEngine.Events;

namespace GameLogic
{
    public abstract class AbilityTrigger
    {

        public abstract Verb TriggerType { get; }

        internal abstract UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue);

        internal abstract string Description(string instigatorString);
        internal abstract float GetValue();
    }
}