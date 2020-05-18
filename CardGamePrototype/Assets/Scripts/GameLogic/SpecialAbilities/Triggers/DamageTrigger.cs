using UnityEngine.Events;

namespace GameLogic
{
    public class DamageTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.IsDAMAGED;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is damaged";
        }

        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = (a) => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnDamaged;

            trigger.AddListener(handler);

            return () => Event.OnDamaged.RemoveListener(handler);
        }
    }
}