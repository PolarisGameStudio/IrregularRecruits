using UnityEngine.Events;

namespace GameLogic
{
    public class DiesTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.DIES;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When {instigatorString } die" + (firstPerson ? "" : "s");
        }

        internal override float AttackOrderModifier(Noun subjekt)
        {
            return subjekt.IsMeInBattle() ? 1f : 0f;
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.OnDeath.AddListener(handler);

            return () => Event.OnDeath.RemoveListener(handler);
        }
    }
}