using UnityEngine.Events;

namespace GameLogic
{
    public class PlayTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.ETB;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When {instigatorString} enter" + (firstPerson ? "" : "s") 
                +" the battlefield";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,Deck.Zone> handler = (a,loc) => executeIfTrue.Invoke(a, owner, loc, subjekt);

            Event.CardEvent trigger = Event.OnEtb;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}