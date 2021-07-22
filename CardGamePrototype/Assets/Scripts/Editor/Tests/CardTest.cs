using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class CardTest : TestFixture
    {
        private Deck TestDeck;
        private DeckObject TestDeckObject;
        private PlayerController TestDeckController;
        private PassiveAbility TestAbility;

        //TODO: move these up to testFixture
        [SetUp]
        public void CardSetup()
        {
            TestCreature = CreateCreature();

            TestAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun()),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.ETB),
            };

            TestCreature.SpecialAbility = TestAbility;
            TestCard = CreateCard();

        }

        private Card CreateCard()
        {
            var TestCard = new Card(TestCreature);

            TestDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };
            TestDeck = new Deck(TestDeckObject);

            TestDeckController = new PlayerController(TestDeck);
            
            TestDeck.DeckController = TestDeckController;

            TestDeck.AddCard(TestCard);

            return TestCard;
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

            Assert.IsTrue(TestCard.Ability() == TestAbility);

        }

        [Test]
        public void IsInCorrectDeck()
        {
            Assert.IsTrue(TestCard.InDeck == TestDeck);
            Assert.NotNull(TestCard.InDeck);
        }

        [Test]
        public void ChangesLocation()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            Assert.IsTrue(TestCard.Location == Deck.Zone.Battlefield);

            TestCard.ChangeLocation(Deck.Zone.Library);


            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }



        [Test]
        public void ClickPlaysWhenActionAndOnHand()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.ActionsLeft = 1;


            TestCard.Click();

            Assert.IsTrue(TestCard.Location == Deck.Zone.Battlefield);
        }
        

        [Test]
        public void ClickPlaysUsesAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.ActionsLeft = 1;


            TestCard.Click();
            Assert.AreEqual(0,TestDeckController.ActionsLeft);
        }

        [Test]
        public void PlayTriggersOnPlayEvent()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.ActionsLeft = 1;

            var triggered = false;

            Event.OnEtb.AddListener((c,l) => triggered = true);

            TestCard.Click();

            Assert.IsTrue(triggered);
        }
        [Test]
        public void ClickPlaysNotWhenNotInHand()
        {
            TestCard.ChangeLocation(Deck.Zone.Library);
            TestDeckController.ActionsLeft = 1;


            TestCard.Click();



            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void ClickPlaysNotWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.ActionsLeft = 0;

            TestCard.Click();

            Assert.IsTrue(TestCard.Location == Deck.Zone.Hand);
        }
        [Test]
        public void ClickWithdrawsUsesAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            TestDeckController.ActionsLeft = 1;

            TestCard.Click();

            Assert.AreEqual(TestDeckController.ActionsLeft, 0);
        }
        [Test]
        public void ClickWithdrawsWhenActionAndOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            TestDeckController.ActionsLeft = 1;

            TestCard.Click();

            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }

        [Test]
        public void WithDrawTriggersWithdraw()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            var triggered = false;

            Event.OnWithdraw.AddListener((c,l) => triggered = true);

            TestCard.Withdraw();



            Assert.IsTrue(triggered);
        }

        [Test]
        public void ClickDoesNotWithdrawsWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            TestDeckController.ActionsLeft = 0;

            TestCard.Click();

            Assert.IsFalse(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void ClickWithdrawsNotWhenNotOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);

            TestDeckController.ActionsLeft = 1;

            TestCard.Click();



            Assert.IsFalse(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void HealHeals()
        {
            var startHealth = TestCard.CurrentHealth;
            var damage = 2;

            TestCard.HealthChange(-damage);


            Assert.IsTrue(startHealth - damage == TestCard.CurrentHealth);
        }
        [Test]
        public void HealTriggersOnHeal()
        {
            Card check = null;
            TestCard.HealthChange(1);

            var heal = 1;

            Event.OnHealed.AddListener((i, val,l) => check = i);

            TestCard.HealthChange(heal);


            Assert.IsTrue(check == TestCard);
        }
        [Test]
        public void DamageChangesHealth()
        {
            var startHealth = TestCard.CurrentHealth;
            var damage = 2;

            TestCard.HealthChange(-damage);


            Assert.IsTrue(startHealth - damage == TestCard.CurrentHealth);
        }
        [Test]
        public void DamageTriggersOnDamage()
        {
            var check = 0;
            var damage = 3;
            Card card = null;

            Event.OnHealthChange.AddListener((c,i,l) => check = i);
            Event.OnDamaged.AddListener((c,l) => card = c);

            TestCard.HealthChange(-damage);

            Assert.NotNull(card);
            Assert.AreEqual(check,-damage);
            Assert.AreEqual(card, TestCard);
        }
        [Test]
        public void DamageTriggersOnDamageEvent()
        {
            Card check = null;

            var damage = 1;

            Event.OnDamaged.AddListener((i,l) => check = i);

            TestCard.HealthChange(-damage);



            Assert.IsTrue(check == TestCard);
        }
        [Test]
        public void HealthChangeTriggersBeforeDeathEvent()
        {
            Card damaged = null;
            bool hasBeenDamagedFirst = false;



            var damage = 1000;

            Event.OnHealthChange.AddListener((i,v,l) => damaged = i);
            Event.OnDeath.AddListener((i,l) => hasBeenDamagedFirst = i == damaged);

            TestCard.HealthChange(-damage);

            Assert.IsTrue(damaged == TestCard);
            Assert.IsTrue(hasBeenDamagedFirst);
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
            Card card = null;

            Event.OnStatMod.AddListener((c, i,l) => check = i);
            Event.OnStatMod.AddListener((c, i,l) => card = c);

            TestCard.StatModifier(boost);

            Assert.IsTrue(check == boost);
            Assert.NotNull(card);
            Assert.AreEqual(card, TestCard);
        }

        //TODO: move to event test
        [Test]
        public void StatboostDoesNotTriggerHeal()
        {
            Card check = null;
            var boost = 3;

            Event.OnHealed.AddListener((i,val,l) => check = i);

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

            Event.OnDamaged.AddListener((i,l) => check = i);

            TestCard.StatModifier(-damage);

            Assert.IsFalse(check == TestCard);
        }

        [Test]
        public void FullDamageKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());

            TestCard.HealthChange(-TestCard.MaxHealth);



            Assert.IsTrue(!TestCard.Alive());
        }
        [Test]
        public void FullStatMinusKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());

            TestCard.StatModifier(-TestCard.MaxHealth);



            Assert.IsTrue(!TestCard.Alive());
        }

        [Test]
        public void DiesKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            TestCard.Die();



            Assert.IsTrue(!TestCard.Alive());
        }

        [Test]
        public void DiesTriggersOnDeathEvent()
        {
            Assert.IsTrue(TestCard.Alive());
            Assert.IsTrue(TestCard.Location != Deck.Zone.Graveyard);

            var triggered = false;

            Event.OnDeath.AddListener((c,l) => triggered = true);

            TestCard.Die();



            Assert.IsTrue(triggered);
        }
    }
}