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
        public Race MainRace;
        public List<Race> AdditionalRaces;
        public int MinCr = 10;
        public int MaxCr = 100;
        private int cRValue;

        public int CRValue
        {
            get
            {
                if (cRValue <= 0)
                    cRValue = Random.Range(MinCr, MaxCr);

                return cRValue;
            }
            set => cRValue = value;
        }

        public override float Difficulty()
        {
            return cRValue;
        }

        public override void ExecuteOption(MapNode owner)
        {
            var possibleRaces = new List<Race>() { MainRace };

            possibleRaces.AddRange(AdditionalRaces);

            var deck = DeckGeneration.GenerateDeck(CRValue, possibleRaces, SpawnCreatures);

            MapController.Instance.StartCombat(deck);

        }
    }
}