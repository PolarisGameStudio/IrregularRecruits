using System.Collections.Generic;

namespace GameLogic
{
    public class WithdrawAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Withdraw;

        public override string Description(string target, int amount)
        {
            return "withdraw " + target;
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {
            Event.OnAbilityTrigger.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Withdraw());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return targetValue;
        }
    }
}