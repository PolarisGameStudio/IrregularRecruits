using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class RallyAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Rally;

        public override string Description(string v, int amount, Creature summon)
        {

            return $"rally {v}";
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return potentialTargets.Any(c => c.Location == Deck.Zone.Library || c.Location != Deck.Zone.Hand);
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder _owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, _owner, targets);
            foreach(var target in targets)
            {
                target.Rally();
            }
        }

        public override float GetValue(float targetvalue, int amount)
        {
            return targetvalue * 1f;
        }
    }
}