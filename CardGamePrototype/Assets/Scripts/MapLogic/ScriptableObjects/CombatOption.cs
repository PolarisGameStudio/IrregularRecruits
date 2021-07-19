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
        public int CRValue;
        public bool UniquesAllowed;

        public CombatOption(Race race, int combatRating, bool allowUniques)
        {
            CRValue = combatRating;
            MainRace = race;

            var friendChance = 0.3f;

            if (friendChance > Random.value && race.FriendRaces.Any()) 
                AdditionalRaces.Add(race.FriendRaces[Random.Range(0, race.FriendRaces.Length)]);

            UniquesAllowed = allowUniques;

            var hard = UniquesAllowed ? "Hard " : "";

            Name = $"{hard}{race.name} battle! ";//({combatRating})";

            //PopUpDescription = "Battle";
        }

        public CombatOption()
        {
        }

        public override float Difficulty()
        {
            return CRValue;
        }

        public override void ExecuteOption(MapNode owner)
        {
            List<Race> possibleRaces = null;

            if (MainRace)
            {
                possibleRaces = new List<Race>() { MainRace};

                possibleRaces.AddRange(AdditionalRaces);
            }

            var deck = DeckGeneration.GenerateDeck(CRValue, possibleRaces, SpawnCreatures,UniquesAllowed);

            new Battle(Battle.PlayerDeck, deck);

        }
    }
}