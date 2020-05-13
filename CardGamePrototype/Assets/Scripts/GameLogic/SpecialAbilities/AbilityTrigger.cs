using System;
using UnityEngine.Events;

namespace GameLogic
{
    public abstract class AbilityTrigger
    {

        public abstract Ability.Verb TriggerType { get; }

        internal abstract UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue);

        internal abstract string Description(string instigatorString);
        internal abstract float GetValue();
    }
}