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

        public Creature GetCreature(Race race, bool includeUniques = true)
        {
            var selectables = AllCreatures.Where(c => c.Race == race && !c.IsSummon()).ToList();

            if (!includeUniques)
                selectables = selectables.Where(c => c.Rarity != Creature.RarityType.Unique).ToList();
            
            return TakeRandom(selectables);

        }

        public Creature TakeRandom(List<Creature> selectables,int belowCr = int.MaxValue)
        {
            if (selectables.Any(c => c.CR <= belowCr)) selectables = selectables.Where(c => c.CR <= belowCr).ToList();

            Creature selected = selectables[Random.Range(0, selectables.Count())];

            return selected;
        }


        public Creature GetShopCreature()
        {
            return TakeRandom(ShopCreatures);
        }

    }

}