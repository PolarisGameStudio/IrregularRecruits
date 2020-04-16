using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBoostAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.StatPlus;

    public override string Description(string target, int amount)
    {
        return $"{target} get +{amount} Attack and Health";
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {
        FlowController.AddEvent(() =>
                Event.OnAbilityTrigger.Invoke(ability, owner, targets));
        FlowController.AddEvent(() =>
            targets.ForEach(c => c.StatModifier(ability.ResultingAction.Amount)));
    }

    public override float GetValue(float targetValue, int amount)
    {
        return 1.5f * targetValue * (1 + amount / 1.5f);
    }
}
