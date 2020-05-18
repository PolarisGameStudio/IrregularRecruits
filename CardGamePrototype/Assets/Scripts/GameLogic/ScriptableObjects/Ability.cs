using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;


namespace GameLogic
{

    public abstract class Ability : ScriptableObject
    {

        protected static Dictionary<ActionType, Deck.Zone> ForcedActionTargetLocations = new Dictionary<ActionType, Deck.Zone>()
        {
            { ActionType.Resurrect,Deck.Zone.Graveyard },
            { ActionType.Withdraw,Deck.Zone.Battlefield },
        };

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

        public Action ResultingAction;


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
                if (summon && !summon.IsSummon())
                    throw new Exception($"Summon {summon} is not a summon");

                //force target locations for action types. TODO: should be defined through structs instead of a switch
                if (ForcedActionTargetLocations.ContainsKey(actionType))
                    Target.Location = ForcedActionTargetLocations[actionType];
            }

            public string Description(Creature owner)
            {
                return AbilityProcessor.GetAction(ActionType).Description(Target.NounAsString(owner, TargetCount), Amount, Summons);
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

    }
}