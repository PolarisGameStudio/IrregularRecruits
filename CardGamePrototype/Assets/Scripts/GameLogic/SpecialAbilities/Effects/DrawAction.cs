using System.Collections.Generic;

namespace GameLogic
{

    public class DrawAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Draw;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"draw {amount} cards";
        }

        public override void ExecuteAction(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, owner, new List<Card>());
            owner.InDeck.Draw(ability.ResultingAction.Amount);
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 1f * amount;
        }
    }
}