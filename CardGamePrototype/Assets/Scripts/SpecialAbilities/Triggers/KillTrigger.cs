using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.KILLS;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString } kills a minion";
    }

    internal override float GetValue()
    {
        return 1f;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnKill.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
