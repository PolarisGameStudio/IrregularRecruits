using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Passive Ability")]
    public class PassiveAbility : AbilityWithEffect
    {
        [SerializeField]
        private float Value;


        public static Dictionary<TriggerType, Deck.Zone> CorrectInstigatorLocations = new Dictionary<TriggerType, Deck.Zone>()
        {
                { TriggerType.ATTACKS,Deck.Zone.Battlefield },
                { TriggerType.DIES, Deck.Zone.Graveyard },
                { TriggerType.ETB, Deck.Zone.Battlefield},
                 { TriggerType.Withdraw,Deck.Zone.Battlefield},
        };


        public Trigger TriggerCondition;

        public override string ToString()
        {
            return base.ToString();
        }
        public override string Description(ICharacter owner)
        {
            return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.";
        }

        public override void SetupListeners(AbilityHolder _owner)
        {
            if (_owner.ListenersInitialized)
                Debug.LogWarning("Initializating listeners already done");

            _owner.ListenersInitialized = true;

            _owner.RemoveListenerAction = AbilityProcessor.GetTrigger(TriggerCondition.TriggerAction).SetupListener(_owner, TriggerCondition.Subjekt, ExecuteIfTrue);
        }

        public override float GetValue()
        {
            Value = TriggerCondition.GetValue() * ResultingAction.GetValue();

            if (Value < 0) Value *= -0.5f;

            return Value;
        }

        private void ExecuteIfTrue(Card instigator, AbilityHolder abilityOwner, Deck.Zone location, Noun subject)
        {
            if (subject.CorrectNoun(instigator, abilityOwner,location))
            {
                UnityAction action = ()=> ExecuteAction(abilityOwner, instigator);

                AbilityProcessor.OnAbilityTriggered.Invoke(TriggerCondition.TriggerAction,ResultingAction.ActionType, abilityOwner, action);

                action.Invoke();

            }
        }


        //TODO: move to Trigger instead
        public void FixTriggerInconsistencies()
        {
            //TODO: fix inconsistencies
            if (CorrectInstigatorLocations.ContainsKey(TriggerCondition.TriggerAction) && CorrectInstigatorLocations[TriggerCondition.TriggerAction] != TriggerCondition.Subjekt.Location)
            {
                Debug.Log($"Fixed ability trigger {name}. '{TriggerCondition.TriggerAction}' instigator location should be {CorrectInstigatorLocations[TriggerCondition.TriggerAction]}, not {TriggerCondition.Subjekt.Location} ");

                TriggerCondition.Subjekt.Location = CorrectInstigatorLocations[TriggerCondition.TriggerAction];
            }
        }

    }
}