using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ability
{

    //Also includes redundant statements like heal damaged
    //TODO: should be moved to a value in Verb Classes
    //public static Dictionary<ActionType, List<Noun>> InconsistentActionTargets = new Dictionary<ActionType, List<Noun>>
    //{
    //    { ActionType.Charm,new List<Noun>() {Noun.FRIENDLY,Noun.CardInOwnersDeck,Noun.CardInOpponentsHand,Noun.CardInOpponentsDeck,Noun.CardInOwnersHand,Noun.COUNT } },//is enemy redudant? Or any?
    //    { ActionType.Clone,new List<Noun>() {Noun.COUNT } },
    //    { ActionType.Copy,new List<Noun>() { Noun.IT, Noun.COUNT } },
    //    { ActionType.DealDamage,new List<Noun>() { Noun.COUNT } },
    //    { ActionType.Heal,new List<Noun>() {Noun.UNDAMAGED,Noun.DAMAGED, Noun.COUNT } },
    //    { ActionType.Kill,new List<Noun>() { Noun.COUNT } },
    //    { ActionType.Resurrect,new List<Noun>() {Noun.DAMAGED,Noun.UNDAMAGED, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand, Noun.CardInOwnersHand, Noun.CardInOwnersDeck, Noun.COUNT } },
    //    { ActionType.StatMinus,new List<Noun>() { Noun.COUNT } },
    //    { ActionType.StatPlus,new List<Noun>() { Noun.COUNT } },
    //    { ActionType.Withdraw,new List<Noun>() {Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand, Noun.CardInOwnersHand,Noun.CardInOwnersDeck, Noun.COUNT } },
    //};

    [Serializable]
    public struct Action
    {
        public ActionType ActionType;
        public Count TargetCount;
        public NounType Target;
        public int Amount;

        public Action(ActionType actionType, Count targetCount, int amount, NounType target)
        {
            ActionType = actionType;
            TargetCount = targetCount;
            Target = target;
            Amount = amount;
        }

        public string Description(Card owner) {
            switch (ActionType)
            {
                case ActionType.Kill:
                    return $"kill {NounAsString(Target, owner, TargetCount)}";
                case ActionType.DealDamage:
                    return $"deal {Amount} damage to {NounAsString(Target, owner, TargetCount)}";
                case ActionType.StatPlus:
                    return $"{NounAsString(Target, owner, TargetCount)} get +{Amount} Attack and Health";
                case ActionType.StatMinus:
                    return $"{NounAsString(Target, owner, TargetCount)} get -{Amount} Attack and Health";
                case ActionType.Withdraw:
                    return $"withdraw {NounAsString(Target, owner, TargetCount)}";
                case ActionType.Heal:
                    return $"heal {NounAsString(Target, owner, TargetCount)} for {Amount}";
                case ActionType.Resurrect:
                    return $"resurrect {NounAsString(Target, owner, TargetCount)} with {Amount} health";
                case ActionType.Draw:
                    return $"draw {Amount} cards";
                case ActionType.Charm:
                    return $"take control of {NounAsString(Target, owner, TargetCount)}";
                case ActionType.Summon:
                    return $"summon {NounAsString(Target, owner, TargetCount)}";
                case ActionType.COUNT:
                default:
                    return $"{ActionType} {TargetCount} {NounAsString(Target, owner, TargetCount)}";
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
            var value = 1f;

            //OTHER factors could change value up or down

            if (Target.Relationship == Allegiance.Enemy || (neutralAsNegative && Target.Relationship == Allegiance.Any)) value *= -1;

            return value;
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

    //internal bool AnyActionInconsistencies()
    //{
    //    return InconsistentActionTargets.Any(a=> a.Key == ResultingAction.ActionType && a.Value.Contains(ResultingAction.Targets));
    //}
}
