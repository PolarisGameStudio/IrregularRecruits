using UnityEngine.Events;

namespace GameLogic
{
    public class IsAttackedTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.IsATTACKED;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "When " + instigatorString
                + (firstPerson ? " am " : " is ") +
                "attacked";
        }

        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.CardEvent trigger = Event.OnBeingAttacked;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}