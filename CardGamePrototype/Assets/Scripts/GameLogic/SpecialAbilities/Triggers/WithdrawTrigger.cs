using UnityEngine.Events;

namespace GameLogic
{
    public class WithdrawTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.Withdraw;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is withdrawn";
        }

        internal override float GetValue()
        {
            return 1;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnWithdraw;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}