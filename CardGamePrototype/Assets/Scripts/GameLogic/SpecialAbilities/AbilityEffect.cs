using System.Collections.Generic;

namespace GameLogic
{
    public abstract class AbilityEffect
    {
        public AbilityEffect() { }

        public abstract EffectType ActionType { get; }
        public abstract string Description(string v, int amount,Creature summon);
        public abstract void ExecuteEffect(AbilityWithEffect ability, AbilityHolder _owner, List<Card> targets);
        public abstract bool CanExecute(AbilityWithEffect ability, AbilityHolder _owner, List<Card> targets);
        public abstract float GetValue(float targetvalue, int amount);


    }
}