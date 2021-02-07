using System.Collections.Generic;

namespace GameLogic
{
    public class WithdrawAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Withdraw;

        public override string Description(string target, int amount, Creature summon)
        {
            return "withdraw " + target;
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Withdraw());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return targetValue;
        }
    }
}