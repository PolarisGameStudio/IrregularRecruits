using UnityEngine.Events;

namespace GameLogic
{
    public class HealTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.IsHealed;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is healed";
        }

        internal override float GetValue()
        {
            return 0.5f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card,int> handler = (a,i) => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardValueEvent trigger = Event.OnHealed;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}