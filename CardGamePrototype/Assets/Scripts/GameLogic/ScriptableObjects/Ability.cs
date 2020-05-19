using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

        protected void ExecuteAction(AbilityHolder owner, Card triggerExecuter)
        {
            //Debug.Log("Trigger: " + TriggerCondition.Description(owner.Creature) + " is true");
            //Debug.Log("Executing: " + ResultingAction.Description(owner.Creature));

            List<Card> targets = GetTargets(ResultingAction.Target, owner, triggerExecuter);

            AbilityProcessor.GetAction(ResultingAction.ActionType).ExecuteAction(this, owner, targets);
        }

        public List<Card> GetTargets(Noun targetType, AbilityHolder _owner, Card triggerExecuter)
        {
            List<Card> cardsInZone = BattleManager.Instance.GetCardsInZone(targetType.Location);

            List<Card> cs = cardsInZone.Where(c =>
                targetType.CorrectCharacter(c, _owner, triggerExecuter) &&
                targetType.CorrectAllegiance(c, _owner) &&
                targetType.CorrectDamageState(c) &&
                targetType.CorrectRace(c, _owner)).ToList();

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
                        return 1.6f;
                    case Count.One:
                    case Count.COUNT:
                    default:
                        return 1f;
                }
            }


        }

    }
}