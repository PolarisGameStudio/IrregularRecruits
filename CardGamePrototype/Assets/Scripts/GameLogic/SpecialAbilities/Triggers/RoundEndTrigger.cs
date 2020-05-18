using UnityEngine.Events;

namespace GameLogic
{
    public class RoundEndTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.RoundEnd;

        internal override string Description(string instigatorString)
        {
            return "at the end of each combat round";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction handler = () => executeIfTrue.Invoke(null, owner, subjekt);

            var trigger = Event.OnCombatResolveFinished ;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}