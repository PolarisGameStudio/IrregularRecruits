using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

namespace Tests
{

    public class CombatTest : TestFixture
    { 

        [Test]
        public void CombatStartsResolving()
        {
            var pDeck = GenerateTestDeck(2);
            var enmDeck = GenerateTestDeck(1);

            var resolveStarted = false;

            Event.OnCombatResolveStart.AddListener(() => resolveStarted = true);
            new Battle(pDeck, enmDeck);

            Assert.IsTrue(resolveStarted);
        }

        [Test]
        public void EqualCreaturesBothDie()
        {
            var pDeck = GenerateTestDeck(1);
            var enmDeck = GenerateTestDeck(1);

            List<Card> deads = new List<Card>();

            Event.OnDeath.AddListener((c,l) => deads.Add(c));
            new Battle(pDeck, enmDeck);

            Assert.AreEqual(pDeck.AllCreatures().Count, 0);
            Assert.AreEqual(enmDeck.AllCreatures().Count, 0);
            Assert.AreEqual(deads.Count, 2);

        }
        [Test]
        public void CombatResolveFinishes()
        {
            var pDeck = GenerateTestDeck(2);
            var enmDeck = GenerateTestDeck(1);

            var resolveFinish = false;

            Event.OnCombatResolveFinished.AddListener(() => resolveFinish = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(resolveFinish);
        }
        [Test]
        public void BattleResolvesAutomatically()
        {
            var pDeck = GenerateTestDeck(2);
            var enmDeck = GenerateTestDeck(1);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener((d,l) => battleFinished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(battleFinished);
        }
        [Test]
        public void BattleRewardsCorrectXp()
        {
            var pDeck = GenerateTestDeck(6);
            var enmDeck = GenerateTestDeck(2);

            enmDeck.AddCard(new Card(new Creature()
            {
                Rarity = Creature.RarityType.Unique
            }));
            
            enmDeck.AddCard(new Card(new Creature()
            {
                Rarity = Creature.RarityType.Rare
            }));

            Hero hero = new Hero(new HeroObject());
            pDeck.Hero = hero;

            var xp = hero.Xp;

            var battleFinished = false;

            var xpValue = enmDeck.GetXpValue();

            Event.OnBattleFinished.AddListener((d,l) => battleFinished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(battleFinished);
            Assert.AreEqual(xp + xpValue, hero.Xp);
        }
        [Test]
        public void BattleResolvesAutomaticallyBigDeck()
        {
            var pDeck = GenerateTestDeck(50);
            var enmDeck = GenerateTestDeck(50);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener((d,l) => battleFinished = true);

            new Battle(pDeck, enmDeck);


            Assert.IsTrue(battleFinished);
        }
        [Test]
        public void DeadCreaturesRemovedFromDeck()
        {
            const int creatures = 5;

            var pDeck = GenerateTestDeck(creatures);
            var enmDeck = GenerateTestDeck(creatures);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener((d,l) => battleFinished = true);

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(battleFinished);
            Assert.IsTrue(pDeck.AllCreatures().Count < creatures);
            Assert.IsTrue(enmDeck.AllCreatures().Count < creatures);
            Assert.IsTrue(pDeck.AllCreatures().Count == 0 || enmDeck.AllCreatures().Count == 0);
        }
        [Test]
        public void BattleResolvesWithOnlyAbilityDamage()
        {
            var pDeck = GenerateTestDeck(3);
            var enmDeck = GenerateTestDeck(3);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener((d,l) => battleFinished = true);

            new Battle(pDeck, enmDeck);


            Assert.IsTrue(battleFinished);
        }

        [Test]
        public void EachUnitGetsToAttack()
        {
            var pDeck = GenerateTestDeck(2);
            var enmDeck = GenerateTestDeck(3);

            var creatures = new List<Card>();

            creatures.AddRange(pDeck.AllCreatures());
            creatures.AddRange(enmDeck.AllCreatures());

            HashSet<Card> attackers = new HashSet<Card>();

            Event.OnAttack.AddListener((c,l) => attackers.Add(c));

            new Battle(pDeck, enmDeck);

            Assert.IsTrue(attackers.Count == creatures.Count);

            Assert.IsTrue(creatures.All(attackers.Contains));
        }
        
        [Test]
        public void AttackingDamagesBoth()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            var creature1 = GenerateTestCreatureWithAbility(null);
            var creature2 = GenerateTestCreatureWithAbility(null);

            pDeck.AddCard(creature1);
            enmDeck.AddCard(creature2);

            creature1.ChangeLocation(Deck.Zone.Battlefield);
            creature2.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(!creature1.Damaged());
            Assert.IsTrue(!creature2.Damaged());

            creature1.AttackCard(creature2);

            Assert.IsTrue(creature1.Damaged());
            Assert.IsTrue(creature2.Damaged());

            Assert.AreEqual(creature1.MaxHealth - creature1.CurrentHealth, creature2.Attack);
            Assert.AreEqual(creature2.MaxHealth - creature2.CurrentHealth, creature1.Attack);
        }
        
        [Test]
        public void AttackingInHandDamagesOnlyTarget()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            var creature1 = GenerateTestCreatureWithAbility(null);
            var creature2 = GenerateTestCreatureWithAbility(null);

            pDeck.AddCard(creature1);
            enmDeck.AddCard(creature2);

            creature1.ChangeLocation(Deck.Zone.Battlefield);
            creature2.ChangeLocation(Deck.Zone.Hand);

            Assert.IsTrue(!creature1.Damaged());
            Assert.IsTrue(!creature2.Damaged());

            creature1.AttackCard(creature2);
            
            Assert.IsTrue(creature2.Damaged());
            Assert.IsFalse(creature1.Damaged());

            Assert.AreEqual(creature2.MaxHealth - creature2.CurrentHealth, creature1.Attack);
        }
        [Test]
        public void AttackingInDeckDamagesOnlyTarget()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            var creature1 = GenerateTestCreatureWithAbility(null);
            var creature2 = GenerateTestCreatureWithAbility(null);

            pDeck.AddCard(creature1);
            enmDeck.AddCard(creature2);

            creature1.ChangeLocation(Deck.Zone.Battlefield);
            creature2.ChangeLocation(Deck.Zone.Library);

            Assert.IsTrue(!creature1.Damaged());
            Assert.IsTrue(!creature2.Damaged());

            creature1.AttackCard(creature2);

            Assert.IsTrue(creature2.Damaged());
            Assert.IsFalse(creature1.Damaged());

            Assert.AreEqual(creature2.MaxHealth - creature2.CurrentHealth, creature1.Attack);
        }

        [Test]
        public void AttackingAndKillingDamagesBoth()
        {
            var pDeck = GenerateTestDeck(0);
            var enmDeck = GenerateTestDeck(0);

            var creature1 = GenerateTestCreatureWithAbility(null);
            var creature2 = GenerateTestCreatureWithAbility(null);

            pDeck.AddCard(creature1);
            enmDeck.AddCard(creature2);

            creature1.ChangeLocation(Deck.Zone.Battlefield);
            creature2.ChangeLocation(Deck.Zone.Battlefield);

            creature1.StatModifier(creature2.CurrentHealth);

            Assert.IsFalse(creature1.Damaged());
            Assert.IsTrue(creature2.Alive());

            creature1.AttackCard(creature2);

            Assert.IsFalse(creature2.Alive());
            Assert.IsTrue(creature1.Damaged());

            Assert.AreEqual(creature1.MaxHealth - creature1.CurrentHealth, creature2.Attack);
        }

    }
}
