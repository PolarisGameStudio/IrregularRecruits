using UnityEngine.Events;

namespace GameLogic
{
    public class DamageTrigger : AbilityTrigger
    {
        public override Ability.Verb TriggerType => Ability.Verb.IsDAMAGED;

        internal override string Description(string instigatorString)
        {
            return $"When {instigatorString } is damaged";
        }

        internal override float GetValue()
        {
            return 3f;
        }

        internal override void SetupListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            Event.OnDamaged.AddListener(a => executeIfTrue.Invoke(a, owner, subjekt));
        }
        internal override void RemoveListener(Card owner, Noun subjekt, UnityAction<Card, Card, Noun> executeIfTrue)
        {
            Event.OnDamaged.RemoveListener(a => executeIfTrue.Invoke(a, owner, subjekt));
        }
    }
}