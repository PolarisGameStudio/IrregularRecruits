using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ability
{

    //Also includes redundant statements like heal damaged
    //TODO: should be moved to a value in Verb Classes
    public static Dictionary<ActionType, List<Noun>> InconsistentActionTargets = new Dictionary<ActionType, List<Noun>>
    {
        { ActionType.Charm,new List<Noun>() {Noun.FRIENDLY,Noun.CardInOwnersDeck,Noun.CardInOpponentsHand,Noun.CardInOpponentsDeck,Noun.CardInOwnersHand,Noun.COUNT } },//is enemy redudant? Or any?
        { ActionType.Clone,new List<Noun>() {Noun.COUNT } },
        { ActionType.Copy,new List<Noun>() { Noun.IT, Noun.COUNT } },
        { ActionType.DealDamage,new List<Noun>() { Noun.COUNT } },
        { ActionType.Heal,new List<Noun>() {Noun.UNDAMAGED,Noun.DAMAGED, Noun.COUNT } },
        { ActionType.Kill,new List<Noun>() { Noun.COUNT } },
        { ActionType.Resurrect,new List<Noun>() {Noun.DAMAGED,Noun.UNDAMAGED, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand, Noun.CardInOwnersHand, Noun.CardInOwnersDeck, Noun.COUNT } },
        { ActionType.StatMinus,new List<Noun>() { Noun.COUNT } },
        { ActionType.StatPlus,new List<Noun>() { Noun.COUNT } },
        { ActionType.Withdraw,new List<Noun>() {Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand, Noun.CardInOwnersHand,Noun.CardInOwnersDeck, Noun.COUNT } },
    };

    [Serializable]
    public struct Action
    {
        public ActionType ActionType;
        public Count TargetCount;
        public Noun Targets;
        public int Amount;

        public Action(ActionType actionType, Count targetCount, Noun targets, int amount)
        {
            ActionType = actionType;
            TargetCount = targetCount;
            Targets = targets;
            Amount = amount;
        }

        public string Description(Card owner) {
            switch (ActionType)
            {
                case ActionType.Kill:
                    return $"kill {NounAsString(Targets, owner, TargetCount)}";
                case ActionType.DealDamage:
                    return $"deal {Amount} damage to {NounAsString(Targets, owner, TargetCount)}";
                case ActionType.StatPlus:
                    return $"{NounAsString(Targets, owner, TargetCount)} get +{Amount} Attack and Health";
                case ActionType.StatMinus:
                    return $"{NounAsString(Targets, owner, TargetCount)} get -{Amount} Attack and Health";
                case ActionType.Withdraw:
                    return $"withdraw {NounAsString(Targets, owner, TargetCount)}";
                case ActionType.Heal:
                    return $"heal {NounAsString(Targets, owner, TargetCount)} for {Amount}";
                case ActionType.Resurrect:
                    return $"resurrect {NounAsString(Targets, owner, TargetCount)} with {Amount} health";
                case ActionType.Draw:
                    return $"draw {Amount} cards";
                case ActionType.Charm:
                    return $"take control of {NounAsString(Targets, owner, TargetCount)}";
                case ActionType.Summon:
                    return $"summon {NounAsString(Targets, owner, TargetCount)}";
                case ActionType.COUNT:
                default:
                    return $"{ActionType} {TargetCount} {Targets}";
            }
        }


        //Negative for an enemy target. Positive for a friendly or neutral target. More impactfull raises value
        public float GetValue()
        {
            switch (ActionType)
            {
                case ActionType.Kill:
                    return -3f * GetTargetTypeValue(true) * GetAmountOfTargetsValue();
                case ActionType.Charm:
                    return -4f * GetTargetTypeValue(true) * GetAmountOfTargetsValue();
                case ActionType.DealDamage:
                    return -1f * GetTargetTypeValue(true) * GetAmountOfTargetsValue() * (1+Amount/20f);
                case ActionType.StatMinus:
                    return -1f * GetTargetTypeValue(true) * GetAmountOfTargetsValue() * (1 + Amount / 10f);
                case ActionType.Withdraw:
                    return 1f * GetAmountOfTargetsValue() ;
                case ActionType.StatPlus:
                    return 1.5f * GetTargetTypeValue() * GetAmountOfTargetsValue() * (1 + Amount / 2);
                case ActionType.Heal:
                    return 0.5f * GetTargetTypeValue() * GetAmountOfTargetsValue() * (1 + Amount / 20f);
                case ActionType.Resurrect:
                    return 3.3f * GetTargetTypeValue() * GetAmountOfTargetsValue() * (1 + Amount / 20f);
                case ActionType.Draw:
                    return 1f * Amount;
                case ActionType.Summon:
                case ActionType.Clone:
                case ActionType.Copy:
                case ActionType.COUNT:
                default:
                    return 1f * GetTargetTypeValue() * GetAmountOfTargetsValue() * (1 + Amount / 20f);
            }
        }

        //Negative if enemy target - so kill enemy becomes a net positive ability
        private float GetTargetTypeValue(bool neutralAsNegative = false)
        {
            switch (Targets)
            {
                case Noun.ANY:
                case Noun.IT:
                case Noun.DAMAGED:
                case Noun.UNDAMAGED:
                    return (neutralAsNegative ? -0.5f:0.5f);
                case Noun.THIS:
                case Noun.FRIENDLY:
                case Noun.OwnerRACE:
                case Noun.CardInOwnersHand:
                case Noun.CardInOwnersDeck:
                    return 1f;
                case Noun.ENEMY:
                case Noun.NotOwnerRACE:
                case Noun.CardInOpponentsDeck:
                case Noun.CardInOpponentsHand:
                    return -1f;
                case Noun.COUNT:
                default:
                    return 0.5f;
            }
        }

        private float GetAmountOfTargetsValue()
        {
            switch (TargetCount)
            {
                case Count.All:
                    return 2f;
                case Count.Two:
                    return 1.25f;
                case Count.Three:
                    return 1.5f;
                case Count.One:
                case Count.COUNT:
                default:
                    return 1f;
            }
        }
    }

    internal bool AnyActionInconsistencies()
    {
        return InconsistentActionTargets.Any(a=> a.Key == ResultingAction.ActionType && a.Value.Contains(ResultingAction.Targets));
    }
}
