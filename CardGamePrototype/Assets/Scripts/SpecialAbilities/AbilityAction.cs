using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;

public abstract class AbilityAction 
{
    public AbilityAction() { }

    public abstract Ability.ActionType ActionType { get; }
    public abstract string Description(Ability.ActionType action, Card owner);
    public abstract void ExecuteAction(Card _owner, Card triggerExecuter);
    public abstract int GetValue();


    private List<Card> TakeCount(List<Card> cards, Count count)
    {
        if (cards.Count == 0) return cards;

        switch (count)
        {
            case Count.All:
                return cards;
            case Count.One:
                return new List<Card>() { cards[Random.Range(0, cards.Count())] };
            case Count.Two:
                cards.OrderBy(o => Random.value);
                return cards.Take(2).ToList();
            case Count.Three:
                cards.OrderBy(o => Random.value);
                return cards.Take(2).ToList();
            default:
                return cards;
        }
    }


}
