using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
namespace Tests
{
    public class AbilityTest
    {
        private Card TestCard, OtherCard;
        private Creature TestCreature,OtherCreature;
        private Deck TestDeck;
        private DeckObject TestDeckObject;
        [OneTimeSetUp]
        public void AbilitySetup()
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = "TestTrait"
            };

            TestCreature = new Creature()
            {
                name = "TesterOne",
                Attack = 2,
                Health = 3,
                Traits = new List<Trait>()
                {
                    trait
                }
            };
            OtherCreature = new Creature()
            {
                name = "TesterTwo",
                Attack = 2,
                Health = 3,
                Traits = new List<Trait>()
                {
                    trait
                }
            };


            TestCard = new Card(TestCreature);
            OtherCard = new Card(OtherCreature);

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
        public void ETBTriggers()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun()),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.This), Ability.Verb.ETB),
            };

            TestCreature.SpecialAbility = testAbility;

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => triggeredAblity = a);




            Assert.IsTrue(false);
        }
        [Test]
        public void ETBTriggersNotOnHand()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTargetsCharacterThis()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTargetsCharacterOther()
        {

            Assert.IsTrue(false);
        }
        
        [Test]
        public void NounsTargetsCharacterAny()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTargetsCharacterIt()
        {

            Assert.IsTrue(false);
        }

        [Test]
        public void NounsTargetsRaceThis()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTargetsRaceOther()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTriggersCharacterThis()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTriggersCharacterOther()
        {

            Assert.IsTrue(false);
        }
        
        [Test]
        public void NounsTriggersCharacterAny()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTriggersCharacterIt()
        {

            Assert.IsTrue(false);
        }

        [Test]
        public void NounsTriggersRaceThis()
        {

            Assert.IsTrue(false);
        }
        [Test]
        public void NounsTriggersRaceOther()
        {

            Assert.IsTrue(false);
        }


    }
}
