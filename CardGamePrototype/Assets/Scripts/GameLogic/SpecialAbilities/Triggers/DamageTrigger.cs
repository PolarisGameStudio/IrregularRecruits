using UnityEngine.Events;

namespace GameLogic
{
    public class DamageTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.IsDAMAGED;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "When "+instigatorString
                + (firstPerson ? " am " : " is ") +
                "dealt damage";
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