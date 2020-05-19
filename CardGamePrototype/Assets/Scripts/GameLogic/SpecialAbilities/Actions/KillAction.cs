using System.Collections.Generic;

namespace GameLogic
{
    public class KillAction : AbilityAction
    {
        public override PassiveAbility.ActionType ActionType => PassiveAbility.ActionType.Kill;

        public override string Description(string target, int amount, Creature summon)
        {
            return "kill " + target;
        }

        public override void ExecuteAction(Ability ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Die());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -3f * targetValue;
        }
    }
}