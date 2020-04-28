using UnityEngine.Events;

namespace GameLogic
{
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

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnHealed;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}