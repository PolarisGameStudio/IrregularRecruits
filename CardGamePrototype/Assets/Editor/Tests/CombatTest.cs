﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    

    public class CombatTest
    {
        private BattleManager BattleManager;

        private Card GenerateTestCreature(Ability ability, Race race = null,int attack = 2)
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = "TestTrait"
            };

            var TestCreature = new Creature()
            {
                name = "TesterOne",
                Attack = attack,
                Race = race,
                Health = 5,
                Traits = new List<Trait>()
                {
                    trait
                }
            };

            if (ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);

            return testCard;
        }

        private Deck GenerateTestDeck(int creatures, bool playerDeck, bool onlyAbilityDamage = false)
        {
            var testDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            var testDeck = new Deck(testDeckObject, playerDeck);

            for (int i = 0; i < creatures; i++)
            {
                var c = onlyAbilityDamage ? GenerateTestCreature(
                    new Ability()
                    {
                        ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 5, new Noun(Noun.CharacterTyp.Any)),
                        TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.RoundEnd),
                    },null,0
                    ): GenerateTestCreature(null);

                testDeck.AddCard(c);
            }

            return testDeck;
        }

        [OneTimeSetUp]
        public void BattleManagement()
        {
            BattleManager = new BattleManager();
        }


        [Test]
        public void CombatStartsResolving()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, false);

            var resolveStarted = false;

            Event.OnCombatResolveStart.AddListener(() => resolveStarted = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);


            Assert.IsTrue(resolveStarted);
        }
        [Test]
        public void CombatResolveFinishes()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, false);

            var resolveFinish = false;

            Event.OnCombatResolveFinished.AddListener(() => resolveFinish = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(resolveFinish);
        }
        [Test]
        public void BattleResolvesAutomatically()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, false);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener(() => battleFinished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);


            Assert.IsTrue(battleFinished);
        }
        [Test]
        public void BattleResolvesAutomaticallyBigDeck()
        {
            var pDeck = GenerateTestDeck(50, true);
            var enmDeck = GenerateTestDeck(50, false);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener(() => battleFinished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);


            Assert.IsTrue(battleFinished);
        }
        [Test]
        public void BattleResolvesWithOnlyAbilityDamage()
        {
            var pDeck = GenerateTestDeck(3, true,true);
            var enmDeck = GenerateTestDeck(3, false,true);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener(() => battleFinished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);


            Assert.IsTrue(battleFinished);
        }

        [Test]
        public void EachUnitGetsToAttack()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(3, false);

            var creatures = new List<Card>();

            creatures.AddRange(pDeck.AllCreatures());
            creatures.AddRange(enmDeck.AllCreatures());

            HashSet<Card> attackers = new HashSet<Card>();

            Event.OnAttack.AddListener(c=> attackers.Add(c));

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Assert.IsTrue(attackers.Count == creatures.Count);

            Assert.IsTrue(creatures.All(attackers.Contains));
        }

    }
}
