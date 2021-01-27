using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Combat Option")]
    public class CombatOption : MapOption
    {
        public override string Name { get; }
        [Header("Creatures always spawned")]
        public List<Creature> SpawnCreatures = new List<Creature>();
        [Header("Random Spawns")]
        public Race MainRace;
        public List<Race> AdditionalRaces = new List<Race>();
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
        public bool UniquesAllowed;

        public CombatOption(Race race, int combatRating, bool allowUniques)
        {
            cRValue = combatRating;
            MainRace = race;

            var friendChance = 0.3f;

            if (friendChance > Random.value && race.FriendRaces.Any()) 
                AdditionalRaces.Add(race.FriendRaces[Random.Range(0, race.FriendRaces.Length)]);

            UniquesAllowed = allowUniques;

            var hard = UniquesAllowed ? "Hard " : "";

            Name = $"{hard}{race.name} battle! ";//({combatRating})";
        }

        public CombatOption()
        {
        }

        public override float Difficulty()
        {
            return cRValue;
        }

        public override void ExecuteOption(MapNode owner)
        {
            var possibleRaces = new List<Race>() { MainRace };

            possibleRaces.AddRange(AdditionalRaces);

            var deck = DeckGeneration.GenerateDeck(CRValue, possibleRaces, SpawnCreatures,UniquesAllowed);

            MapController.Instance.StartCombat(deck);

        }
    }
}