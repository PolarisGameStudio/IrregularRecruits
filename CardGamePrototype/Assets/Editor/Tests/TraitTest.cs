using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;

namespace Tests
{
    public class TraitTest
    {

        [OneTimeSetUp]
        public void BattleManage()
        {
            var neededForBM = BattleManager.Instance;
        }

        private Card GenerateTestCreature(string traitName)
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = traitName
            };

            var TestCreature = new Creature()
            {
                name = "Tester" + Random.Range(0, 1000),
                Attack = 2,
                Health = 10,
                Traits = string.IsNullOrEmpty(traitName) ? new List<Trait>() : new List<Trait>()
                {
                    trait
                }
            };

            var testCard = new Card(TestCreature);

            return testCard;
        }

        private Deck GenerateTestDeck(int creatures, bool playerDeck)
        {
            var testDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            var testDeck = new Deck(testDeckObject, playerDeck);

            for (int i = 0; i < creatures; i++)
            {
                var c = GenerateTestCreature("");

                testDeck.AddCard(c);
            }


            return testDeck;
        }

        [Test]
        public void DefenderIsAttackedFirst()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card defPlayer = GenerateTestCreature("Defender");
            Card nonDefPlayer = GenerateTestCreature("");            
            Card defEnm = GenerateTestCreature("Defender");
            Card nonDefEnm = GenerateTestCreature("");


            pDeck.AddCard(defPlayer);
            pDeck.AddCard(nonDefPlayer);
            
            enmDeck.AddCard(defEnm);
            enmDeck.AddCard(nonDefEnm);

            var attacked = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(attacked.Contains(defPlayer));
            Assert.IsTrue(attacked.Contains(defEnm));
            Assert.IsFalse(attacked.Contains(nonDefPlayer));
            Assert.IsFalse(attacked.Contains(nonDefEnm));
        }
        [Test]
        public void RangedDoesNotTakeDamageAttacking()
        {
            var pDeck = GenerateTestDeck(5, true);
            var enmDeck = GenerateTestDeck(3, false);

            
            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(false);
        }
        [Test]
        public void EtherealIsNotAtaackedwwhileOthers()
        {
            var pDeck = GenerateTestDeck(5, true);
            var enmDeck = GenerateTestDeck(3, false);

            
            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(false);
        }
        [Test]
        public void EtherealIsAtaackedwWhileOtherEthereals()
        {
            var pDeck = GenerateTestDeck(5, true);
            var enmDeck = GenerateTestDeck(3, false);

            
            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(false);
        }
    }
}