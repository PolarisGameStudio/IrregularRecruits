using System.Collections.Generic;

namespace GameLogic
{
    public class StatMinusAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.StatMinus;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"{target} lose {amount} Attack and Health";
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.StatModifier(-ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -1f * targetValue * (1 + amount / 3f);
        }
    }
}