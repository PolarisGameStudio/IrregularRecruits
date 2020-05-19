using UnityEngine.Events;

namespace GameLogic
{
    public class PlayTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.ETB;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString} enters the battlefield";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardEvent trigger = Event.OnEtb;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}