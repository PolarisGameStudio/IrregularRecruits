using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public abstract class AbilityWithEffect : SpecialAbility
    {

        public static int AbilityStackCount;
        private static readonly int MaxAbilityStack = 12;

        public AbilityEffectObject ResultingAction;

        protected void ExecuteAction(AbilityHolder owner, Card triggerExecuter)
        {
            //Debug.Log("Trigger: " + TriggerCondition.Description(owner.Creature) + " is true");
            //Debug.Log("Executing: " + ResultingAction.Description(owner.Creature));

            AbilityStackCount++;

            if(AbilityStackCount >= MaxAbilityStack)
            {
                Debug.LogWarning("Terminating ability, due to stack size");
                return;
            }


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

    }
}