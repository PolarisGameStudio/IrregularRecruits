using GameLogic;
using System.Collections.Generic;
using UnityEngine;


namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Combat Option")]
    class CombatOption : MapOption
    {
        [Header("Creatures always spawned")]
        public List<Creature> SpawnCreatures;
        [Header("Random Spawns")]
        public List<Race> PossibleRaces;
        public int CRValue;

        public override void ExecuteOption(MapLocation owner)
        {
            throw new System.NotImplementedException();
        }
    }
}