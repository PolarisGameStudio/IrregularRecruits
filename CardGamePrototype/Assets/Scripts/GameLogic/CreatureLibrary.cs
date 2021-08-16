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
        public Creature[] SpawnableEnemies;
        public List<Creature> ShopCreatures;

        public Creature GetShopCreature(Race race)
        {
            var selectables = ShopCreatures.Where(c => c.Race == race && !c.IsSummon()).ToList();

            return TakeRandom(selectables);

        }

        //should never return already spawned unique
        public Creature TakeRandom(List<Creature> selectables,int belowCr = int.MaxValue)
        {
            if (selectables.Except(DeckGeneration.UniquesGenerated).Any())
                selectables = selectables.Except(DeckGeneration.UniquesGenerated).ToList();

            if (selectables.Any(c => c.CR <= belowCr))
                selectables = selectables.Where(c => c.CR <= belowCr).ToList();
            else
                return selectables.First(min => min.CR == selectables.Min(c => c.CR));

            Creature selected = selectables[Random.Range(0, selectables.Count())];

            return selected;
        }


        public Creature GetShopCreature()
        {
            return TakeRandom(ShopCreatures);
        }

    }

}