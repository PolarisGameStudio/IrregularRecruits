using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    public  class CombatOption : MapOption
    {
        public readonly List<Creature> SpawnCreatures = new List<Creature>();
        //should probably just have a creatures created?
        public readonly Race MainRace;
        public readonly List<Race> AdditionalRaces = new List<Race>();
        public readonly int CRValue;
        public readonly bool UniquesAllowed;

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

        public CombatOption( MapOptionObject optionObject)
            : base(optionObject)
        {

            CRValue = optionObject.CRValue;
            MainRace = optionObject.MainRace;

            var friendChance = 0.3f;

            if (friendChance > Random.value && MainRace.FriendRaces.Any())
                AdditionalRaces.Add(MainRace.FriendRaces[Random.Range(0, MainRace.FriendRaces.Length)]);

            UniquesAllowed = optionObject.UniquesAllowed;

            var hard = UniquesAllowed ? "Hard " : "";

            Name = $"{hard}{MainRace.name} battle! ";//({combatRating})";


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
                possibleRaces = new List<Race>() { MainRace };

                possibleRaces.AddRange(AdditionalRaces);
            }

            var deck = DeckGeneration.GenerateDeck(CRValue, possibleRaces, SpawnCreatures, UniquesAllowed);

            new Battle(Battle.PlayerDeck, deck);

        }
    }
}