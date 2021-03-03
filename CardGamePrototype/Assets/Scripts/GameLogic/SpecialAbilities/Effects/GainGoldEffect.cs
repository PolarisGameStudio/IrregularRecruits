using System.Collections.Generic;

namespace GameLogic
{
    public class GainGoldEffect : AbilityEffect
    {
        public override EffectType ActionType => EffectType.GainGold;

        public override string Description(string v, int amount, Creature summon)
        {
            return $"gain {amount} gold";
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            return true;
        }


        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder _owner, List<Card> targets)
        {
            Event.OnAbilityExecution.Invoke(ability, _owner, new List<Card>());
            
            if( BattleManager.Instance.PlayerDeck == _owner.InDeck)
                Event.OnPlayerGoldAdd.Invoke(ability.ResultingAction.Amount);
        }

        public override float GetValue(float targetvalue, int amount)
        {
            return 0.2f * amount;
        }
    }
}