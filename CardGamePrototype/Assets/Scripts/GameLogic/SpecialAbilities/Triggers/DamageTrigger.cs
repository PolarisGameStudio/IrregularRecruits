using UnityEngine.Events;

namespace GameLogic
{
    public class DamageTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.IsDAMAGED;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "When "+instigatorString
                + (firstPerson ? " am " : " is ") +
                "dealt damage";
        }

        internal override float AttackOrderModifier(Noun subjekt)
        {
            return subjekt.IsMeInBattle() ? 1f : 0f;
        }

        internal override float GetValue()
        {
            return 2f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.CardEvent trigger = Event.OnDamaged;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}