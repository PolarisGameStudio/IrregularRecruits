using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public abstract class Noun
{
    public Noun() { }

    public abstract Ability.Noun NounType { get; }

    public abstract bool Appliable(Card executer, Card abilityOwner);

    public abstract List<Card> GetTargets(Action action, Count count, Deck.Zone location, Card _owner, Card triggerExecuter);

}
