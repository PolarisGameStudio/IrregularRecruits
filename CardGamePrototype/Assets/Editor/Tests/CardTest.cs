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

            TestDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };
            TestDeck = new Deck(TestDeckObject, true);
            TestDeck.AddCard(TestCard);

            //To make test run faster:
            GameSettings.Instance.CombatSpeed = 0.01f;
        }

        [Test]
        public void CardHasCorrectCreatureStats()
        {
            Assert.IsTrue(TestCreature.Attack == TestCard.Attack);
            Assert.IsTrue(TestCreature.Health == TestCard.CurrentHealth);
            Assert.IsTrue(TestCreature.Health == TestCard.MaxHealth);
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
            TestCard.ChangeLocation(Deck.Zone.Hand);
            CombatManager.PlayerActionsLeft = 1;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }

            Assert.IsTrue(TestCard.Location == Deck.Zone.Battlefield);
        }
        [UnityTest]
        public IEnumerator PlayTriggersOnPlayEvent()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            CombatManager.PlayerActionsLeft = 1;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }

            Assert.IsTrue(false);
        }
        [UnityTest]
        public IEnumerator ClickPlaysNotWhenNotInHand()
        {
            TestCard.ChangeLocation(Deck.Zone.Library);
            CombatManager.PlayerActionsLeft = 1;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }

            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }
        [UnityTest]
        public IEnumerator ClickPlaysNotWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            CombatManager.PlayerActionsLeft = 0;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }

            Assert.IsTrue(TestCard.Location == Deck.Zone.Hand);
        }
        [UnityTest]
        public IEnumerator ClickWithdrawsWhenActionAndOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            CombatManager.PlayerActionsLeft = 1;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }

            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }
        [UnityTest]
        public IEnumerator WithDrawTriggersWithdraw()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            CombatManager.PlayerActionsLeft = 1;


            yield return null;

            Assert.IsTrue(false);
        }
        [UnityTest]
        public IEnumerator ClickDoesNotWithdrawsWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            CombatManager.PlayerActionsLeft = 0;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }


            Assert.IsFalse(TestCard.Location == Deck.Zone.Library);
        }
        [UnityTest]
        public IEnumerator ClickWithdrawsNotWhenNotOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            CombatManager.PlayerActionsLeft = 1;

            TestCard.Click();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }


            Assert.IsFalse(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void HealHeals()
        {
            var startHealth = TestCard.CurrentHealth;
            var damage = 2;

            TestCard.Damage(damage);

            Assert.IsTrue(startHealth - damage == TestCard.CurrentHealth);
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
        public void DamageChangesHealth()
        {
            var startHealth = TestCard.CurrentHealth;
            var damage = 2;

            TestCard.Damage(damage);

            Assert.IsTrue(startHealth - damage == TestCard.CurrentHealth);
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
        [UnityTest]
        public IEnumerator DamageTriggersOnDamageEvent()
        {
            Card check = null;

            var damage = 1;

            Event.OnDamaged.AddListener(i => check = i);

            TestCard.Damage(damage);

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }


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

            Assert.IsTrue(startAttack + boost == TestCard.Attack);
            Assert.IsTrue(startHealth + boost == TestCard.CurrentHealth);
            Assert.IsTrue(startMaxHealth + boost == TestCard.MaxHealth);
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
            Card check = null;

            var damage = 1;

            Event.OnDamaged.AddListener(i => check = i);

            TestCard.StatModifier(-damage);

            Assert.IsFalse(check == TestCard);
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

        [UnityTest]
        public IEnumerator DiesKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Die();

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
                yield return null;
            }


            Assert.IsTrue(!TestCard.Alive());

            Assert.IsTrue(TestCard.Location == Deck.Zone.Graveyard);
        }

        [Test]
        public void DiesTriggersOnDeathEvent()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Die();


            Assert.IsTrue(false);
        }


    }
}