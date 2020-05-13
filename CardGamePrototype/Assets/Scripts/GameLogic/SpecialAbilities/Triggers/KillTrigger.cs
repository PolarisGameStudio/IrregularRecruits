using UnityEngine.Events;

namespace GameLogic
{
    public class KillTrigger : AbilityTrigger
    {
        public override Ability.Verb TriggerType => Ability.Verb.KILLS;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } kills a minion";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnKill.AddListener(handler);

            return () => Event.OnKill.RemoveListener(handler);
        }
    }
}