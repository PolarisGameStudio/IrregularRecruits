using UnityEngine.Events;

public class WithdrawTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.Withdraw;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString } is withdrawn";
    }

    internal override float GetValue()
    {
        return 1;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnWithdraw.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
