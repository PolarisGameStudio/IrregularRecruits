using System.Collections.Generic;

public abstract class AbilityAction
{
    public AbilityAction() { }

    public abstract Ability.ActionType ActionType { get; }
    public abstract string Description(string v, int amount);
    public abstract void ExecuteAction(Ability ability, Card _owner, List<Card> targets);
    public abstract float GetValue(float targetvalue, int amount);


}
