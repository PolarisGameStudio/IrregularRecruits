using System.Collections.Generic;

namespace GameLogic
{
    public class GainEnergyAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.GainEnergy;

        public override string Description(string v, int amount, bool firstPerson, Creature summon)
        {
            return $"gain {amount} energy";
        }


        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            return true;
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder _owner, List<Card> targets)
        {

            Event.OnAbilityExecution.Invoke(ability, _owner, new List<Card>());
            _owner.InDeck.DeckController.ActionsLeft += ability.ResultingAction.Amount;
        }

        public override float GetValue(float targetvalue, int amount)
        {
            return 1f * amount;
        }
    }
}