using UnityEngine.Events;

namespace GameLogic
{
    public class AttackTrigger : AbilityTrigger
    {
        public override TriggerType TriggerType => TriggerType.ATTACKS;

        internal override string Description(string instigatorString, bool firstPerson)
        {
            return $"When {instigatorString} attack" + (firstPerson?"": "s");
        }


        internal override float GetValue()
        {
            return 3f;
        }

        internal override UnityAction SetupListener(AbilityHolder owner, Noun subjekt, UnityAction<Card, AbilityHolder, Noun> executeIfTrue)
        {
            void handler(Card a) => executeIfTrue.Invoke(a, owner, subjekt);

            Event.OnAttack.AddListener(handler);

            return () => Event.OnAttack.RemoveListener(handler);
        }
    }
}