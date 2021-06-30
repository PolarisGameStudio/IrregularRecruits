using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{

    public class ResurrectAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Resurrect;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            //target = target.Replace(", that is dead", "");

            return $"resurrect {target} with {amount} health";
        }
        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return potentialTargets.Any(c => !c.Alive());
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> graveTargets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, graveTargets);
            graveTargets.ForEach(c => c.Resurrect(ability.ResultingAction.Amount));
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 3.3f * targetValue * (1 + amount / 20f);
        }
    }
}