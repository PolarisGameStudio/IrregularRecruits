using System;
using UnityEngine.Events;

namespace GameLogic
{
    public abstract class AbilityTrigger
    {

        public abstract PassiveAbility.Verb TriggerType { get; }

        internal abstract UnityAction SetupListener(IAbilityHolder owner, Noun subjekt, UnityAction<Card, IAbilityHolder, Noun> executeIfTrue);

        internal abstract string Description(string instigatorString);
        internal abstract float GetValue();
    }
}