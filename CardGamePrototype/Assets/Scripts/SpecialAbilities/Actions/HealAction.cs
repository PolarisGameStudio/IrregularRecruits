using System.Collections.Generic;

namespace GameLogic
{
    public class HealAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Heal;

        public override string Description(string target, int amount)
        {
            return $"Heal {target} for {amount}";
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {
            Event.OnAbilityTrigger.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Heal(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 0.5f * targetValue * (1 + amount / 20f);
        }
    }
}