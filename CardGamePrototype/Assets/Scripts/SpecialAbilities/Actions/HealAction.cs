using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.Heal;

    public override string Description(string target, int amount)
    {
        return $"Heal {target} for {amount}";
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {
        FlowController.AddEvent(() =>
            Event.OnAbilityTrigger.Invoke(ability, owner, targets));
        FlowController.AddEvent(() =>
            targets.ForEach(c => c.Heal(ability.ResultingAction.Amount)));
    }

    public override float GetValue(float targetValue,int amount)
    {
        return 0.5f * targetValue * (1 + amount / 20f);
    }
}
