using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AssetManager 
{

#if UNITY_EDITOR
    private static float FavoredAbilityUseRate = 0.5f;

    [MenuItem("Content/Re-Generate unlocked creatures")]
    public static void GenerateCreatures()
    {
        List<Creature> creatureObjects = new List<Creature>();

        creatureObjects = GetAssetsOfType<Creature>();

        int attackMin = 1;
        int attackMax = 5;
        int defMin = 1;
        int defMax = 11;

        foreach (var c in creatureObjects.Where(c => !c.Locked))
        {
            c.Attack = Random.Range(attackMin, attackMax);
            c.Health = Random.Range(defMin, defMax);
        }
    }

    [MenuItem("Content/Divide abilities")]
    public static void DivideAbilitiesToCreature()
    {
        List<Creature> creatureObjects = new List<Creature>();

        creatureObjects = GetAssetsOfType<Creature>().Where(c=>c.Race ).ToList();


        List<Ability> abilities = new List<Ability>();

        abilities = GetAssetsOfType<Ability>();
               
        foreach (var a in abilities)
        {
            //if already distributed
            if (creatureObjects.Any(c => c.SpecialAbility == a))
                continue;

            List<Creature> selected = new List<Creature>();

            if (creatureObjects.Any(c => c.Race.FavoriteActions.Contains(a.ResultingAction.ActionType)))
                selected.AddRange(creatureObjects.Where(c => c.Race.FavoriteActions.Contains(a.ResultingAction.ActionType)));

            if (creatureObjects.Any(c => c.Race.FavoriteTriggers.Contains(a.TriggerCondition.TriggerAction)))
                selected.AddRange(creatureObjects.Where(c => c.Race.FavoriteTriggers.Contains(a.TriggerCondition.TriggerAction)));

            selected.RemoveAll(c => !AbilityFitsRarity(a, c.Rarity) || c.SpecialAbility);

            selected.OrderBy(c => Random.value);

            if (selected.Any())
            {
                selected.First().SpecialAbility = a;
                EditorUtility.SetDirty( selected.First());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        //foreach (var common in creatureObjects.Where(c => c.Rarity == Creature.RarityType.Common))
        //    common.SpecialAbility = null;

        Debug.Log("rare creatures without abilities: " + creatureObjects.Count(c => c.Rarity == Creature.RarityType.Rare & !c.SpecialAbility));
        Debug.Log("common creatures with abilities: " + creatureObjects.Count(c => c.Rarity == Creature.RarityType.Common && c.SpecialAbility));

    }
    
    [MenuItem("Content/Do stuff")]
    public static void DoStuff()
    {
        List<Creature> creatureObjects = new List<Creature>();
        creatureObjects = GetAssetsOfType<Creature>().Where(c=>c.Race ).ToList();

        List<Ability> abilities = new List<Ability>();
        abilities = GetAssetsOfType<Ability>();

        //creatureObjects.ForEach(c => c.Health *= 2);

        AssetDatabase.Refresh();
        abilities.ForEach(a => EditorUtility.SetDirty(a));
        creatureObjects.ForEach(a => EditorUtility.SetDirty(a));
        AssetDatabase.SaveAssets();

    }

    [MenuItem("Content/Re-Generate unlocked creature abilities")]
    public static void GenerateAbilities()
    {

        //List<Creature> creatureObjects = new List<Creature>();

        //creatureObjects = GetAssetsOfType<Creature>();

        //int amountMin = 1;
        //int amountMax = 4;

        //foreach (var c in creatureObjects.Where(c => !c.Locked && c.Rarity != Creature.RarityType.Common))
        //{
        //    //todo: rarity should be a factor
        //    Ability ability;
        //    do
        //    {
        //        ability = ScriptableObject.CreateInstance<Ability>();
        //        do
        //            ability.TriggerCondition = new Ability.Trigger((Noun)Random.Range(0, (int)Noun.CardInOwnersHand), (Random.value < FavoredAbilityUseRate) && c.Race.FavoriteTriggers.Any() ? c.Race.FavoriteTriggers[Random.Range(0, c.Race.FavoriteTriggers.Length)] : (Ability.Verb)Random.Range(0, (int)Ability.Verb.COUNT));
        //        while (ability.AnyTriggerInconsistencies());

        //        do
        //            ability.ResultingAction = new Ability.Action(
        //                (Random.value < FavoredAbilityUseRate) && c.Race.FavoriteActions.Any() ? c.Race.FavoriteActions[Random.Range(0, c.Race.FavoriteActions.Length)] : (Ability.ActionType)Random.Range(0, (int)Ability.ActionType.Summon),
        //                (Ability.Count)Random.Range(0, (int)Ability.Count.COUNT),
        //                (Noun)Random.Range(0, (int)Noun.COUNT),
        //                Random.Range(amountMin, amountMax)
        //                );
        //        while (ability.AnyActionInconsistencies());
        //    }
        //    while (!AbilityFitsRarity(ability, c.Rarity));

        //    ability.GetValue();

        //    string path = "Assets/Resources/Abilities/";

        //    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path +ability.TriggerCondition.Subject+ ability.TriggerCondition.TriggerAction + "-" + ability.ResultingAction.ActionType + ability.ResultingAction.Targets + Random.Range(0, 10000) + ".asset");

        //    AssetDatabase.CreateAsset(ability, assetPathAndName);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();

        //    c.SpecialAbility = ability;

        //}
    }

    private static bool AbilityFitsRarity(Ability ability, Creature.RarityType rarity)
    {
        var v = ability.GetValue();

        if (v < 0) return false;

        if (v < -8)
            return rarity == Creature.RarityType.Unique;
        else if (v < -2)
            return rarity == Creature.RarityType.Rare;
        else if (v <= 2)
            return rarity == Creature.RarityType.Rare;
        else if (v <= 6)
            return rarity == Creature.RarityType.Rare;
        else
            return rarity == Creature.RarityType.Unique;

    }
    public static List<T> GetAssetsOfType<T>() where T : UnityEngine.Object
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToList();

    }
#endif


}

