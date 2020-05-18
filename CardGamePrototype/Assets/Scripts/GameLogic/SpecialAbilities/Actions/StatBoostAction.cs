using System.Collections.Generic;
namespace GameLogic
{
    public class StatBoostAction : AbilityAction
    {
        public override PassiveAbility.ActionType ActionType => PassiveAbility.ActionType.StatPlus;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"{target} gain {amount} Attack and Health";
        }

        public override void ExecuteAction(PassiveAbility ability, Card owner, List<Card> targets)
        {
            Event.OnAbilityTrigger.Invoke(ability, owner, targets);
            targets.ForEach(c => c.StatModifier(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 1.5f * targetValue * (1 + amount / 1.5f);
        }
    }
}