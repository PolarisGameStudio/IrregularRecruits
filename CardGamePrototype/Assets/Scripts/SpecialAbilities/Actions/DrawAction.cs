using System.Collections.Generic;

namespace GameLogic
{
    public class DrawAction : AbilityAction
    {
        public override Ability.ActionType ActionType => Ability.ActionType.Draw;

        public override string Description(string target, int amount)
        {
            return $"draw {amount} cards";
        }

        public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
        {

            Event.OnAbilityTrigger.Invoke(ability, owner, new List<Card>());
            owner.InDeck.Draw(ability.ResultingAction.Amount);
        }

        public override float GetValue(float targetValue, int amount)
        {
            return 1f * amount;
        }
    }
}