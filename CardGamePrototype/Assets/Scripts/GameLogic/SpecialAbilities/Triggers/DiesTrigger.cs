using UnityEngine.Events;

namespace GameLogic
{
    public class DiesTrigger : AbilityTrigger
    {
        public override Verb TriggerType => Verb.DIES;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } dies";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnDeath.AddListener(handler);

            return () => Event.OnDeath.RemoveListener(handler);
        }
    }
}