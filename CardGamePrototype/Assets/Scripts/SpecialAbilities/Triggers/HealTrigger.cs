using UnityEngine.Events;

public class HealTrigger : AbilityTrigger
{
    public override Ability.Verb TriggerType => Ability.Verb.IsHealed;

    internal override string Description(string instigatorString)
    {
        return $"When {instigatorString } is healed";
    }

    internal override float GetValue()
    {
        return 0.5f;
    }

    internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
    {
        Event.OnHealed.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
    }
}
