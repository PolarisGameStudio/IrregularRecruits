using System;
using System.Collections.Generic;

namespace GameLogic
{

    [Serializable]
    public struct AbilityEffectObject
    {
        private static Dictionary<EffectType, Deck.Zone> ForcedEffectTargetLocations = new Dictionary<EffectType, Deck.Zone>()
        {
            { EffectType.Resurrect,Deck.Zone.Graveyard },
            { EffectType.Withdraw,Deck.Zone.Battlefield },
            { EffectType.Rally,Deck.Zone.Library},
        };

        public EffectType ActionType;
        public Count TargetCount;
        public Noun Target;
        public int Amount;
        public Creature Summons;

        public AbilityEffectObject(EffectType actionType, Count targetCount, int amount, Noun target, Creature summon = null)
        {
            ActionType = actionType;
            TargetCount = targetCount;
            Target = target;
            Amount = amount;
            Summons = summon;

            if (actionType == EffectType.Summon && !summon)
                throw new Exception("No summon for summon ability");
            if (summon && !summon.IsSummon())
                throw new Exception($"Summon {summon} is not a summon");

            //force target locations for action types. TODO: should be defined through structs instead of a switch
            if (ForcedEffectTargetLocations.ContainsKey(actionType))
                Target.Location = ForcedEffectTargetLocations[actionType];
        }

        public string Description(ICharacter owner)
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
            else if (Target.Race == Noun.RaceType.Different) value *= -1;

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
                    return 1.6f;
                case Count.One:
                case Count.COUNT:
                default:
                    return 1f;
            }
        }


    }

}