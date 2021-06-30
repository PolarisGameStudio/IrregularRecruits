using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class CharmAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Charm;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            return "take control of " + target;
        }

        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, targets);

            targets.ForEach(c => c.Charm(owner.InDeck));
        }
        
        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            return targets.Any(c => c.InDeck != owner.InDeck);
        }

        public override float GetValue(float targetValue, int amount)
        {
            return - 5f * targetValue;
        }
    }
}