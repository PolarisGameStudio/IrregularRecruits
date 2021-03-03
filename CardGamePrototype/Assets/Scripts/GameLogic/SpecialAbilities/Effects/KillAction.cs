using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class KillAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Kill;

        public override string Description(string target, int amount, Creature summon)
        {
            return "kill " + target;
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return potentialTargets.Any(c => c.Alive());
        }

        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
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