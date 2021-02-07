using UnityEngine.Events;

namespace GameLogic
{
    public class KillTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.KILLS;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } kills a minion";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnKill.AddListener(handler);

            return () => Event.OnKill.RemoveListener(handler);
        }
    }
}