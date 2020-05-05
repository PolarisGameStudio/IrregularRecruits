using System.Collections.Generic;

namespace GameLogic
{
    public class CharmAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Charm;

        public override string Description(string target, int amount, Creature summon)
        {
            return "take control of " + target;
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {

            Event.OnAbilityTrigger.Invoke(ability, owner, targets);

            targets.ForEach(c => c.Charm(owner.InDeck));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -4f * targetValue;
        }
    }
}