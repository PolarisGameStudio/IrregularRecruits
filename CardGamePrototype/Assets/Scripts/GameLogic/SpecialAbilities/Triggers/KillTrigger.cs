using UnityEngine.Events;

namespace GameLogic
{
    public class KillTrigger : AbilityTrigger
    {
        public override PassiveAbility.Verb TriggerType => PassiveAbility.Verb.KILLS;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } kills a minion";
        }

        internal override float GetValue()
        {
            return 1f;
        }

        internal override UnityAction SetupListener(IAbilityHolder owner, Noun subjekt, UnityAction<Card, IAbilityHolder, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnKill.AddListener(handler);

            return () => Event.OnKill.RemoveListener(handler);
        }
    }
}