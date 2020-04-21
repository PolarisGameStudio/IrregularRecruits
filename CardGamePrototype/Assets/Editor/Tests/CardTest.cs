using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardTest : MonoBehaviour
    {
        private Card TestCard;
        private Creature TestCreature;
        private Deck TestDeck;
        private DeckObject TestDeckObject;

        [OneTimeSetUp]
        public void CardSetup()
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = "TestTrait"
            };

            TestCreature = new Creature()
            {
                name = "Testeron",
                Attack = 2,
                Health = 3,
                Traits = new List<Trait>()
                {
                    trait
                }
            };

            TestCard = new Card(TestCreature);

            TestDeckObject = new DeckObject();
            TestDeck = new Deck(TestDeckObject, true);
            TestDeck.AddCard(TestCard);

        }

        [Test]
        public void CardHasCorrectCreatureStats()
        {
            Assert.IsTrue(TestCreature.Attack == TestCard.Attack );
            Assert.IsTrue(TestCreature.Health == TestCard.CurrentHealth );
            Assert.IsTrue(TestCreature.Health == TestCard.MaxHealth );
        }

        [Test]
        public void CardHasCorrectCreatureTraits()
        {
            Assert.IsTrue(TestCard.GetTraits() == TestCreature.Traits);
        }

        [Test]
        public void CardHasCorrectCreatureAbility()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void IsInCorrectDeck()
        {
            Assert.IsTrue(TestCard.InDeck == TestDeck);

        }

        [UnityTest]
        public IEnumerator ChangesLocation()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            yield return null;

            Assert.IsTrue(TestCard.Location == Deck.Zone.Battlefield);

            TestCard.ChangeLocation(Deck.Zone.Library);

            yield return null;

            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }



        [UnityTest]
        public IEnumerator ClickPlaysWhenActionAndOnHand()
        {

            yield return null;
        }
        [UnityTest]
        public IEnumerator ClickPlaysNotWhenNotInHand()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator ClickPlaysNotWhenNoAction()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator ClickWithdrawsWhenActionAndOnBattlefield()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator ClickDoesNotWithdrawsWhenNoAction()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator ClickWithdrawsNotWhenNotOnBattlefield()
        {
            yield return null;
        }
        [UnityTest]
        public IEnumerator HealHeals()
        {
            yield return null;
        }
        [UnityTest]
        public void HealTriggersOnHeal()
        {

        }
        [UnityTest]
        public void DamageDamages()
        {

        }
        [UnityTest]
        public void DamageTriggersOnDamage()
        {

        }
        [UnityTest]
        public void StatboostBoosts()
        {

        }
        [UnityTest]
        public void StatboostDoesNotTriggerHeal()
        {

        }
        [UnityTest]
        public void StatMinusMinusses()
        {

        }
        [UnityTest]
        public void StatMinusDoesNotTriggerDamage()
        {

        }

        [UnityTest]
        public void FullDamageKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Damage(TestCard.MaxHealth);

            Assert.IsTrue(!TestCard.Alive());
            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }
        [UnityTest]
        public void FullStatMinusKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.StatModifier(-TestCard.MaxHealth);

            Assert.IsTrue(!TestCard.Alive());
            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }
        
        [UnityTest]
        public void DiesKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Die();

            Assert.IsTrue(!TestCard.Alive());

            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }


    }
}