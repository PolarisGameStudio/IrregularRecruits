using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameLogic
{
    public class AbilityProcessor
    {
        private static Dictionary<Ability.ActionType, AbilityAction> actionStates = new Dictionary<Ability.ActionType, AbilityAction>();
        private static Dictionary<PassiveAbility.Verb, AbilityTrigger> triggers = new Dictionary<PassiveAbility.Verb, AbilityTrigger>();
        private static bool initialized;

        private static void Initialize()
        {
            /// ACTIONS
            /// //TODO: replace duplicate code with method with generics
            actionStates.Clear();

            var assembly = Assembly.GetAssembly(typeof(AbilityAction));

            var allStates = assembly.GetTypes()
                .Where(t => typeof(AbilityAction).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var state in allStates)
            {
                var action = Activator.CreateInstance(state) as AbilityAction;

                if (actionStates.ContainsKey(action.ActionType)) Debug.LogError($"Multiple instances of action: {action.ActionType}");

                actionStates.Add(action.ActionType, action);
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
        public static AbilityAction GetAction(PassiveAbility.ActionType state)
        {
            if (!initialized) Initialize();

            if (!actionStates.ContainsKey(state))
            {
                Debug.Log($"No  avaiable for : {state}");
                return null;
            }
            return actionStates[state];
        }
        public static AbilityTrigger GetTrigger(PassiveAbility.Verb state)
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