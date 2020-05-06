using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class CardTest
    {
        private BattleManager BattleManager;
        private Card TestCard;
        private Creature TestCreature;
        private Deck TestDeck;
        private DeckObject TestDeckObject;
        private PlayerController TestDeckController;
        private Ability TestAbility;

        [SetUp]
        public void CardSetup()
        {
            //Event.ResetListeners();

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

            TestAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun()),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.This), Ability.Verb.ETB),
            };

            TestCreature.SpecialAbility = TestAbility;

            TestCard = new Card(TestCreature);


            TestDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            TestDeckController = new PlayerController();

            TestDeck = new Deck(TestDeckObject, true);

            TestDeck.DeckController = TestDeckController;

            TestDeck.AddCard(TestCard);
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
            TestDeckController.PlayerActionsLeft = 1;


            TestCard.Click();

            Assert.IsTrue(TestCard.Location == Deck.Zone.Battlefield);
        }

        [Test]
        public void PlayTriggersOnPlayEvent()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.PlayerActionsLeft = 1;

            var triggered = false;

            Event.OnEtb.AddListener(c => triggered = true);

            TestCard.Click();

            Assert.IsTrue(triggered);
        }
        [Test]
        public void ClickPlaysNotWhenNotInHand()
        {
            TestCard.ChangeLocation(Deck.Zone.Library);
            TestDeckController.PlayerActionsLeft = 1;


            TestCard.Click();



            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void ClickPlaysNotWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);
            TestDeckController.PlayerActionsLeft = 0;

            TestCard.Click();



            Assert.IsTrue(TestCard.Location == Deck.Zone.Hand);
        }
        [Test]
        public void ClickWithdrawsWhenActionAndOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            TestDeckController.PlayerActionsLeft = 1;

            TestCard.Click();



            Assert.IsTrue(TestCard.Location == Deck.Zone.Library);
        }

        [Test]
        public void WithDrawTriggersWithdraw()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            var triggered = false;

            Event.OnWithdraw.AddListener(c => triggered = true);

            TestCard.Withdraw();



            Assert.IsTrue(triggered);
        }

        [Test]
        public void ClickDoesNotWithdrawsWhenNoAction()
        {
            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            TestDeckController.PlayerActionsLeft = 0;

            TestCard.Click();


            Assert.IsFalse(TestCard.Location == Deck.Zone.Library);
        }
        [Test]
        public void ClickWithdrawsNotWhenNotOnBattlefield()
        {
            TestCard.ChangeLocation(Deck.Zone.Hand);

            TestDeckController.PlayerActionsLeft = 1;

            TestCard.Click();



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

            Event.OnHealed.AddListener((i, val) => check = i);

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
            Card card = null;

            Event.OnHealthLoss.AddListener((c,i) => check = i);
            Event.OnDamaged.AddListener((c) => card = c);

            TestCard.Damage(damage);

            Assert.NotNull(card);
            Assert.IsTrue(check == damage);
            Assert.AreEqual(card, TestCard);
        }
        [Test]
        public void DamageTriggersOnDamageEvent()
        {
            Card check = null;

            var damage = 1;

            Event.OnDamaged.AddListener((i) => check = i);

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

            Event.OnStatMod.AddListener((c, i) => check = i);
            Event.OnStatMod.AddListener((c, i) => card = c);

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

            Event.OnHealed.AddListener((i,val) => check = i);

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

            Event.OnDamaged.AddListener((i) => check = i);

            TestCard.StatModifier(-damage);

            Assert.IsFalse(check == TestCard);
        }

        [Test]
        public void FullDamageKillsCard()
        {
            Assert.IsTrue(TestCard.Alive());

            TestCard.Damage(TestCard.MaxHealth);



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

            Event.OnDeath.AddListener(c => triggered = true);

            TestCard.Die();



            Assert.IsTrue(triggered);
        }


    }
}