using UnityEngine.Events;

namespace GameLogic
{
    public class HealTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.IsHealed;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "When " + instigatorString
                + (firstPerson ? " am " : " is ") +
                "healed";
        }

        internal override float GetValue()
        {
            return 0.5f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Deck.Zone, Noun> executeIfTrue)
        {
            UnityAction<Card,int,Deck.Zone> handler = (a,i,loc) => executeIfTrue.Invoke(a, owner,loc, subjekt);

            Event.CardValueEvent trigger = Event.OnHealed;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}