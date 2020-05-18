using UnityEngine.Events;

namespace GameLogic
{
    public class DiesTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.DIES;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } dies";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnDeath.AddListener(handler);

            return () => Event.OnDeath.RemoveListener(handler);
        }
    }
}