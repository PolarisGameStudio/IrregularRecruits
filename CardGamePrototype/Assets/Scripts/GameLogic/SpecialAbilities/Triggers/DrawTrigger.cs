using UnityEngine.Events;

namespace GameLogic
{
    public class DrawTrigger : AbilityTrigger
    {
        public override Ability.Verb TriggerType => Ability.Verb.Draw;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString}'s controller draws a card";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnDraw;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}