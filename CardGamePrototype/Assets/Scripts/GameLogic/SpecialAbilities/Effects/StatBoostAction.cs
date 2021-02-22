using System.Collections.Generic;
namespace GameLogic
{
    public class StatBoostAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.StatPlus;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"{target} gain {amount}/{amount}";
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.StatModifier(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 1.5f * targetValue * (1 + amount / 1.5f);
        }
    }
}