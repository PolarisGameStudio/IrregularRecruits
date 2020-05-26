using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


namespace GameLogic
{


    [CreateAssetMenu]
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
        private UnityAction RemoveListenerAction;

        public override string ToString()
        {
            return base.ToString();
        }
        public override string Description(ICharacter owner)
        {
            return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.";
        }

        public void SetupListeners(AbilityHolder _owner)
        {
            RemoveListenerAction = AbilityProcessor.GetTrigger(TriggerCondition.TriggerAction).SetupListener(_owner, TriggerCondition.Subjekt, ExecuteIfTrue);
            //TODO: replace with CardEvent Reference
        }

        public void RemoveListeners()
        {
            RemoveListenerAction?.Invoke();
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