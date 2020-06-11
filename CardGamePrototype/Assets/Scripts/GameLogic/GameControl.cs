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
        public int CombatDifficultyIncrease = 50;
        public int CurrentCombatDifficulty;
        public int MaxEnemyDeckSize = 10;

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

            if(TestCreature)
                PlayerDeck = GenerateDeck(true);

        }

        public void NextCombat()
        {
            CurrentCombatDifficulty += CombatDifficultyIncrease;

            //if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = GenerateDeck();

            if(GameSettings.Instance.EnemyDeckSize < MaxEnemyDeckSize)
                GameSettings.Instance.EnemyDeckSize ++;

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
            
            var difficultyLeft = CurrentCombatDifficulty;

            if (!creatures.Any(c => c.CR <= difficultyLeft) &! (player&&TestCreature))
                throw new System.Exception("Creatures of type " + creatures.First().Race + " has no creature with CR below " + difficultyLeft);

            int v = (GameSettings.DeckSize(player));
            for (int i = 0; i < v; i++)
            {
                Creature selected;

                if(player && TestCreature)
                {
                    selected = creatures[Random.Range(0, creatures.Count())];
                }
                else
                {
                    if (!creatures.Any(c => c.CR <= difficultyLeft) || player)
                        break;

                    var below = creatures.Where(c => c.CR <= difficultyLeft).ToList();

                    selected = below.OrderByDescending(b => b.CR * Random.value).First();

                }

                var card = new Card(selected);

                difficultyLeft -= selected.CR;

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