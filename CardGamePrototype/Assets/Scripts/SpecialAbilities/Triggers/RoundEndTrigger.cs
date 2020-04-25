using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundEndTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.RoundEnd;

    internal override string Description(string instigatorString)
    {
        return "at the end of each combat round";
    }

    internal override float GetValue()
    {
        return 1f;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnCombatResolveFinished.AddListener(() => executeIfTrue.Invoke(null, owner, subjekt));
    }
}
