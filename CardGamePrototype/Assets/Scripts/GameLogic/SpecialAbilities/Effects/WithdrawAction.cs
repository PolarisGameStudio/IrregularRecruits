using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class WithdrawAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Withdraw;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            return "withdraw " + target;
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return potentialTargets.Any(c => c.Location == Deck.Zone.Battlefield);
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, owner, targets);
            targets.ForEach(c => c.Withdraw());
        }

        public override float GetValue(float targetValue, int amount)
        {
            return -2f * targetValue;
        }
    }
}