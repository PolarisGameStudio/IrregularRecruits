using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrawTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.Draw;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString}'s controller draws a card";
    }

    internal override float GetValue()
    {
        return 1f;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnDraw.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
