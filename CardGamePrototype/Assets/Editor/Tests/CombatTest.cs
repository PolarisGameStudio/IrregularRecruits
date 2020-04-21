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
        }

        [UnityTest]
        public IEnumerator KillingUnitsEndsCombat()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator InitialDraws()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator DrawForTurn()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator LibraryDecreasesOnDraw()
        {
            yield return null;
        }

        [UnityTest]
        public IEnumerator NoPlayerActionsLeftStartsCombat()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator CombatResolvesAutomatically()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator EachUnitGetsToAttack()
        {
            yield return null;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator AbilityTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
