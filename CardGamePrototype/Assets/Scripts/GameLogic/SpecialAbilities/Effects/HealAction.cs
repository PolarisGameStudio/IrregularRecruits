using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class HealAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Heal;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            return $"Heal {target} for {amount}";
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            return targets.Any(c => c.Damaged());
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
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