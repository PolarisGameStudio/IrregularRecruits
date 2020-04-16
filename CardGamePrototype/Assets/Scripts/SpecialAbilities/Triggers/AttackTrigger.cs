using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.ATTACKS;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString} attacks";
    }

    internal override float GetValue()
    {
        return 3f;
    }

    internal override void SetupListener(Card owner, Ability.NounType subjekt, UnityAction<Card, Card, Ability.NounType> executeIfTrue)
    {
        Event.OnAttack.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
