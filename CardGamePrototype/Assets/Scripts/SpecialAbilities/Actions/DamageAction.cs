using System.Collections.Generic;

public class DamageAction : AbilityAction
{
    public override Ability.ActionType ActionType => Ability.ActionType.DealDamage;

    public override string Description(string target, int amount)
    {
        return $"Deal {amount} damage to {target}";
    }

    public override void ExecuteAction(Ability ability, Card owner, List<Card> targets)
    {

        Event.OnAbilityTrigger.Invoke(ability, owner, targets);

        targets.ForEach(c => c.Damage(ability.ResultingAction.Amount));
    }

    public override float GetValue(float targetValue, int amount)
    {
        return -1f * targetValue * (1 + amount / 20f);
    }
}
