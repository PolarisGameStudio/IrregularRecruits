using UnityEngine.Events;

namespace GameLogic
{
    public class AttackTrigger : AbilityTrigger
    {
        public override Ability.Verb TriggerType => Ability.Verb.ATTACKS;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString} attacks";
        }


        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            UnityAction<Card> handler = a => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnAttack.AddListener(handler);

            return () => Event.OnAttack.RemoveListener(handler);
        }
    }
}