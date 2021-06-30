using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class StatMinusAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.StatMinus;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            if (firstPerson)
            {
                target = target.Replace("me", "I");

                return $"{target} lose {amount}/{amount} Attack and Health";
            }

            return $"{target} loses {amount}/{amount} Attack and Health";
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return potentialTargets.Any(c => c.Alive());
        }

        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.StatModifier(-ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -1f * targetValue * (1 + amount / 3f);
        }
    }
}