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
            //TestDeck = new Deck(TestDeckObject, true);
            //TestDeck.AddCard(TestCard);

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
            Assert.NotNull(TestCard.InDeck);
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
        [Test]
        public void HealTriggersOnHeal()
        {
            Card check = null;
            TestCard.Damage(1);

            var heal = 1;

            Event.OnHealed.AddListener(i => check = i);

            TestCard.Heal(heal);

            Assert.IsTrue(check == TestCard);
        }
        [Test]
        public void DamageDamages()
        {

        }
        [Test]
        public void DamageTriggersOnDamage()
        {
            var check = 0;
            var damage = 3;

            TestCard.OnDamage.AddListener(i => check = i);

            TestCard.Damage(damage);

            Assert.IsTrue(check == damage);
        }
        [Test]
        public void DamageTriggersOnDamageEvent()
        {
            Card check = null;

            var damage = 1;

            Event.OnDamaged.AddListener(i => check = i);

            TestCard.Damage(damage);

            Assert.IsTrue(check == TestCard);
        }

        [Test]
        public void StatboostBoosts()
        {
            var startAttack = TestCard.Attack;
            var startHealth = TestCard.CurrentHealth;
            var startMaxHealth = TestCard.MaxHealth;
            var boost = 31;

            TestCard.StatModifier(boost);

            Assert.IsTrue(startAttack+boost == TestCard.Attack);
            Assert.IsTrue(startHealth+boost == TestCard.CurrentHealth);
            Assert.IsTrue(startMaxHealth+boost == TestCard.MaxHealth);
        }
        [Test]
        public void StatboostTriggersOnStatMod()
        {
            var check = 0;
            var boost = 3;

            TestCard.OnStatMod.AddListener(i => check = i);

            TestCard.StatModifier(boost);

            Assert.IsTrue(check == boost);
        }

        //TODO: move to event test
        [Test]
        public void StatboostDoesNotTriggerHeal()
        {
            Card check = null;
            var boost = 3;

            Event.OnHealed.AddListener(i => check = i);

            TestCard.StatModifier(boost);

            Assert.IsFalse(check == TestCard);
        }
        [Test]
        public void StatMinusMinusses()
        {
            var startAttack = TestCard.Attack;
            var startHealth = TestCard.CurrentHealth;
            var startMaxHealth = TestCard.MaxHealth;
            var boost = -1;

            TestCard.StatModifier(boost);

            Assert.IsTrue(startAttack + boost == TestCard.Attack);
            Assert.IsTrue(startHealth + boost == TestCard.CurrentHealth);
            Assert.IsTrue(startMaxHealth + boost == TestCard.MaxHealth);
        }
        [Test]
        public void StatMinusDoesNotTriggerDamage()
        {

        }

        [Test]
        public void FullDamageKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Damage(TestCard.MaxHealth);

            Assert.IsTrue(!TestCard.Alive());
            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }
        [Test]
        public void FullStatMinusKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.StatModifier(-TestCard.MaxHealth);

            Assert.IsTrue(!TestCard.Alive());
            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }
        
        [Test]
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