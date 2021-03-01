using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    [CreateAssetMenu]
    public class CreatureLibrary : SingletonScriptableObject<CreatureLibrary>
    {
        public Race[] AllRaces;
        public Creature[] AllCreatures;
        public List<Creature> ShopCreatures;
        public HashSet<Creature> SpawnedUniques = new HashSet<Creature>();

        public void OnEnable()
        {
            Event.OnGameBegin.AddListener(Reset);
        }

        public Creature GetCreature(Race race, bool includeUniques = true)
        {
            var selectables = AllCreatures.Where(c => c.Race == race && !c.IsSummon()).ToList();

            if (!includeUniques)
                selectables = selectables.Where(c => c.Rarity != Creature.RarityType.Unique).ToList();
            
            return TakeRandom(selectables);

        }

        public Creature TakeRandom(List<Creature> selectables,int belowCr = int.MaxValue)
        {
            var exceptUniques = selectables.Except(SpawnedUniques).ToList();

            if (exceptUniques.Any()) selectables = exceptUniques;

            if (selectables.Any(c => c.CR <= belowCr)) selectables = selectables.Where(c => c.CR <= belowCr).ToList();

            Creature selected = selectables[Random.Range(0, selectables.Count())];

            if (selected.Rarity == Creature.RarityType.Unique)
                SpawnedUniques.Add(selected);
            
            return selected;
        }


        public Creature GetShopCreature()
        {
            return TakeRandom(ShopCreatures);
        }

        public void Reset()
        {
            Debug.Log("resetting uniques");

            SpawnedUniques.Clear();
        }
    }

}