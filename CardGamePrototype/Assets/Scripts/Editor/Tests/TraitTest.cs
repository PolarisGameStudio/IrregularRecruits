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

        [SetUp]
        public void BattleManage()
        {
            BattleManager.Init();

            GameSettings.Instance.AiControlledPlayer = true;
        }

        [TearDown]
        public void Reset()
        {
            Event.ResetEvents();
        }


        private Card GenerateTestCreature(string traitName, int attack = 2, int health = 10)
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

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
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

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(rangedCharacter.CurrentHealth == rangedCharacter.MaxHealth);
            Assert.IsTrue(finished);
        }
        [Test]
        public void PlayersDeathlessNotRemovedAfterDeath()
        {
            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            Card defPlayer = GenerateTestCreature("Deathless");
            Card defEnm = GenerateTestCreature("Deathless");

            pDeck.AddCard(defPlayer);
            enmDeck.AddCard(defEnm);

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(pDeck.AllCreatures().Contains(defPlayer));
            Assert.IsFalse(enmDeck.AllCreatures().Contains(defEnm));
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


        //        test shapeshifter
        [Test]
        public void ShapeshifterIsAllTypes()
        {
            var shifter = GenerateTestCreature("Shapeshifter");

            var gobbo = new Race()
            {
                name = "Goblin"
            };
            var terran = new Race()
            {
                name = "Terran"
            };

            Assert.IsTrue(shifter.Shapeshifter());

            Assert.IsTrue(shifter.IsRace(gobbo));
            Assert.IsTrue(shifter.IsRace(terran));
        }

        //test lifedrain
        [Test]
        public void LifedrainDrainsTest()
        {
            const int Attack = 2;
            var vampire = GenerateTestCreature("Lifedrain", Attack, 10);

            const int enmattack = 3;
            var enm = GenerateTestCreature("", enmattack, 10);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(vampire);
            enmDeck.AddCard(enm);

            vampire.ChangeLocation( Deck.Zone.Battlefield);
            enm.ChangeLocation( Deck.Zone.Battlefield);


            Assert.IsTrue(vampire.Lifedrain());

            vampire.AttackCard(enm);

            Assert.AreEqual(enm.MaxHealth - Attack, enm.CurrentHealth);
            Assert.AreEqual(vampire.MaxHealth + Attack - enmattack, vampire.CurrentHealth);
        }

        //test lifedrain
        [Test]
        public void LifedrainDoesNotSafeFromLethalDamage()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var vampire = GenerateTestCreature("Lifedrain", Attack, enmattack);

            var enm = GenerateTestCreature("", enmattack, 10);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(vampire);
            enmDeck.AddCard(enm);

            vampire.ChangeLocation( Deck.Zone.Battlefield);
            enm.ChangeLocation( Deck.Zone.Battlefield);

            Assert.IsTrue(vampire.Lifedrain());

            vampire.AttackCard(enm);

            Assert.AreEqual(enm.MaxHealth - Attack, enm.CurrentHealth);
            Assert.IsFalse(vampire.Alive());
        }

        //test assassin
        [Test]
        public void AssassinTest()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var assassin = GenerateTestCreature("Assassin", Attack, 1000);

            var enm = GenerateTestCreature("", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack,4);
            var enm3 = GenerateTestCreature("", enmattack, 20);
            var lowHealthEnm = GenerateTestCreature("", enmattack, 3);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(assassin);
            enmDeck.AddCard(enm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);
            enmDeck.AddCard(lowHealthEnm);

            assassin.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);
            lowHealthEnm.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(assassin.Assassin());

            List<Card> attacked = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(finished);

            Assert.Contains(lowHealthEnm, attacked);
        }

        //test assassin
        [Test]
        public void AssassinIgnoresDefenderTest()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var assassin = GenerateTestCreature("Assassin", Attack, 1000);

            var defenderEnm = GenerateTestCreature("Defender", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack, 4);
            var enm3 = GenerateTestCreature("", enmattack, 20);
            var lowHealthEnm = GenerateTestCreature("", enmattack, 3);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(assassin);
            enmDeck.AddCard(defenderEnm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);
            enmDeck.AddCard(lowHealthEnm);

            assassin.ChangeLocation(Deck.Zone.Battlefield);
            defenderEnm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);
            lowHealthEnm.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(assassin.Assassin());
            Assert.IsTrue(defenderEnm.Defender());

            List<Card> attacked = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(finished);

            var firstTwoAttacked = attacked.GetRange(0, 2);

            Assert.Contains(lowHealthEnm, firstTwoAttacked);
            Assert.False(firstTwoAttacked.Contains(defenderEnm));
        }

        //test assassin
        [Test]
        public void AssassinRespectsEtherealTest()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var assassin = GenerateTestCreature("Assassin", Attack, 1000);

            var enm = GenerateTestCreature("", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack, 4);
            var enm3 = GenerateTestCreature("", enmattack, 20);
            var ethereal = GenerateTestCreature("Ethereal", enmattack, 3);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(assassin);
            enmDeck.AddCard(enm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);
            enmDeck.AddCard(ethereal);

            assassin.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);
            ethereal.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(assassin.Assassin());
            Assert.IsTrue(ethereal.Ethereal());

            List<Card> attacked = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener(d => finished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(finished);

            var firstTwoAttacked = attacked.GetRange(0, 2);

            Assert.Contains(enm2, firstTwoAttacked);
            Assert.False(firstTwoAttacked.Contains(ethereal));
        }

        //test ferocity
        [Test]
        public void CarnageMiddleAttackTest()
        {

            const int Attack = 2;

            const int enmattack = 2;

            var angryDude = GenerateTestCreature("Carnage", Attack, enmattack);

            var enm = GenerateTestCreature("", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack, 10);
            var enm3 = GenerateTestCreature("", enmattack, 10);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(angryDude);
            enmDeck.AddCard(enm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);

            angryDude.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);

            var target = enmDeck.CreaturesInZone(Deck.Zone.Battlefield)[1];

            Assert.IsTrue(angryDude.Carnage());

            angryDude.AttackCard(target);

            Assert.AreEqual(enm.MaxHealth - Attack, enm.CurrentHealth);
            Assert.AreEqual(enm2.MaxHealth - Attack, enm2.CurrentHealth);
            Assert.AreEqual(enm3.MaxHealth - Attack, enm3.CurrentHealth);
            Assert.AreEqual(angryDude.MaxHealth - enmattack, angryDude.CurrentHealth);
            
        }

        [Test]
        public void CarnageLastAttackTest()
        {

            const int Attack = 2;

            const int enmattack = 2;

            var angryDude = GenerateTestCreature("Carnage", Attack, enmattack);

            var enm = GenerateTestCreature("", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack, 10);
            var enm3 = GenerateTestCreature("", enmattack, 10);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(angryDude);
            enmDeck.AddCard(enm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);

            angryDude.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);

            var target = enmDeck.CreaturesInZone(Deck.Zone.Battlefield)[2];

            Assert.IsTrue(angryDude.Carnage());

            angryDude.AttackCard(target);

            Assert.AreEqual(enm.MaxHealth -Attack, enm.CurrentHealth);
            Assert.AreEqual(enm2.MaxHealth - Attack, enm2.CurrentHealth);
            Assert.AreEqual(enm3.MaxHealth , enm3.CurrentHealth);
            Assert.AreEqual(angryDude.MaxHealth - enmattack, angryDude.CurrentHealth);
            
        }

        [Test]
        public void CarnageFirstAttackTest()
        {

            const int Attack = 2;

            const int enmattack = 2;

            var angryDude = GenerateTestCreature("Carnage", Attack, enmattack);

            var enm = GenerateTestCreature("", enmattack, 10);
            var enm2 = GenerateTestCreature("", enmattack, 10);
            var enm3 = GenerateTestCreature("", enmattack, 10);

            var pDeck = GenerateTestDeck(0, true);
            var enmDeck = GenerateTestDeck(0, false);

            pDeck.AddCard(angryDude);
            enmDeck.AddCard(enm);
            enmDeck.AddCard(enm2);
            enmDeck.AddCard(enm3);

            angryDude.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);
            enm2.ChangeLocation(Deck.Zone.Battlefield);
            enm3.ChangeLocation(Deck.Zone.Battlefield);

            var target = enmDeck.CreaturesInZone(Deck.Zone.Battlefield)[0];

            Assert.IsTrue(angryDude.Carnage());

            angryDude.AttackCard(target);

            Assert.AreEqual(enm.MaxHealth , enm.CurrentHealth);
            Assert.AreEqual(enm2.MaxHealth - Attack, enm2.CurrentHealth);
            Assert.AreEqual(enm3.MaxHealth -Attack, enm3.CurrentHealth);
            Assert.AreEqual(angryDude.MaxHealth - enmattack, angryDude.CurrentHealth);
            
        }

        //test ward
        //test carnage
    }
}