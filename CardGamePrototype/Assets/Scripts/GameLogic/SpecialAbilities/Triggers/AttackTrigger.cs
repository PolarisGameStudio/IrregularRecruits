using UnityEngine.Events;

namespace GameLogic
{
    public class AttackTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.ATTACKS;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When {instigatorString} attack" + (firstPerson?"": "s");
        }

        internal override float AttackOrderModifier(Noun subjekt)
        {
            return subjekt.IsMeInBattle() ? 2f : 0f;
        }

        internal override float GetValue()
        {
            return 2f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.OnAttack.AddListener(handler);

            return () => Event.OnAttack.RemoveListener(handler);
        }
    }
}