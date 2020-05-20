using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    public class GameControl
    {
        public  Deck PlayerDeck;
        public Deck EnemyDeck;
        public Creature TestCreature;
        public HeroObject TestHero;
        public Race[] SpawnableRaces;
        public Creature[] EnemyCreatures;
        public int CombatDifficultyIncrease = 3;
        public int CombatRarityIncrease = 1;

        public GameControl(Creature testCreature, Race[] races, Creature[] enmCreatures,HeroObject testHero = null)
        {
            TestCreature = testCreature;
            TestHero = testHero;
            SpawnableRaces = races; 
            EnemyCreatures = enmCreatures;
            
            if (EnemyCreatures == null|| EnemyCreatures.Length == 0 )
                EnemyCreatures= Resources.FindObjectsOfTypeAll<Creature>();
            if (SpawnableRaces == null || SpawnableRaces.Length == 0)
                SpawnableRaces = Resources.FindObjectsOfTypeAll<Race>();

            PlayerDeck = GenerateDeck(true);

        }

        public void NextCombat()
        {
            if (EnemyDeck == null || EnemyDeck.Alive() == 0)
                EnemyDeck = GenerateDeck();

            GameSettings.Instance.EnemyDeckSize += CombatDifficultyIncrease;
            GameSettings.Instance.MaxRareEnemiesPrCombat += CombatRarityIncrease;

            Event.OnCombatSetup.Invoke(PlayerDeck, EnemyDeck);
        }


        private Deck GenerateDeck(bool player = false)
        {
            var library = new List<Card>();

            List<Creature> creatures;

            //Generate a deck full of the test creature
            if (TestCreature && player)
            {
                creatures = new List<Creature>() { TestCreature };
            }
            else
            {
                Race[] possibleRaces = player ? SpawnableRaces.Where(r => r.PlayerRace).ToArray() : SpawnableRaces;
                var race = possibleRaces[Random.Range(0, possibleRaces.Length)];

                creatures = EnemyCreatures.Where(c => c.Race == race).ToList();
            }

            var rares = creatures.Where(c => c.Rarity == Creature.RarityType.Rare || c.Rarity == Creature.RarityType.Unique).ToList();
            var notRares = creatures.Except(rares).ToList();
            if (!notRares.Any())
                notRares = rares;

            for (int i = 0; i < (GameSettings.DeckSize(player)); i++)
            {
                if (rares.Any())
                {
                    for (; i < GameSettings.Instance.MaxRareEnemiesPrCombat; i++)
                    {
                        var rare = new Card(rares[Random.Range(0, rares.Count())]);

                        library.Add(rare);
                    }
                }


                var card = new Card(notRares[Random.Range(0, notRares.Count())]);

                library.Add(card);
            }

            var deck = new Deck(library);

            if(player && TestHero)
            {
                var hero = new Hero(TestHero);

                hero.InDeck = deck;

                deck.Hero = hero;
            }


            return deck;
        }
    }
}