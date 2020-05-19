using UnityEngine.Events;

namespace GameLogic
{
    public class HealTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.IsHealed;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is healed";
        }

        internal override float GetValue()
        {
            return 0.5f;
        }

        internal override UnityAction SetupListener(IAbilityHolder owner, Noun subjekt, UnityAction<Card, IAbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card,int> handler = (a,i) => executeIfTrue.Invoke(a, owner, subjekt);

            Event.CardValueEvent trigger = Event.OnHealed;

            trigger.AddListener(handler);

            return () => trigger.RemoveListener(handler);
        }
    }
}