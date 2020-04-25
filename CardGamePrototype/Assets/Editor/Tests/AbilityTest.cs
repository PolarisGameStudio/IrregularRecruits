using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
namespace Tests
{
    public class AbilityTest
    {
        private Creature TestCreature;
        private Deck TestDeck;
        private DeckObject TestDeckObject;
        
        private Card GenerateTestCreature(Ability ability, Race race = null)
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
                Race = race,
                Health = 30,
                Traits = new List<Trait>()
                {
                    trait
                }
                
            };

            TestDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            TestDeck = new Deck(TestDeckObject, true);

            CombatManager.PlayerDeck = TestDeck;
            
            if(ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);

            TestDeck.AddCard(testCard);

            return testCard;
        }

        private void SetObjectIfCorrectAbility<T>(Ability thisAbility,Ability otherAb, ref T toBeSet,T value)
        {
            if (thisAbility == otherAb) 
                toBeSet = value;
        }

        [SetUp]
        public void ResetListeners()
        {
            Event.ResetListeners();
        }

        [Test]
        public void PlayTriggers()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.This,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Hand), Ability.Verb.ETB),
            };

            var testCard = GenerateTestCreature(testAbility);

            testCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => triggeredAblity = a);

            testCard.PlayCard();

            FlowController.ResolveAllActions();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
        }
        [Test]
        public void PlayTriggersOnOther()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.ETB),
            };

            var testCard = GenerateTestCreature(testAbility);
            var other = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            other.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => triggeredAblity = a);

            other.PlayCard();

            FlowController.ResolveAllActions();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
        }
        [Test]
        public void ETBTriggersNotFromLibrary()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.This), Ability.Verb.ETB),
            };

            var testCard = GenerateTestCreature(testAbility);

            testCard.ChangeLocation(Deck.Zone.Library);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            testCard.PlayCard();

            FlowController.ResolveAllActions();

            Assert.IsFalse(triggeredAblity == testAbility);
        }


        [Test]
        public void DamageTriggersAbility()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Heal, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);

            testCard.ChangeLocation(Deck.Zone.Battlefield);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            testCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsTrue(triggeredAblity == testAbility);
        }

        [Test]
        public void NounsTargetsCharacterThis()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);
            var otherCard = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            otherCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(testCard));
        }
        [Test]
        public void NounsTargetsCharacterOther()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Heal, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);
            var otherCard = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            otherCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(otherCard));
        }
        
        [Test]
        public void NounsTargetsCharacterAny()
        {

            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.Two, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);
            var otherCard = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;
            
            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));
            
            otherCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(otherCard));
            Assert.IsTrue(targettedCards.Contains(testCard));
        }
        [Test]
        public void NounsTargetsCharacterItWhenOther()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);
            var otherCard = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));


            otherCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(otherCard));
        }
        [Test]
        public void NounsTargetsCharacterItWhenThis()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var testCard = GenerateTestCreature(testAbility);
            var otherCard = GenerateTestCreature(null);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            testCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsTrue(testCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(testCard));
        }

        [Test]
        public void NounsTargetsRaceThis()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.All, 1, new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Same)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",
                
            };

            var testCard = GenerateTestCreature(testAbility,race);
            var otherCard = GenerateTestCreature(null,race);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            testCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsTrue(testCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(otherCard));
            Assert.IsTrue(targettedCards.Contains(testCard));
        }        
        
        [Test]
        public void NounsTargetsRaceSameButNoTargets()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.All, 1, new Noun(Noun.CharacterTyp.Other, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };
            
            var otherRace = new Race()
            {
                name = "otherrace",

            };

            var testCard = GenerateTestCreature(testAbility, race);
            var otherCard = GenerateTestCreature(null, otherRace);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            testCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsTrue(testCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 0);
        }
        [Test]
        public void NounsTargetsRaceOther()
        {

            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.All, 1, new Noun(Noun.CharacterTyp.Other, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Different)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };

            var otherRace = new Race()
            {
                name = "otherrace",

            };

            var testCard = GenerateTestCreature(testAbility, race);
            var otherCard = GenerateTestCreature(null, otherRace);

            testCard.ChangeLocation(Deck.Zone.Battlefield);
            otherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            testCard.Damage(1);

            FlowController.ResolveAllActions();

            Assert.IsTrue(testCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(otherCard));
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
