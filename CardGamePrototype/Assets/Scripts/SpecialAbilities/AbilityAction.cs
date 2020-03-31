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


    protected List<Card> GetTargets(Action resultingAction, Ability.Noun targetType, Count count, Deck.Zone location, Card _owner, Card triggerExecuter)
    {
        List<Card> cs = new List<Card>();
        switch (resultingAction.Targets)
        {
            case Ability.Noun.THIS:
                if (_owner.Location == location)
                    cs.Add(_owner);
                break;
            case Ability.Noun.ANY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location).ToList();
                break;
            case Ability.Noun.FRIENDLY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.InDeck == _owner.InDeck).ToList();
                break;
            case Ability.Noun.ENEMY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.InDeck != _owner.InDeck).ToList();
                break;
            case Ability.Noun.OwnerRACE:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.Creature.Race ==_owner.Creature.Race).ToList();
                break;
            case Ability.Noun.DAMAGED:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.CurrentHealth < c.MaxHealth).ToList();
                break;
            case Ability.Noun.UNDAMAGED:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.CurrentHealth >= c.MaxHealth).ToList();
                break;
            case Ability.Noun.CardInOwnersHand:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Hand && c.InDeck == _owner.InDeck).ToList();

                break;
            case Ability.Noun.CardInOwnersDeck:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Library && c.InDeck == _owner.InDeck).ToList();

                break;
            case Ability.Noun.IT:
                if (triggerExecuter != null && triggerExecuter.Location == location)
                    cs.Add(triggerExecuter);
                break;
            case Ability.Noun.NotOwnerRACE:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.Creature.Race !=_owner.Creature.Race).ToList();

                break;
            case Ability.Noun.CardInOpponentsHand:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Hand && c.InDeck != _owner.InDeck).ToList();

                break;
            case Ability.Noun.CardInOpponentsDeck:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Library && c.InDeck != _owner.InDeck).ToList();
                break;
            case Ability.Noun.COUNT:
            default:
                return cs;
        }
        return TakeCount(cs, resultingAction.TargetCount);
    }

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
