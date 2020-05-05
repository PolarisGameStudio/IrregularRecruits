using System;
using System.Collections.Generic;

namespace GameLogic
{
    public partial class Ability
    {
        private static Dictionary<ActionType, Deck.Zone> ForcedActionTargetLocations = new Dictionary<ActionType, Deck.Zone>()
        {
            { ActionType.Resurrect,Deck.Zone.Graveyard },
            { ActionType.Withdraw,Deck.Zone.Battlefield },
        };

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
            public Noun Target;
            public int Amount;
            public Creature Summons;

            public Action(ActionType actionType, Count targetCount, int amount, Noun target, Creature summon = null)
            {
                ActionType = actionType;
                TargetCount = targetCount;
                Target = target;
                Amount = amount;
                Summons = summon;

                if (actionType == ActionType.Summon && !summon)
                    throw new Exception("No summon for summon ability");
                if (summon &! summon.IsSummon())
                    throw new Exception($"Summon {summon} is not a summon");

                //force target locations for action types. TODO: should be defined through structs instead of a switch
                if (ForcedActionTargetLocations.ContainsKey(actionType))
                    Target.Location = ForcedActionTargetLocations[actionType];
            }

            public string Description(Creature owner)
            {
                return AbilityProcessor.GetAction(ActionType).Description(Target.NounAsString(owner, TargetCount), Amount,Summons);
            }


            //Negative for an enemy target. Positive for a friendly or neutral target. More impactfull raises value
            public float GetValue()
            {
                return AbilityProcessor.GetAction(ActionType).GetValue(GetTargetTypeValue() * GetAmountOfTargetsValue(), Amount);
            }

            //Negative if enemy target - so kill enemy becomes a net positive ability
            private float GetTargetTypeValue(bool neutralAsNegative = false)
            {
                var value = 1f;

                //OTHER factors could change value up or down

                if (Target.Relationship == Noun.Allegiance.Enemy || (neutralAsNegative && Target.Relationship == Noun.Allegiance.Any)) value *= -1;

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
}