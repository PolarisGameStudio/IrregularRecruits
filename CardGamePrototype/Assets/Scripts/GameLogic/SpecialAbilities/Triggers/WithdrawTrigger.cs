using UnityEngine.Events;

namespace GameLogic
{
    public class WithdrawTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.Withdraw;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "When " + instigatorString
                + (firstPerson ? " am " : " is ") +
                "withdrawn";
        }

        internal override float AttackOrderModifier(Noun subjekt)
        {
            return 0f;
        }

        internal override float GetValue()
        {
            return 1;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.CardEvent trigger = Event.OnWithdraw;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}