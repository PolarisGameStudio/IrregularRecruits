using GameLogic;
using NUnit.Framework;
using System;
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

            GameSettings.Instance.AiControlledPlayer = true;
        }

        private Card GenerateTestCreature(string traitName,int attack = 2,int health = 10)
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = traitName
            };

            var TestCreature = new Creature()
            {
                name = "Tester" + UnityEngine.Random.Range(0, 1000),
                Attack = attack,
                Health = health,
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

            var testDeck = new Deck(testDeckObject);

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
            var attackedInFirstRound = new List<Card>();

            Event.OnBeingAttacked.AddListener(c =>  attacked.Add(c));
            Event.OnCombatResolveFinished.AddListener(() => DoIfTrue(() => attackedInFirstRound.AddRange(attacked), attackedInFirstRound.Count == 0));

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(attackedInFirstRound.Contains(defPlayer));
            Assert.IsTrue(attackedInFirstRound.Contains(defEnm));
            Assert.IsFalse(attackedInFirstRound.Contains(nonDefPlayer));
            Assert.IsFalse(attackedInFirstRound.Contains(nonDefEnm));
        }

        private void DoIfTrue(Action p, bool v)
        {
            if (v) p.Invoke();
        }


        [Test]
        public void RangedDoesNotTakeDamageAttacking()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card defPlayer = GenerateTestCreature("Defender");
            Card rangedCharacter = GenerateTestCreature("Ranged");
            Card defEnm = GenerateTestCreature("Defender");

            pDeck.AddCard(defPlayer);
            pDeck.AddCard(rangedCharacter);

            enmDeck.AddCard(defEnm);

            var finished = false;

            Event.OnBattleFinished.AddListener(d=> finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(rangedCharacter.CurrentHealth == rangedCharacter.MaxHealth);
            Assert.IsTrue(finished);
        }
        [Test]
        public void DeathlessNotRemovedAfterDeath()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card defPlayer = GenerateTestCreature("Deathless");
            Card defEnm = GenerateTestCreature("Deathless");

            pDeck.AddCard(defPlayer);
            enmDeck.AddCard(defEnm);

            var finished = false;

            Event.OnBattleFinished.AddListener(d=> finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(pDeck.AllCreatures().Contains(defPlayer));
            Assert.IsTrue(enmDeck.AllCreatures().Contains(defEnm));
            Assert.IsTrue(finished);
        }
        [Test]
        public void EtherealIsNotAttackedWhileOtherNonEthereals()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card ethereal = GenerateTestCreature("Ethereal");
            Card nonEthereal = GenerateTestCreature("");
            Card nonEthereal2 = GenerateTestCreature("");
            Card enm = GenerateTestCreature("");

            pDeck.AddCard(ethereal);
            pDeck.AddCard(nonEthereal);
            pDeck.AddCard(nonEthereal2);

            enmDeck.AddCard(enm);

            List<Card> attacked = new List<Card>(); 
            List<Card> attackers = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
            Event.OnAttack.AddListener(c => attackers.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(ethereal.CurrentHealth == nonEthereal.MaxHealth);
            Assert.IsFalse(attacked.Contains(ethereal));
            Assert.IsFalse(attackers.Contains(ethereal));
            Assert.IsTrue(finished);

        }
        [Test]
        public void EtherealIsAttackedWhileOtherEtherealsOnly()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card ethereal = GenerateTestCreature("Ethereal");
            Card etherereal1 = GenerateTestCreature("Ethereal");
            Card ethereal2 = GenerateTestCreature("Ethereal");

            Card enm = GenerateTestCreature("");

            pDeck.AddCard(ethereal);
            pDeck.AddCard(etherereal1);
            pDeck.AddCard(ethereal2);

            enmDeck.AddCard(enm);

            List<Card> attacked = new List<Card>();
            List<Card> attackers = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
            Event.OnAttack.AddListener(c => attackers.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(finished);

            Assert.IsFalse(attacked.Contains(enm));
            Assert.IsTrue(attackers.Contains(enm));

            Assert.IsTrue(attacked.Contains(ethereal));
            Assert.IsFalse(attackers.Contains(ethereal));
            Assert.IsTrue(attacked.Contains(ethereal2));
            Assert.IsFalse(attackers.Contains(ethereal2));
            Assert.IsTrue(attacked.Contains(etherereal1));
            Assert.IsFalse(attackers.Contains(etherereal1));
        }
    }
}