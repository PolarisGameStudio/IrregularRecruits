using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class DamageAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.DealDamage;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"Deal {amount} damage to {target}";
        }

        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);

            targets.ForEach(c => c.HealthChange(-ability.ResultingAction.Amount));
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            return targets.Any(c => c.Alive());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -1f * targetValue * (1 + amount / 20f);
        }
    }
}