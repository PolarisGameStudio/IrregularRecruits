using UnityEngine.Events;

namespace GameLogic
{
    public class KillTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.KILLS;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When {instigatorString } kill" + (firstPerson ? "" : "s");
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.OnKill.AddListener(handler);

            return () => Event.OnKill.RemoveListener(handler);
        }
    }
}