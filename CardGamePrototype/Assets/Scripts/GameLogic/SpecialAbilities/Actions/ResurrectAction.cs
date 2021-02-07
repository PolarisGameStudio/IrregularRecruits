using System.Collections.Generic;

namespace GameLogic
{

    public class ResurrectAction : AbilityAction
    {
        public override ActionType ActionType => ActionType.Resurrect;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"resurrect {target} with {amount} health";
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> graveTargets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, graveTargets);
            graveTargets.ForEach(c => c.Resurrect(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 3.3f * targetValue * (1 + amount / 20f);
        }
    }
}