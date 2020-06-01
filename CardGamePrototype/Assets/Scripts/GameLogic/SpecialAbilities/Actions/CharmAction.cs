using System.Collections.Generic;

namespace GameLogic
{
    public class CharmAction : AbilityAction
    {
        public override PassiveAbility.ActionType ActionType => PassiveAbility.ActionType.Charm;

        public override string Description(string target, int amount, Creature summon)
        {
            return "take control of " + target;
        }

        public override void ExecuteAction(Ability ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);

            targets.ForEach(c => c.Charm(owner.InDeck));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 5f * targetValue;
        }
    }
}