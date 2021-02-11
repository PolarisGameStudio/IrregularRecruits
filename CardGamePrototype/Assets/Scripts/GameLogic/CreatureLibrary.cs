using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    [CreateAssetMenu]
    public class CreatureLibrary : SingletonScriptableObject<CreatureLibrary>
    {
        public Race[] AllRaces;
        public Race[] EnemyRaces;
        public Creature[] AllCreatures;

        public Creature GetCreature(Race race, bool includeUniques = true)
        {
            var selectables = AllCreatures.Where(c => c.Race == race).ToList();

            if (!includeUniques)
                selectables = selectables.Where(c => c.Rarity != Creature.RarityType.Unique).ToList();

            return selectables[Random.Range(0, selectables.Count())];

        }

        public Creature GetCreature(bool includeEnemies = false)
        {
            var races = AllRaces;
            if (!includeEnemies)
                races = races.Where(r => !EnemyRaces.Contains(r)).ToArray();

            return GetCreature(races[Random.Range(0, races.Length)]);

        }
    }

}