using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    public class AbilityProcessor
    {
        private static Dictionary<EffectType, AbilityEffect> abilityEffects = new Dictionary<EffectType, AbilityEffect>();
        private static Dictionary<TriggerType, AbilityTrigger> triggers = new Dictionary<TriggerType, AbilityTrigger>();
        private static bool initialized;

        //takes the trigger type, the ability owner and the result action to allow doublers to double the effect
        public class AbilityTriggerEvent : UnityEvent<TriggerType,EffectType, AbilityHolder,UnityAction> { }
        public static AbilityTriggerEvent OnAbilityTriggered = new AbilityTriggerEvent();

        private static void Initialize()
        {
            /// ACTIONS
            /// //TODO: replace duplicate code with method with generics
            abilityEffects.Clear();

            var assembly = Assembly.GetAssembly(typeof(AbilityEffect));

            var allStates = assembly.GetTypes()
                .Where(t => typeof(AbilityEffect).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var state in allStates)
            {
                var action = Activator.CreateInstance(state) as AbilityEffect;

                if (abilityEffects.ContainsKey(action.ActionType)) Debug.LogError($"Multiple instances of action: {action.ActionType}");

                abilityEffects.Add(action.ActionType, action);
            }

            /// TRIGGERS
            triggers.Clear();

            var triggerAssembly = Assembly.GetAssembly(typeof(AbilityTrigger));

            var alltriggers = assembly.GetTypes()
                .Where(t => typeof(AbilityTrigger).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var state in alltriggers)
            {
                var trigger = Activator.CreateInstance(state) as AbilityTrigger;

                if (triggers.ContainsKey(trigger.TriggerType)) Debug.LogError($"Multiple instances of action: {trigger.TriggerType}");

                triggers.Add(trigger.TriggerType, trigger);
            }

            initialized = true;
        }

        /// //TODO: replace duplicate code with method with generics
        public static AbilityEffect GetAction(EffectType state)
        {
            if (!initialized) Initialize();

            if (!abilityEffects.ContainsKey(state))
            {
                Debug.Log($"No  avaiable for : {state}");
                return null;
            }
            return abilityEffects[state];
        }
        public static AbilityTrigger GetTrigger(TriggerType state)
        {
            if (!initialized) Initialize();

            if (!triggers.ContainsKey(state))
            {
                Debug.Log($"No  avaiable for : {state}");
                return null;
            }
            return triggers[state];
        }


    }
}