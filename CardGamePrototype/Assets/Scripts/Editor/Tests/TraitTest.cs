using GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

namespace Tests
{
    public class TraitTest : TestFixture
    {
        [Test]
        public void DefenderIsAttackedFirst()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            Card defPlayer = GenerateTestCreatureWithTrait("Defender");
            Card nonDefPlayer = GenerateTestCreatureWithTrait("");
            Card defEnm = GenerateTestCreatureWithTrait("Defender");
            Card nonDefEnm = GenerateTestCreatureWithTrait("");


            pDeck.AddCard(defPlayer);
            pDeck.AddCard(nonDefPlayer);

            enmDeck.AddCard(defEnm);
            enmDeck.AddCard(nonDefEnm);

            var attacked = new List<Card>();
            var attackedInFirstRound = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
            Event.OnCombatResolveFinished.AddListener(() => DoIfTrue(() => attackedInFirstRound.AddRange(attacked), attackedInFirstRound.Count == 0));

            new Battle(pDeck, enmDeck);

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
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            Card defPlayer = GenerateTestCreatureWithTrait("Defender");
            Card rangedCharacter = GenerateTestCreatureWithTrait("Ranged");
            Card defEnm = GenerateTestCreatureWithTrait("Defender");

            pDeck.AddCard(defPlayer);
            pDeck.AddCard(rangedCharacter);

            enmDeck.AddCard(defEnm);

            var finished = false;

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(rangedCharacter.CurrentHealth == rangedCharacter.MaxHealth);
            Assert.IsTrue(finished);
        }
        [Test]
        public void PlayersDeathlessNotRemovedAfterDeath()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            Card defPlayer = GenerateTestCreatureWithTrait("Deathless");
            Card defEnm = GenerateTestCreatureWithTrait("Deathless");

            pDeck.AddCard(defPlayer);
            enmDeck.AddCard(defEnm);

            var finished = false;

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(pDeck.AllCreatures().Contains(defPlayer));
            Assert.IsFalse(enmDeck.AllCreatures().Contains(defEnm));
            Assert.IsTrue(finished);
        }
        [Test]
        public void EtherealIsNotAttackedWhileOtherNonEthereals()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            Card ethereal = GenerateTestCreatureWithTrait("Ethereal");
            Card nonEthereal = GenerateTestCreatureWithTrait("");
            Card nonEthereal2 = GenerateTestCreatureWithTrait("");
            Card enm = GenerateTestCreatureWithTrait("");

            pDeck.AddCard(ethereal);
            pDeck.AddCard(nonEthereal);
            pDeck.AddCard(nonEthereal2);

            enmDeck.AddCard(enm);

            List<Card> attacked = new List<Card>();
            List<Card> attackers = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
            Event.OnAttack.AddListener(c => attackers.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(ethereal.CurrentHealth == nonEthereal.MaxHealth);
            Assert.IsFalse(attacked.Contains(ethereal));
            Assert.IsFalse(attackers.Contains(ethereal));
            Assert.IsTrue(finished);

        }
        [Test]
        public void EtherealIsAttackedWhileOtherEtherealsOnly()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            Card ethereal = GenerateTestCreatureWithTrait("Ethereal");
            Card etherereal1 = GenerateTestCreatureWithTrait("Ethereal");
            Card ethereal2 = GenerateTestCreatureWithTrait("Ethereal");

            Card enm = GenerateTestCreatureWithTrait("");

            pDeck.AddCard(ethereal);
            pDeck.AddCard(etherereal1);
            pDeck.AddCard(ethereal2);

            enmDeck.AddCard(enm);

            List<Card> attacked = new List<Card>();
            List<Card> attackers = new List<Card>();

            Event.OnBeingAttacked.AddListener(c => attacked.Add(c));
            Event.OnAttack.AddListener(c => attackers.Add(c));

            var finished = false;

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

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
            var shifter = GenerateTestCreatureWithTrait("Shapeshifter");

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
            var vampire = GenerateTestCreatureWithTrait("Lifedrain", Attack, 10);

            const int enmattack = 3;
            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            var vampire = GenerateTestCreatureWithTrait("Lifedrain", Attack, enmattack);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            var assassin = GenerateTestCreatureWithTrait("Assassin", Attack, 1000);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack,4);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 20);
            var lowHealthEnm = GenerateTestCreatureWithTrait("", enmattack, 3);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(finished);

            Assert.Contains(lowHealthEnm, attacked);
        }

        //test assassin
        [Test]
        public void AssassinIgnoresDefenderTest()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var assassin = GenerateTestCreatureWithTrait("Assassin", Attack, 1000);

            var defenderEnm = GenerateTestCreatureWithTrait("Defender", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack, 4);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 20);
            var lowHealthEnm = GenerateTestCreatureWithTrait("", enmattack, 3);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(finished);

            var firstTwoAttacked = attacked.GetRange(0, 2);

            Assert.Contains(lowHealthEnm, firstTwoAttacked);
            Assert.False(firstTwoAttacked.Contains(defenderEnm));
        }

        [Test]
        public void AssassinRespectsEtherealTest()
        {
            const int Attack = 2;

            const int enmattack = 2;

            var assassin = GenerateTestCreatureWithTrait("Assassin", Attack, 1000);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack, 4);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 20);
            var ethereal = GenerateTestCreatureWithTrait("Ethereal", enmattack, 3);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            Event.OnBattleFinished.AddListener((d,l) => finished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(finished);

            var firstTwoAttacked = attacked.GetRange(0, 2);

            Assert.Contains(enm2, firstTwoAttacked);
            Assert.False(firstTwoAttacked.Contains(ethereal));
        }

        //test Carnage
        [Test]
        public void CarnageMiddleAttackTest()
        {

            const int Attack = 2;

            const int enmattack = 2;

            var angryDude = GenerateTestCreatureWithTrait("Carnage", Attack, enmattack);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            var angryDude = GenerateTestCreatureWithTrait("Carnage", Attack, enmattack);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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

            var angryDude = GenerateTestCreatureWithTrait("Carnage", Attack, enmattack);

            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm2 = GenerateTestCreatureWithTrait("", enmattack, 10);
            var enm3 = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

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
        [Test]
        public void WardNotProtectsNonCombatDamage()
        {
            var warded = GenerateTestCreatureWithTrait("Ward");

            Assert.IsTrue(warded.Ward());
            Assert.IsTrue(warded.Warded);

            warded.HealthChange(-2);

            Assert.AreEqual(warded.MaxHealth -2, warded.CurrentHealth);

            Assert.IsTrue(warded.Warded);

        }
        
        //test ward
        [Test]
        public void WardProtectsOnlyFirstDamage()
        {
            const int Attack = 2;
            var warded = GenerateTestCreatureWithTrait("Ward", Attack, 10);

            const int enmattack = 3;
            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            pDeck.AddCard(warded);
            enmDeck.AddCard(enm);

            warded.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);


            Assert.IsTrue(warded.Ward());
            Assert.IsTrue(warded.Warded);

            warded.AttackCard(enm);

            Assert.AreEqual(enm.MaxHealth - Attack, enm.CurrentHealth);
            Assert.AreEqual(warded.MaxHealth, warded.CurrentHealth);

            Assert.IsFalse(warded.Warded);

            warded.AttackCard(enm);


            Assert.AreEqual(warded.MaxHealth - enmattack, warded.CurrentHealth);

        }

        [Test]
        public void WardProtectsCombatDamage()
        {
            const int Attack = 2;
            var warded = GenerateTestCreatureWithTrait("Ward", Attack, 10);

            const int enmattack = 3;
            var enm = GenerateTestCreatureWithTrait("", enmattack, 10);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            pDeck.AddCard(warded);
            enmDeck.AddCard(enm);

            warded.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);


            Assert.IsTrue(warded.Ward());
            Assert.IsTrue(warded.Warded);

            warded.AttackCard(enm);

            Assert.AreEqual(enm.MaxHealth - Attack, enm.CurrentHealth);
            Assert.AreEqual(warded.MaxHealth , warded.CurrentHealth);

            Assert.IsFalse(warded.Warded);

        }

        [Test]
        public void WardProtectsEachCombatRound()
        {
            var warded = GenerateTestCreatureWithTrait("Ward", 1, 10);

            const int enmattack = 2;
            var enm = GenerateTestCreatureWithTrait("", enmattack, 8);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            pDeck.AddCard(warded);
            enmDeck.AddCard(enm);

            warded.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(warded.Ward());
            Assert.IsTrue(warded.Warded);

            new Battle(pDeck, enmDeck);

            Assert.IsFalse(enm.Alive());
        }
        

        [Test]
        public void FerocityAttacksTwice() 
        {

            var ferious = GenerateTestCreatureWithTrait("Ferocity", 5, 10);
            var frind = GenerateTestCreatureWithTrait("", 5, 10);

            var enm = GenerateTestCreatureWithTrait("", 2, 19);

            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            pDeck.AddCard(ferious);
            pDeck.AddCard(frind);
            enmDeck.AddCard(enm);

            ferious.ChangeLocation(Deck.Zone.Battlefield);
            frind.ChangeLocation(Deck.Zone.Battlefield);
            enm.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(ferious.Ferocity());

            var attackers = new List<Card>();

            Event.OnAttack.AddListener(c => attackers.Add(c));

            new Battle(pDeck, enmDeck);

            Assert.AreEqual(2, attackers.Count(a => a == ferious));
            Assert.AreEqual(1, attackers.Count(a => a == frind));

            Assert.IsFalse(enm.Alive());

        }
    }
}