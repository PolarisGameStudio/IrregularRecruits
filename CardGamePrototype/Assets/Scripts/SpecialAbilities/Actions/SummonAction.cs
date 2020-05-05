using System.Collections.Generic;

namespace GameLogic
{
    public class SummonAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Summon;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"summon a {summon.Attack}/{summon.Health} {summon.name}" ;
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {
            var summon = ability.ResultingAction.Summons;

            Event.OnAbilityTrigger.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Withdraw());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return targetValue;
        }
    }
}