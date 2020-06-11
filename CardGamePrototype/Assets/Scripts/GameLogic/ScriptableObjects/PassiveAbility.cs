using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Passive Ability")]
    public partial class PassiveAbility : Ability
    {
        [SerializeField]
        private float Value;
        public enum Verb
        {
            ATTACKS,
            IsATTACKED,
            KILLS,
            DIES,
            ETB,
            IsDAMAGED,
            IsHealed,
            Draw,
            Withdraw,
            RoundEnd,
            COUNT
        }


        public Trigger TriggerCondition;

        public override string ToString()
        {
            return base.ToString();
        }
        public override string Description(ICharacter owner)
        {
            return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.";
        }

        /// <returns>return the remove listener action</returns>
        public void SetupListeners(AbilityHolder _owner)
        {
            if (_owner.ListenersInitialized)
                Debug.LogWarning("Initializating listeners already done");

            _owner.ListenersInitialized = true;

            _owner.RemoveListenerAction = AbilityProcessor.GetTrigger(TriggerCondition.TriggerAction).SetupListener(_owner, TriggerCondition.Subjekt, ExecuteIfTrue);
        }

        public void RemoveListeners(AbilityHolder _owner)
        {
            if (!_owner.ListenersInitialized)
                Debug.LogWarning("removing non-initialized listeners ");

            _owner.RemoveListenerAction?.Invoke();

            _owner.ListenersInitialized = false;
            _owner.RemoveListenerAction = null;
        }

        public float GetValue()
        {
            Value = TriggerCondition.GetValue() * ResultingAction.GetValue();
            return Value;
        }

        private void ExecuteIfTrue(Card instigator, AbilityHolder abilityOwner, Noun subject)
        {
            if (subject.CorrectNoun(instigator, abilityOwner))
                ExecuteAction(abilityOwner, instigator);
        }

    }
}