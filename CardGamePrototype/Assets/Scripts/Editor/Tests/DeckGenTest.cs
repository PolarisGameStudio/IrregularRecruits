using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests
{
    public class DeckGenTest : TestFixture
    {
        [SetUp]
        public void UniqueReset()
        {
            DeckGeneration.UniquesGenerated = new HashSet<Creature>();
        }

        [Test]
        public void DecksHaveCorrectCR()
        {

            for (int i = 0; i < 100; i++)
            {
                var value = Random.Range(40, 500);

                var deck = DeckGeneration.GenerateDeck(value);

                Assert.IsNotEmpty(deck.AllCreatures());

                Assert.LessOrEqual(deck.CR, value);

            }

        }

        [Test]
        public void DecksContainCorrectCreatures()
        {
            var creatures = new List<Creature>();

            TestCreature = CreateCreature();

            creatures.Add(TestCreature);

            var deck = DeckGeneration.GenerateDeck(50, null, creatures);

            Assert.IsTrue(deck.AllCreatures().Any(card => card.Creature == TestCreature));

        }

        [Test]
        public void UniquesAreNeverGeneratedTwice()
        {

            Assert.IsEmpty(DeckGeneration.UniquesGenerated);

            var creatures = CreatureLibrary.Instance.SpawnableEnemies.Count();

            for (int i = 0; i < 1000; i++)
            {
                DeckGeneration.GenerateDeck(400, null, null, true);

            }

            Assert.AreEqual(creatures, CreatureLibrary.Instance.SpawnableEnemies.Count());

            Assert.IsNotEmpty(DeckGeneration.UniquesGenerated);

            foreach (var unique in DeckGeneration.UniquesGenerated)
            {
                Assert.AreEqual(1, DeckGeneration.UniquesGenerated.Count(c => c == unique));
            }

        }
    }
}
