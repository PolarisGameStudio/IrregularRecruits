using System.Collections.Generic;

namespace GameLogic
{
    public class HealAction : AbilityAction
    {
        public override PassiveAbility.ActionType ActionType => PassiveAbility.ActionType.Heal;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"Heal {target} for {amount}";
        }

        public override void ExecuteAction(Ability ability, AbilityHolder owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.HealthChange(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 0.5f * targetValue * (1 + amount / 20f);
        }
    }
}