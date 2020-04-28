using UnityEngine.Events;

namespace GameLogic
{
    public class PlayTrigger : AbilityTrigger
    {
        public override Ability.Verb TriggerType => Ability.Verb.ETB;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString} enters the battlefield";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnPlay;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}