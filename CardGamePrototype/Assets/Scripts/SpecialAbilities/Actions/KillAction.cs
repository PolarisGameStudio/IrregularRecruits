using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.Kill;

    public override string Description(Ability.ActionType action, Card owner)
    {
        throw new System.NotImplementedException();
    }

    public override void ExecuteAction(Card _owner, Card triggerExecuter)
    {
        throw new System.NotImplementedException();
    }

    public override int GetValue()
    {
        return -10;
    }
}
