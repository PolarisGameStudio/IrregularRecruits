using System.Collections.Generic;

namespace GameLogic
{
    public abstract class AbilityAction
    {
        public AbilityAction() { }

        public abstract PassiveAbility.ActionType ActionType { get; }
        public abstract string Description(string v, int amount,Creature summon);
        public abstract void ExecuteAction(Ability ability, IAbilityHolder _owner, List<Card> targets);
        public abstract float GetValue(float targetvalue, int amount);


    }
}