using System.Collections.Generic;

public class StatBoostAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.StatPlus;

    public override string Description(string target, int amount)
    {
        return $"{target} gain {amount} Attack and Health";
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {
        Event.OnAbilityTrigger.Invoke(ability, owner, targets);
        targets.ForEach(c => c.StatModifier(ability.ResultingAction.Amount));
    }

    public override float GetValue(float targetValue, int amount)
    {
        return 1.5f * targetValue * (1 + amount / 1.5f);
    }
}
