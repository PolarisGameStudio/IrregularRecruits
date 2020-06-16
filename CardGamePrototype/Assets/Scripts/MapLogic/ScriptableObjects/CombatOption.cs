using GameLogic;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Combat Option")]
    public class CombatOption : MapOption
    {
        [Header("Creatures always spawned")]
        public List<Creature> SpawnCreatures;
        [Header("Random Spawns")]
        public List<Race> PossibleRaces;
        public int CRValue;

        public override void ExecuteOption(MapLocation owner)
        {
            var deck =  DeckGeneration.GenerateDeck(CRValue, PossibleRaces, SpawnCreatures);

            MapController.Instance.StartCombat(deck);

        }
    }
}