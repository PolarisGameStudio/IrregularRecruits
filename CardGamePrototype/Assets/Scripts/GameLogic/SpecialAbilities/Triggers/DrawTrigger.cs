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

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnDraw;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}