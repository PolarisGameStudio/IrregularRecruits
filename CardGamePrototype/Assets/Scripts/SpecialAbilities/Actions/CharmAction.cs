using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.Charm;

    public override string Description(string target, int amount)
    {
        return "take control of " + target;
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {
        FlowController.AddEvent(() =>
                Event.OnAbilityTrigger.Invoke(ability, owner, targets));
        FlowController.AddEvent(() =>
            targets.ForEach(c => c.Charm(owner.InDeck)));
    }

    public override float GetValue(float targetValue, int amount)
    {
        return -4f * targetValue;
    }
}
