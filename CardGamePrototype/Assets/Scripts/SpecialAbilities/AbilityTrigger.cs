using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityTrigger
{

    public abstract Ability.Verb TriggerType { get; }

    internal abstract void SetupListener(Card owner, Noun subjekt, UnityAction<Card,Card,Noun> executeIfTrue);

    internal abstract string Description(string instigatorString);
    internal abstract float GetValue();
}
