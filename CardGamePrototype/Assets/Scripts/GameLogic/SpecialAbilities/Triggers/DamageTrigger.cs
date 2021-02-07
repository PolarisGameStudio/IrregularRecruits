using UnityEngine.Events;

namespace GameLogic
{
    public class DamageTrigger : AbilityTrigger
    {
        public override Verb TriggerType => Verb.IsDAMAGED;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is damaged";
        }

        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            void handler(Card a) => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnDamaged;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}