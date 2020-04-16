﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.Kill;

    public override string Description(string target, int amount)
    {
        return "kill " + target;
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {
        FlowController.AddEvent(() =>
                Event.OnAbilityTrigger.Invoke(ability, owner, targets));
        FlowController.AddEvent(() =>
            targets.ForEach(c => c.Die()));
    }

    public override float GetValue(float targetValue, int amount)
    {
        return -3f * targetValue;
    }
}
