using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public partial class Ability : ScriptableObject
{
    [SerializeField]
    private float Value;
    
    //could be composite (friendly undamaged OwnerRace) Negated (!OwnerRace)
    public enum Noun { 
        THIS,
        ANY, // can also mean random
        IT, //refers to the executer of trigger
        FRIENDLY,
        ENEMY,
        OwnerRACE,
        NotOwnerRACE,
        DAMAGED,
        UNDAMAGED, 
        CardInOpponentsHand,
        CardInOpponentsDeck,
        CardInOwnersHand, 
        CardInOwnersDeck,
        COUNT
    }
    public enum Verb
    {
        ATTACKS, 
        IsATTACKED, 
        KILLS,
        DIES,
        ETB,
        IsDAMAGED,
        IsHealed,
        Draw,
        Withdraw,
        RoundEnd,
        COUNT
    }
    public enum ActionType
    {
        Kill,
        DealDamage,
        StatPlus,
        StatMinus,
        Withdraw,
        //Discard, use kill in hand instead
        Heal,
        Resurrect,
        Draw,
        Charm,
        Summon,
        Clone,
        Copy,
        COUNT
    }
    public enum Count
    {
        All, One, Two,Three,
        COUNT
    }

    public Trigger TriggerCondition;
    public Action ResultingAction;

    public override string ToString()
    {
        return base.ToString();
    }
    public string Description(Card owner)
    {
        return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.({GetValue().ToString("N")})"; 
    }

    public void SetupListeners(Card _owner)
    {

        //TODO: replace with CardEvent Reference
        switch (TriggerCondition.TriggerAction)
        {
            case Verb.ATTACKS:
                Event.OnAttack.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.IsATTACKED: 
                Event.OnBeingAttacked.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.KILLS:
                Event.OnKill.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.DIES:
                Event.OnDeath.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.ETB:
                Event.OnPlay.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.IsDAMAGED:
                Event.OnDamaged.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.Draw:
                Event.OnDraw.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.Withdraw:
                Event.OnWithdraw.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.IsHealed:
                Event.OnHealed.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subject));
                break;
            case Verb.RoundEnd:
                Event.OnCombatRoundFinished.AddListener(() => ExecuteIfTrue(null, _owner, TriggerCondition.Subject));
                break;
            case Verb.COUNT:
                break;
        }
    }

    public float GetValue()
    {
        Value = TriggerCondition.GetValue() * ResultingAction.GetValue();
        return Value;
    }


    private void ExecuteIfTrue(Card instigator, Card abilityOwner, Noun subject)
    {
        if (CorrectNoun(instigator, abilityOwner,subject))
            ExecuteAction(abilityOwner,instigator);
    }

    private bool CorrectNoun(Card instigator, Card abilityOwner, Noun subject)
    {
        if (abilityOwner.Location != Deck.Zone.Battlefield) return false;

        //Trigger Actions without instigators always has correct noun
        if (instigator == null) return true;

        switch (subject)
        {
            case Noun.THIS:
                return instigator == abilityOwner;
            case Noun.ANY:
                return true;
            case Noun.FRIENDLY:
                return instigator.InDeck == abilityOwner.InDeck;
            case Noun.ENEMY:
                return instigator.InDeck != abilityOwner.InDeck;
            case Noun.OwnerRACE:
                return instigator.Creature.Race ==abilityOwner.Creature.Race;
            case Noun.NotOwnerRACE:
                return instigator.Creature.Race != abilityOwner.Creature.Race;
            case Noun.DAMAGED:
                return instigator.CurrentHealth < instigator.MaxHealth;
            case Noun.UNDAMAGED:
                return instigator.CurrentHealth >= instigator.MaxHealth;
            case Noun.CardInOwnersHand:
            case Noun.CardInOpponentsHand:
                return instigator.Location == Deck.Zone.Hand;
            case Noun.CardInOwnersDeck:
            case Noun.CardInOpponentsDeck:
                return instigator.Location == Deck.Zone.Library;
            case Noun.IT:
                return false;
            case Noun.COUNT:
            default:
                return false;
        }
    }

    private void ExecuteAction(Card _owner, Card triggerExecuter)
    {
        Debug.Log("Trigger: " + TriggerCondition.Description(_owner) + " is true");
        Debug.Log("Executing: " + ResultingAction.Description(_owner));
        _owner.OnAbilityTrigger.Invoke();


        List<Card> targets = GetTargets(ResultingAction.Targets, ResultingAction.TargetCount, Deck.Zone.Battlefield, _owner, triggerExecuter);
        switch (ResultingAction.ActionType)
        {
            case ActionType.Kill:
                targets.ForEach(c=> c.Die());
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.DealDamage:
                targets.ForEach(c => c.CurrentHealth -= ResultingAction.Amount);
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.StatPlus:
                targets.ForEach(c => c.StatModifier(ResultingAction.Amount));
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.StatMinus:
                targets.ForEach(c => c.StatModifier(-ResultingAction.Amount));
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.Withdraw:
                targets.ForEach(c => c.Withdraw());
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.Heal:
                targets.ForEach(c=> c.CurrentHealth += ResultingAction.Amount);
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.Resurrect:
                List<Card> graveTargets = GetTargets(ResultingAction.Targets, ResultingAction.TargetCount, Deck.Zone.Graveyard, _owner, triggerExecuter);
                graveTargets.ForEach(c => c.Resurrect(ResultingAction.Amount));
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, graveTargets));
                break;
            case ActionType.Draw:
                _owner.InDeck.Draw(ResultingAction.Amount);
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, new List<Card>()));
                break;
            case ActionType.Charm:
                targets.ForEach(c => c.Charm(_owner.InDeck));
                EventController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                break;
            case ActionType.Summon:
                Debug.Log("summoning not implemented");
                break;
            default:
                break;
        }


    }

    private List<Card> GetTargets(Noun targetType, Count count,Deck.Zone location, Card _owner,Card triggerExecuter )
    {
        List<Card> cs = new List<Card>();
        switch (ResultingAction.Targets)
        {
            case Noun.THIS:
                if (_owner.Location == location)
                    cs.Add(_owner);
                break;
            case Noun.ANY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location).ToList();
                break;
            case Noun.FRIENDLY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.InDeck == _owner.InDeck).ToList();
                break;
            case Noun.ENEMY:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.InDeck != _owner.InDeck).ToList();
                break;
            case Noun.OwnerRACE:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.Creature.Race ==_owner.Creature.Race).ToList();
                break;
            case Noun.DAMAGED:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.CurrentHealth < c.MaxHealth).ToList();
                break;
            case Noun.UNDAMAGED:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.CurrentHealth >= c.MaxHealth).ToList();
                break;
            case Noun.CardInOwnersHand:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Hand && c.InDeck == _owner.InDeck).ToList();

                break;
            case Noun.CardInOwnersDeck:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Library && c.InDeck == _owner.InDeck).ToList();

                break;
            case Noun.IT:
                if (triggerExecuter != null && triggerExecuter.Location == location)
                    cs.Add(triggerExecuter);
                break;
            case Noun.NotOwnerRACE:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == location && c.Creature.Race != _owner.Creature.Race).ToList();

                break;
            case Noun.CardInOpponentsHand:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Hand && c.InDeck != _owner.InDeck).ToList();

                break;
            case Noun.CardInOpponentsDeck:
                cs = CombatManager.GetCardsInZone(location).Where(c => c.Location == Deck.Zone.Library && c.InDeck != _owner.InDeck).ToList();
                break;
            case Noun.COUNT:
            default:
                return cs;
        }
        return TakeCount(cs, ResultingAction.TargetCount);
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

    private static string NounAsString(Noun noun,Card _owner,Count count = Count.One)
    {
        switch (noun)
        {
            case Noun.THIS:
                return _owner.Creature.name;
            case Noun.ANY:
                if (count == Count.One)
                    return "a minion";
                else
                    return count.ToString() + " minions";
            case Noun.FRIENDLY:
                if (count == Count.One)
                    return "a friendly minion";
                else
                    return count.ToString() + " friendly minions";
            case Noun.ENEMY:
                if (count == Count.One)
                    return "an enemy";
                else
                    return count.ToString() + " enemies";
            case Noun.OwnerRACE:
                if (count == Count.One)
                    return $"a {_owner.Creature.Race?.name}";
                else
                    return $"{count.ToString()} {_owner.Creature.Race?.name}s";
            case Noun.NotOwnerRACE:
                if (count == Count.One)
                    return $"a non-{_owner.Creature.Race?.name}";
                else
                    return $"{count.ToString()} non-{_owner.Creature.Race?.name}s";
            case Noun.DAMAGED:
                if (count == Count.One)
                    return "a damaged minion";
                else
                    return count.ToString() + " damaged minions";
            case Noun.UNDAMAGED:
                if (count == Count.One)
                    return "an undamaged minion";
                else
                    return count.ToString() + " undamaged minions";
            case Noun.CardInOwnersHand:
                if (count == Count.One)
                    return $"a minion in {_owner.Creature.name}'s controllers hand";
                else
                    return $"{count} minions in {_owner.Creature.name}'s controllers hand";
            case Noun.CardInOwnersDeck:
                if (count == Count.One)
                    return $"a minion in {_owner.Creature.name}'s controllers deck";
                else
                    return $"{count} minions in {_owner.Creature.name}'s controllers deck";
            case Noun.IT:
                return "it";
            case Noun.CardInOpponentsHand:
                if (count == Count.One)
                    return $"a minion in the enemy hand";
                else
                    return $"{count} minions in the enemy hand";
            case Noun.CardInOpponentsDeck:
                if (count == Count.One)
                    return $"a minion in the enemy deck";
                else
                    return $"{count} minions in the enemy deck";
            case Noun.COUNT:
            default:
                return "the green goose";
        }
    }

}
