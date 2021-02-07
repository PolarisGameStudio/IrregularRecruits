using System.Collections.Generic;

namespace GameLogic
{
    public class KillAction : AbilityAction
    {
        public override ActionType ActionType => ActionType.Kill;

        public override string Description(string target, int amount, Creature summon)
        {
            return "kill " + target;
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
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