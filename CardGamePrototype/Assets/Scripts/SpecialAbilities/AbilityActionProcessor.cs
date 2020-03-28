using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AbilityActionProcessor : MonoBehaviour
{

    private static Dictionary<Ability.ActionType, AbilityAction> actionStates = new Dictionary<Ability.ActionType, AbilityAction>();
    private static bool initialized;
    public static AbilityActionProcessor Instance;

    private void Start()
    {
        if (!Instance) Instance = this;
    }

    private static void Initialize()
    {
        actionStates.Clear();

        var assembly = Assembly.GetAssembly(typeof(AbilityAction));

        var allStates = assembly.GetTypes()
            .Where(t => typeof(AbilityAction).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var state in allStates)
        {
            var action = Activator.CreateInstance(state) as AbilityAction;

            if (actionStates.ContainsKey(action.ActionType)) Debug.LogError($"Multiple instances of action: {action.ActionType}");

            actionStates.Add(action.ActionType,action);
        }

        initialized = true;
    }

    public static void Method(Ability.ActionType state)
    {
        if (!initialized) Initialize();

        if (!actionStates.ContainsKey(state))
        {
            Debug.Log($"No  avaiable for : {state}");
        }
    }


}
