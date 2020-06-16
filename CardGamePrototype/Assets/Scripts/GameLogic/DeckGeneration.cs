using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    public abstract class DeckGeneration
    {

        public Race[] SpawnableRaces;
        public Creature[] EnemyCreatures;

        public static Deck GenerateDeck(int CR, List<Race> races)
        {
            return new Deck(new DeckObject());
        }
        
        public static Deck GenerateDeck(int CR, List<Race> races, List<Creature> creatures)
        {
            return new Deck(new DeckObject());
        }

        protected Deck GenerateDeck(int CR, bool player = false, Creature testCreature = null, HeroObject testHero = null)
        {
            List<Creature> creatures;

            //Generate a deck full of the test creature
            if (testCreature && player)
            {
                creatures = new List<Creature>() { testCreature };
            }
            else
            {
                Race[] possibleRaces = player ? SpawnableRaces.Where(r => r.PlayerRace).ToArray() : SpawnableRaces;
                var race = possibleRaces[Random.Range(0, possibleRaces.Length)];

                creatures = EnemyCreatures.Where(c => c.Race == race).ToList();
            }

            var library = new List<Card>();


            var difficultyLeft = CR;

            if (!creatures.Any(c => c.CR <= difficultyLeft) & !(player && testCreature))
                throw new System.Exception("Creatures of type " + creatures.First().Race + " has no creature with CR below " + difficultyLeft);

            int v = (GameSettings.DeckSize(player));
            for (int i = 0; i < v; i++)
            {
                Creature selected;

                if (player && testCreature)
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

            if ( testHero)
            {
                GenerateHero(testHero, deck);
            }

            return deck;
        }

        private static void GenerateHero(HeroObject heroObject, Deck deck)
        {
            var hero = new Hero(heroObject);

            hero.InDeck = deck;

            deck.Hero = hero;
        }
    }

}