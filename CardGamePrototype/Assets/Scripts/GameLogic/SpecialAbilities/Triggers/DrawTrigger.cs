using UnityEngine.Events;

namespace GameLogic
{
    public class DrawTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.Draw;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When " + (firstPerson ? "my " : instigatorString+"'s ")
                + "commander draws a card";
        }

        internal override float AttackOrderModifier(Noun subjekt)
        {
            return  0f;
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.CardEvent trigger = Event.OnDraw;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}