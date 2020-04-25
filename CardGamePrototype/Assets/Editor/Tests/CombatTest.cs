using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class CombatTest
    {
        private BattleManager BattleManager;

        private Card GenerateTestCreature(Ability ability, Race race = null)
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = "TestTrait"
            };

            var TestCreature = new Creature()
            {
                name = "TesterOne",
                Attack = 2,
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

        private Deck GenerateTestDeck(int creatures, bool playerDeck)
        {
            var testDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            var testDeck = new Deck(testDeckObject, playerDeck);

            for (int i = 0; i < creatures; i++)
            {
                var c = GenerateTestCreature(null);

                testDeck.AddCard(c);
            }

            return testDeck;
        }

        [SetUp]
        public void BattleManagement()
        {
            Event.ResetListeners();
            BattleManager = new BattleManager();
        }

        [Test]
        public void CombatStartsResolving()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, true);

            var resolveStarted = false;

            Event.OnCombatResolveStart.AddListener(() => resolveStarted = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Event.OnTurnBegin.Invoke();

            Assert.IsTrue(resolveStarted);
        }
        [Test]
        public void CombatResolveFinishes()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, true);

            var resolveFinish = false;

            Event.OnCombatResolveFinished.AddListener(() => resolveFinish = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Event.OnTurnBegin.Invoke();

            Assert.IsTrue(resolveFinish);
        }
        [Test]
        public void BattleResolvesAutomatically()
        {
            var pDeck = GenerateTestDeck(2, true);
            var enmDeck = GenerateTestDeck(1, true);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener(() => battleFinished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Event.OnTurnBegin.Invoke();

            Assert.IsTrue(battleFinished);
        }
        [Test]
        public void BattleResolvesAutomaticallyGiant()
        {
            var pDeck = GenerateTestDeck(200, true);
            var enmDeck = GenerateTestDeck(1000, true);

            var battleFinished = false;

            Event.OnBattleFinished.AddListener(() => battleFinished = true);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);

            Event.OnTurnBegin.Invoke();

            Assert.IsTrue(battleFinished);
        }
        [UnityTest]
        public IEnumerator InitialDraws()
        {
            yield return null;
            Assert.IsTrue(false);
        }
        [UnityTest]
        public IEnumerator DrawForTurn()
        {
            yield return null;
            Assert.IsTrue(false);
        }
        [UnityTest]
        public IEnumerator EachUnitGetsToAttack()
        {
            yield return null;
            Assert.IsTrue(false);
        }

    }
}
