using System;
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

    //TODO: old noun type. remove ocne data has been converted
    public enum Noun
    {
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
    public NounType NounToNounType(Noun noun)
    {
        switch (noun)
        {
            case Noun.THIS:
                return new NounType(Character.This);
            case Noun.ANY:
                return new NounType(Character.Any);
            case Noun.IT:
                return new NounType(Character.It);
            case Noun.FRIENDLY:
                return new NounType(Character.Any, Allegiance.Friend);
            case Noun.ENEMY:
                return new NounType(Character.Any, Allegiance.Enemy);
            case Noun.OwnerRACE:
                return new NounType(Character.Any, Allegiance.Any, DamageState.Any, RaceType.Same);
            case Noun.NotOwnerRACE:
                return new NounType(Character.Any, Allegiance.Any, DamageState.Any, RaceType.Different);
            case Noun.DAMAGED:
                return new NounType(Character.Any, Allegiance.Any, DamageState.Damaged);
            case Noun.UNDAMAGED:
                return new NounType(Character.Any, Allegiance.Any, DamageState.Undamaged);
            case Noun.CardInOpponentsHand:
                return new NounType(Character.Any, Allegiance.Enemy, DamageState.Any, RaceType.Any, Deck.Zone.Hand);
            case Noun.CardInOpponentsDeck:
                return new NounType(Character.Any, Allegiance.Enemy, DamageState.Any, RaceType.Any, Deck.Zone.Library);
            case Noun.CardInOwnersHand:
                return new NounType(Character.Any, Allegiance.Friend, DamageState.Any, RaceType.Any, Deck.Zone.Hand);
            case Noun.CardInOwnersDeck:
                return new NounType(Character.Any, Allegiance.Friend, DamageState.Any, RaceType.Any, Deck.Zone.Library);
            case Noun.COUNT:
                return new NounType();
            default:
                return new NounType();
        }
    }

    //TODO: move amount to noun as well
    [System.Serializable]
    public struct NounType
    {
        public Character Character;
        public Deck.Zone Location;
        public Allegiance Relationship;
        public DamageState DamageState;
        public RaceType Race;

        public NounType(Character character = Character.Any,  Allegiance relationship = Allegiance.Any, DamageState damageState = DamageState.Any, RaceType race = RaceType.Any, Deck.Zone location = Deck.Zone.Battlefield)
        {
            Character = character;
            Location = location;
            Relationship = relationship;
            DamageState = damageState;
            Race = race;
        }
    }
    public enum Character { Any, This, Other, It }
    public enum Allegiance { Any, Friend, Enemy };
    public enum DamageState { Any, Damaged, Undamaged };
    public enum RaceType { Any, Same, Different };

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
        All, One, Two, Three,
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
        return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.";
    }

    public void SetupListeners(Card _owner)
    {

        //TODO: replace with CardEvent Reference
        switch (TriggerCondition.TriggerAction)
        {
            case Verb.ATTACKS:
                Event.OnAttack.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.IsATTACKED:
                Event.OnBeingAttacked.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.KILLS:
                Event.OnKill.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.DIES:
                Event.OnDeath.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.ETB:
                Event.OnPlay.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.IsDAMAGED:
                Event.OnDamaged.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.Draw:
                Event.OnDraw.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.Withdraw:
                Event.OnWithdraw.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.IsHealed:
                Event.OnHealed.AddListener(a => ExecuteIfTrue(a, _owner, TriggerCondition.Subjekt));
                break;
            case Verb.RoundEnd:
                Event.OnCombatRoundFinished.AddListener(() => ExecuteIfTrue(null, _owner, TriggerCondition.Subjekt));
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


    private void ExecuteIfTrue(Card instigator, Card abilityOwner, NounType subject)
    {
        if (CorrectNoun(instigator, abilityOwner, subject))
            ExecuteAction(abilityOwner, instigator);
    }

    private bool CorrectNoun(Card instigator, Card abilityOwner, NounType subject)
    {
        if (abilityOwner.Location != Deck.Zone.Battlefield) return false;

        //Trigger Actions without instigators always has correct noun
        if (instigator == null) return true;

        return
            CorrectCharacter(instigator, abilityOwner, subject.Character) &&
            CorrectRace(instigator, abilityOwner, subject.Race) &&
            CorrectAllegiance(instigator, abilityOwner, subject.Relationship) &&
            CorrectDamageState(instigator, subject.DamageState) &&
            instigator.Location == subject.Location; 
    }

    private bool CorrectCharacter(Card instigator, Card abilityOwner, Character character, Card triggerExecuter = null)
    {
        switch (character)
        {
            case Character.Any:
                return true;
            case Character.This:
                return instigator == abilityOwner;
            case Character.Other:
                return instigator != abilityOwner;
            case Character.It:
                return triggerExecuter != null && instigator == triggerExecuter;
            default:
                return true;
        }
    }

    private bool CorrectDamageState(Card instigator, DamageState damageState)
    {
        switch (damageState)
        {
            case DamageState.Any:
                return true;
            case DamageState.Damaged:
                return instigator.CurrentHealth < instigator.MaxHealth;
            case DamageState.Undamaged:
                return instigator.CurrentHealth < instigator.MaxHealth;
            default: 
                return true;
        }
    }
private bool CorrectRace(Card instigator, Card abilityOwner, RaceType race)
    {
        switch (race)
        {
            case RaceType.Any:
                return true;
            case RaceType.Same:
                return instigator.Creature.Race == abilityOwner.Creature.Race;
            case RaceType.Different:
                return instigator.Creature.Race == abilityOwner.Creature.Race;
            default:
                return true;
        }
    }
    private bool CorrectAllegiance(Card instigator, Card abilityOwner, Allegiance ally)
    {
        if (instigator.InDeck == null || abilityOwner.InDeck == null)
            return false;

        switch (ally)
        {
            case Allegiance.Any:
                return true;
            case Allegiance.Friend:
                return instigator.InDeck == abilityOwner.InDeck;
            case Allegiance.Enemy:
                return instigator.InDeck != abilityOwner.InDeck;
            default:
                return true;
        }
    }

    private void ExecuteAction(Card _owner, Card triggerExecuter)
    {
        Debug.Log("Trigger: " + TriggerCondition.Description(_owner) + " is true");
        Debug.Log("Executing: " + ResultingAction.Description(_owner));
        _owner.OnAbilityTrigger.Invoke();


        List<Card> targets = GetTargets(ResultingAction.Target, ResultingAction.TargetCount, Deck.Zone.Battlefield, _owner, triggerExecuter);
        switch (ResultingAction.ActionType)
        {
            case ActionType.Kill:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.Die()));
                break;
            case ActionType.DealDamage:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.CurrentHealth -= ResultingAction.Amount));
                break;
            case ActionType.StatPlus:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.StatModifier(ResultingAction.Amount)));
                break;
            case ActionType.StatMinus:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.StatModifier(-ResultingAction.Amount)));
                break;
            case ActionType.Withdraw:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                targets.ForEach(c => c.Withdraw());
                break;
            case ActionType.Heal:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.CurrentHealth += ResultingAction.Amount));
                break;
            case ActionType.Resurrect:
                List<Card> graveTargets = GetTargets(ResultingAction.Target, ResultingAction.TargetCount, Deck.Zone.Graveyard, _owner, triggerExecuter);
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, graveTargets));
                graveTargets.ForEach(c => c.Resurrect(ResultingAction.Amount));
                break;
            case ActionType.Draw:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, new List<Card>()));
                _owner.InDeck.Draw(ResultingAction.Amount);
                break;
            case ActionType.Charm:
                FlowController.AddEvent(() =>
                        Event.OnAbilityTrigger.Invoke(this, _owner, targets));
                FlowController.AddEvent(() =>
                    targets.ForEach(c => c.Charm(_owner.InDeck)));
                break;
            case ActionType.Summon:
                Debug.Log("summoning not implemented");
                break;
            default:
                break;
        }


    }

    private List<Card> GetTargets(NounType targetType, Count count, Deck.Zone location, Card _owner, Card triggerExecuter)
    {
        List<Card> cs = CombatManager.GetCardsInZone(targetType.Location).Where(c =>
            CorrectCharacter(c, _owner, targetType.Character,triggerExecuter) &&
            CorrectAllegiance(c,_owner,targetType.Relationship)&& 
            CorrectDamageState(c,targetType.DamageState) &&
            CorrectRace(c,_owner,targetType.Race)).ToList();

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

    private static string NounAsString(NounType noun, Card _owner, Count count = Count.One)
    {
        var str = "";

        switch (noun.Character)
        {
            case Character.This:
                str += _owner.Creature.name;
                break;
            case Character.It:
                str += "it";
                break;
            case Character.Any:
                if (count == Count.One)
                    str+= "a ";
                else
                    str += count.ToString() + " ";
                break;
            case Character.Other:
                if (count == Count.One)
                    str += "another ";
                else
                    str += count.ToString() + " other ";
                break;
        }

        switch (noun.DamageState)
        {
            case DamageState.Any:
                break;
            case DamageState.Damaged:
                str += "damaged ";
                break;
            case DamageState.Undamaged:
                str += "damaged ";
                break;
        }
        switch (noun.Relationship)
        {
            case Allegiance.Any:
                break;
            case Allegiance.Friend:
                str += "friendly ";
                break;
            case Allegiance.Enemy:
                str += "enemy ";
                break;
        }

        switch (noun.Race)
        {
            case RaceType.Any:
                str += "minion" + (count == Count.One ? "" : "s");
                break;
            case RaceType.Same:
                str += _owner.Creature.Race?.name + (count == Count.One ? "" : "s");
                break;
            case RaceType.Different:
                str += "non-"+ _owner.Creature.Race?.name + (count == Count.One ? "" : "s");
                break;
        }

        switch (noun.Location)
        {
            case Deck.Zone.Library:
                switch (noun.Relationship)
                {
                    case Allegiance.Friend:
                        return $"{str} in {_owner.Creature.name}'s deck";
                    case Allegiance.Enemy:
                        return $"{str} in the enemy deck";
                    case Allegiance.Any:
                    default:
                        return $"{str} in a deck";
                }
            case Deck.Zone.Battlefield:
                return str;
            case Deck.Zone.Graveyard:
                return str + ", that are dead"; //TODO: remove. just for debugging now
            case Deck.Zone.Hand:
                switch (noun.Relationship)
                {
                    case Allegiance.Friend:
                        return $"{str} in {_owner.Creature.name}'s hand";
                    case Allegiance.Enemy:
                        return $"{str} in the enemy hand";
                    case Allegiance.Any:
                    default:
                        return $"{str} in a hand";
                }

            default:
                return str;
        }


    }

}
