using GameLogic;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Combat Option")]
    public class CombatOptionObject : MapOptionObject
    {
        public override string Name { get; }
        [Header("Creatures always spawned")]
        public List<Creature> SpawnCreatures = new List<Creature>();
        [Header("Random Spawns")]
        public Race MainRace;
        public List<Race> AdditionalRaces = new List<Race>();
        public int MinCr = 10;
        public int MaxCr = 100;
        public int CRValue;
        public bool UniquesAllowed;

        public override MapOption InstantiateMapOption()
        {
            return new CombatOption(this);
        }
    }
}