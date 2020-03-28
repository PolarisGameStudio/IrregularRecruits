using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class NounProcessor : MonoBehaviour
{
    private static Dictionary<Ability.Noun, Noun> Nouns = new Dictionary<Ability.Noun, Noun>();
    private static bool Initialized;
    public static NounProcessor Instance;

    private void Start()
    {
        if (!Instance) Instance = this;
    }

    private static void Initialize()
    {
        Nouns.Clear();

        var assembly = Assembly.GetAssembly(typeof(Noun));

        var allStates = assembly.GetTypes()
            .Where(t => typeof(Noun).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var state in allStates)
        {
            var stateInstance = Activator.CreateInstance(state) as Noun;

            if (Nouns.ContainsKey(stateInstance.NounType)) Debug.LogError($"Multiple instances of state action: {stateInstance.NounType}");

            Nouns.Add(stateInstance.NounType, stateInstance);
        }

        Initialized = true;
    }

    public static bool LegalNoun(Card executer, Card abilityOwner, Ability.Noun noun)//,Deck.Zone considerLocation = Deck.Zone.COUNT)
    {
        if (!Initialized) Initialize();

        if (!Nouns.ContainsKey(noun))
        {
            Debug.Log($"No state routine avaiable for : {noun}");
            return false;
        }

        if (!(executer.Location == Deck.Zone.Battlefield)) return false;

        return Nouns[noun].Appliable(executer, abilityOwner);

    }

}
