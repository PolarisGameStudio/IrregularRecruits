using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ETBTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.ETB;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString} enters the battlefield";
    }

    internal override float GetValue()
    {
        return 1f;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnPlay.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
