using UnityEngine.Events;

namespace GameLogic
{
    public class IsAttackedTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.IsATTACKED;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is attacked";
        }

        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnBeingAttacked;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}