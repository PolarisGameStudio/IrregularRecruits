using GameLogic;
using Sound;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class AssetManager
{

#if UNITY_EDITOR
    private static float FavoredAbilityUseRate = 0.5f;
    private static List<Creature> creatureObjects = new List<Creature>();

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
        if (!creatureObjects.Any())
            creatureObjects = GetAssetsOfType<Creature>().Where(c => c.Race).ToList();


        List<PassiveAbility> abilities = new List<PassiveAbility>();

        abilities = GetAssetsOfType<PassiveAbility>();

        foreach (var a in abilities)
        {
            //if already distributed
            if (creatureObjects.Any(c => c.SpecialAbility == a))
                continue;

            MoveAbilityToOtherCreature(a);
        }

        //foreach (var common in creatureObjects.Where(c => c.Rarity == Creature.RarityType.Common))
        //    common.SpecialAbility = null;

        Debug.Log("rare creatures without abilities: " + creatureObjects.Count(c => c.Rarity == Creature.RarityType.Rare & !c.SpecialAbility));
        Debug.Log("common creatures with abilities: " + creatureObjects.Count(c => c.Rarity == Creature.RarityType.Common && c.SpecialAbility));

    }

    public static void MoveAbilityToOtherCreature(PassiveAbility a)
    {
        if (!creatureObjects.Any())
            creatureObjects = GetAssetsOfType<Creature>().Where(c => c.Race).ToList();

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
            EditorUtility.SetDirty(selected.First());
            Debug.Log("moved ability: " + a + "; to: " + selected.First());
        }
        else
            Debug.Log("no suitable creature found for ability: " + a);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Content/Do stuff")]
    public static void DoStuff()
    {
        List<Creature> creatures = new List<Creature>();
        creatures = GetAssetsOfType<Creature>();
        
        var races = new List<Race>();
        races = GetAssetsOfType<Race>();

        //Debug.Log("Checking abilities: " + creatures.Count);
        //var switchFrom = races.First(r => r.name == "Bird");
        //var switchto = races.First(r => r.name == "Animal");

        //foreach(var creature in creatures)
        //{
        //    if (creature.Race == switchFrom)
        //    {
        //        creature.Race = switchto;
        //        EditorUtility.SetDirty(creature);
        //    }
        //}



        //foreach (var creature in CreatureLibrary.Instance.AllCreatures)
        //{
        //    if (creature.Enabling.Contains(DeckStrategy.Withdraw) & !CreatureLibrary.Instance.ShopCreatures.Contains(creature))
        //        CreatureLibrary.Instance.ShopCreatures.Add(creature);
        //}

        //for (int i = 0; i < (int) DeckStrategy.COUNT; i++)
        //{
        //    var strat = (DeckStrategy) i;

        //    Debug.Log($"{strat} Enablers: {CreatureLibrary.Instance.ShopCreatures.Count(c => c.Enabling.Contains(strat))}");
        //    Debug.Log($"{strat} PayOffs: {CreatureLibrary.Instance.ShopCreatures.Count(c => c.Payoff.Contains(strat))}");


        //}

        AssetDatabase.Refresh();
        //creatures.ForEach(a => EditorUtility.SetDirty(a));
        AssetDatabase.SaveAssets();

    }

    [MenuItem("Content/Assign Deck Strategy To Creatures")]
    public static void StrategyAssignment()
    {
        List<Creature> creatures = GetAssetsOfType<Creature>();

        foreach (var creature in creatures)
        {

            //enablers
            DeckStrategy strat;

            if (Enum.TryParse(creature.Race?.name,true, out strat) &! creature.Enabling.Contains(strat))
            {
                creature.Enabling.Add(strat);
                EditorUtility.SetDirty(creature);
            }

            foreach (var trait in creature.Traits)
            {
                if (Enum.TryParse(trait.name, true, out strat) & !creature.Enabling.Contains(strat))
                {
                    creature.Enabling.Add(strat);
                    EditorUtility.SetDirty(creature);
                }


                if (trait.name == "Avantgarde" &! creature.Enabling.Contains(DeckStrategy.GoWide))
                {
                    creature.Enabling.Add(DeckStrategy.GoWide);
                    EditorUtility.SetDirty(creature);
                }
            }



            if (creature.SpecialAbility is PassiveAbility passive)
            {
                //ability enablers
                if (Enum.TryParse(passive.ResultingAction.ActionType.ToString(),true,out strat) & !creature.Enabling.Contains(strat))
                {
                    creature.Enabling.Add(strat);
                    EditorUtility.SetDirty(creature);

                }

                if (passive.ResultingAction.ActionType == EffectType.Summon & !creature.Enabling.Contains(DeckStrategy.GoWide))
                {
                    creature.Enabling.Add(DeckStrategy.GoWide);
                    EditorUtility.SetDirty(creature);

                }

                if (passive.ResultingAction.ActionType == EffectType.Rally && passive.ResultingAction.Target.Relationship != Noun.Allegiance.Enemy & !creature.Enabling.Contains(DeckStrategy.GoWide))
                {
                    creature.Enabling.Add(DeckStrategy.GoWide);
                    EditorUtility.SetDirty(creature);

                }


                //ability payoffs

                //boost others is a gowide payoff
                if (passive.ResultingAction.ActionType == EffectType.StatPlus && 
                    passive.ResultingAction.Target.Race != Noun.RaceType.Same &&  
                    passive.ResultingAction.TargetCount == Count.All && 
                    passive.ResultingAction.Target.Relationship != Noun.Allegiance.Enemy )
                {
                    AddPayoff(creature, DeckStrategy.GoWide);

                }

                if (passive.TriggerCondition.Subjekt.Trait )
                {
                    if (Enum.TryParse(passive.TriggerCondition.Subjekt.Trait.name, true, out strat) )
                    {

                        AddPayoff(creature, strat);
                    }
                }

                if(passive.TriggerCondition.Subjekt.Race == Noun.RaceType.Same)
                {
                    if (Enum.TryParse(creature.Race.name, true, out strat))
                    {
                        AddPayoff(creature, strat);
                    }
                }

                if (passive.TriggerCondition.TriggerAction == TriggerType.ATTACKS && passive.TriggerCondition.Subjekt.Trait && passive.TriggerCondition.Subjekt.Relationship != Noun.Allegiance.Enemy && passive.TriggerCondition.Subjekt.Character != Noun.CharacterTyp.This)
                {
                    DeckStrategy strategy = DeckStrategy.Ferocity;
                    AddPayoff(creature, strategy);
                }

                if (passive.TriggerCondition.TriggerAction == TriggerType.Withdraw)
                {
                    AddPayoff(creature, DeckStrategy.Withdraw);
                }
            }


        }


        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }

    private static void AddPayoff(Creature creature, DeckStrategy strategy)
    {
        if (!creature.Payoff.Contains(strategy))
        {
            creature.Payoff.Add(strategy);
            EditorUtility.SetDirty(creature);
        }
    }

    [MenuItem("Content/Recalculate CR")]
    public static void CalculateCR()
    {
        List<Creature> creatures = new List<Creature>();
        creatures = GetAssetsOfType<Creature>();

        //Debug.Log("Checking abilities: " + creatures.Count);

        foreach (var creature in creatures)
        {
            creature.CR = CalculateCR(creature);
        }

        AssetDatabase.Refresh();
        creatures.ForEach(a => EditorUtility.SetDirty(a));
        AssetDatabase.SaveAssets();

    }

    [MenuItem("Content/Re-Generate unlocked creature abilities")]
    public static void GenerateAbilities()
    {
        List<Creature> creatureObjects = new List<Creature>();

        creatureObjects = GetAssetsOfType<Creature>();

        foreach (var c in creatureObjects.Where(c => !c.Locked && c.Rarity != Creature.RarityType.Common))
        {
            GenerateAbilityForCreature(c);
        }
    }

    public static void GenerateAbilityForCreature(Creature c)
    {
        int amountMin = 1;
        int amountMax = 4;

        //todo: rarity should be a factor
        PassiveAbility ability;
        do
        {
            ability = ScriptableObject.CreateInstance<PassiveAbility>();

            var triggerCondition = new Noun(
                Random.value > 0.4f ? Noun.CharacterTyp.This : Noun.CharacterTyp.Any,
                Random.value > 0.2f ? Noun.Allegiance.Any : (Random.value > 0.5f ? Noun.Allegiance.Friend : Noun.Allegiance.Enemy),
                Random.value > 0.05f ? Noun.DamageType.Any : (Random.value > 0.5f ? Noun.DamageType.Damaged : Noun.DamageType.Undamaged),
                Random.value > 0.15f ? Noun.RaceType.Any : (Random.value > 0.4f ? Noun.RaceType.Same : Noun.RaceType.Different)
                );

            TriggerType triggerAction = (Random.value < FavoredAbilityUseRate) && c.Race.FavoriteTriggers.Any() ?
                c.Race.FavoriteTriggers[Random.Range(0, c.Race.FavoriteTriggers.Length)]
                : (TriggerType)Random.Range(0, (int)TriggerType.COUNT);

            ability.TriggerCondition = new Trigger(
                triggerCondition,
                triggerAction);

            ability.FixTriggerInconsistencies();

            var abilityTarget = new Noun(
                Random.value > 0.6f ? Noun.CharacterTyp.This : Random.value > 0.5f ? Noun.CharacterTyp.It : Noun.CharacterTyp.Any,
                Random.value > 0.2f ? Noun.Allegiance.Any : (Random.value > 0.5f ? Noun.Allegiance.Friend : Noun.Allegiance.Enemy),
                Random.value > 0.05f ? Noun.DamageType.Any : (Random.value > 0.5f ? Noun.DamageType.Damaged : Noun.DamageType.Undamaged),
                Random.value > 0.15f ? Noun.RaceType.Any : (Random.value > 0.4f ? Noun.RaceType.Same : Noun.RaceType.Different)
                );


            ability.ResultingAction = new AbilityEffectObject(
                (Random.value < FavoredAbilityUseRate) && c.Race.FavoriteActions.Any() ? c.Race.FavoriteActions[Random.Range(0, c.Race.FavoriteActions.Length)] : (EffectType)Random.Range(0, (int)EffectType.Summon),
                (Count)Random.Range(0, (int)Count.COUNT),
                Random.Range(amountMin, amountMax),
                abilityTarget
                );
        }
        while (!AbilityFitsRarity(ability, c.Rarity));

        ability.GetValue();

        string path = "Assets/Resources/Abilities/";

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + ability.TriggerCondition.TriggerAction + "-" + ability.ResultingAction.ActionType + Random.Range(0, 10000) + ".asset");

        AssetDatabase.CreateAsset(ability, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        c.SpecialAbility = ability;

    }
    private static Noun FixTriggerInconsistencies(Noun triggerCondition)
    {


        return triggerCondition;
    }


    public static int CalculateCR(Creature creature)
    {
        int cr = 0;

        cr += creature.Attack + creature.Health;

        if (creature.SpecialAbility)
            cr += (int)(creature.SpecialAbility.GetValue()*2);

        foreach (var item in creature.Traits)
        {
            cr += item.CR;
        }
        return cr;
    }

    private static bool AbilityFitsRarity(PassiveAbility ability, Creature.RarityType rarity)
    {
        return true;

        //var v = ability.GetValue();

        //if (v < 0) return false;

        //if (v < -8)
        //    return rarity == Creature.RarityType.Unique;
        //else if (v < -2)
        //    return rarity == Creature.RarityType.Rare;
        //else if (v <= 2)
        //    return rarity == Creature.RarityType.Common;
        //else if (v <= 6)
        //    return rarity == Creature.RarityType.Rare;
        //else
        //    return rarity == Creature.RarityType.Unique;

    }

    public static List<T> GetAssetsOfType<T>() where T : UnityEngine.ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToList();

    }
#endif


}

