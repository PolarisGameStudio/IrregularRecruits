using System.Collections.Generic;

namespace GameLogic
{
    public class KillAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Kill;

        public override string Description(string target, int amount, Creature summon)
        {
            return "kill " + target;
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {

            Event.OnAbilityTrigger.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Die());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -3f * targetValue;
        }
    }
}