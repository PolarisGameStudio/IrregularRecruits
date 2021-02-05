using System.Collections.Generic;

namespace GameLogic
{
    public class RallyAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Rally;

        public override string Description(string v, int amount, Creature summon)
        {

            return $"rally {v}";
        }

        public override void ExecuteAction(Ability ability, AbilityHolder _owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, _owner, targets);
            foreach(var target in targets)
            {
                target.Rally();
            }
        }

        public override float GetValue(float targetvalue, int amount)
        {
            return targetvalue * 1f;
        }
    }
}