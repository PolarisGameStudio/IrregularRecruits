using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsAttackedTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.IsATTACKED;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString } is attacked";
    }

    internal override float GetValue()
    {
        return 3f;
    }

    internal override void SetupListener(Card owner, Ability.NounType subjekt, UnityAction<Card, Card, Ability.NounType> executeIfTrue)
    {
        Event.OnBeingAttacked.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
