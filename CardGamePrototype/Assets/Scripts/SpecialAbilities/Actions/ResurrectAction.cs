using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.Resurrect;

    public override string Description(string target,int amount)
    {
        return $"resurrect {target} with {amount} health";
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> graveTargets)
    {
        FlowController.AddEvent(() =>
           Event.OnAbilityTrigger.Invoke(ability, owner, graveTargets));
        graveTargets.ForEach(c => c.Resurrect(ability.ResultingAction.Amount));
    }

    public override float GetValue(float targetValue,int amount)
    {
        return 3.3f * targetValue * (1 + amount / 20f);
    }
}
