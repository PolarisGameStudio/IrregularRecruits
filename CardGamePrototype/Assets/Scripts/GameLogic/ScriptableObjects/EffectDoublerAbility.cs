using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Effect Doubler Ability")]
    public class EffectDoublerAbility : SpecialAbility
    {
        [Header("While active, this will double any effect triggered by this type")]
        public EffectType Effect;
        public string AbilityDescription;

        public override string Description(ICharacter owner)
        {
            if (string.IsNullOrEmpty(AbilityDescription))
                return $"{Effect} abilities happen an additional time";

            return AbilityDescription;
        }

        public override void SetupListeners(AbilityHolder _owner)
        {
            if (_owner.ListenersInitialized)
                Debug.LogWarning("Initializating listeners already done");

            _owner.ListenersInitialized = true;

            UnityAction<TriggerType, EffectType, AbilityHolder, UnityAction> checkTriggerAction = (t, e, c, a) => TriggerCheck(e, c, a, _owner);
            Event.OnAbilityTriggered.AddListener(checkTriggerAction);

            _owner.RemoveListenerAction = () => Event.OnAbilityTriggered.RemoveListener(checkTriggerAction);
        }

        //should only detect the team
        private void TriggerCheck(EffectType effect, AbilityHolder card, UnityAction a, AbilityHolder owner)
        {
            //TODO: could let allegiance be a paramter, so some doublers could count both enemy and friendly effects
            if (effect == Effect && owner.IsActive() && owner.InDeck == card.InDeck)
            {
                Event.OnAbilityExecution.Invoke(this, owner, new List<Card>());
                a.Invoke();
                
            }
        }

        public override float GetValue()
        {
            return 15f;
        }
    }
}