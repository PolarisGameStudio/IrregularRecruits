using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CombatTest
    {
        private string[] Creatures;
        private Deck PlayerDeck;
        private Card PlayerCard;
        private Card EnemyCard;

        [OneTimeSetUp]
        public void AbilitySetup()
        {
            Creatures = AssetDatabase.FindAssets("t:" + typeof(Creature).Name);
        }

        [UnityTest]
        public IEnumerator PlayerActionUsed()
        {
            yield return null;

            Assert.IsTrue(false);
        }

        [UnityTest]
        public IEnumerator KillingUnitsEndsCombat()
        {
            yield return null;
            Assert.IsTrue(false);
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
        public IEnumerator LibraryDecreasesOnDraw()
        {
            yield return null;
            Assert.IsTrue(false);
        }

        [UnityTest]
        public IEnumerator NoPlayerActionsLeftStartsCombat()
        {
            yield return null;
            Assert.IsTrue(false);
        }
        [UnityTest]
        public IEnumerator CombatResolvesAutomatically()
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
