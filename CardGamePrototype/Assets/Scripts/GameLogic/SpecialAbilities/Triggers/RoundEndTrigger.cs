using UnityEngine.Events;

namespace GameLogic
{
    public class RoundEndTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.RoundEnd;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return "at the end of each combat round";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            UnityAction handler = () => executeIfTrue.Invoke(null, owner, subjekt);

            var trigger = Event.OnCombatResolveFinished ;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}