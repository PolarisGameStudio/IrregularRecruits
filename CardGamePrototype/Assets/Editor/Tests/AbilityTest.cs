using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public class AbilityTest
    {
        private Creature TestCreature;
        private Deck TestDeck;
        private DeckObject TestDeckObject;
        private Card TestCard;
        private Card OtherCard;


        [TearDown]
        public void CleanAbility()
        {
            if (TestCard != null && TestCard.Creature?.SpecialAbility)
            {
                TestCard.Creature.SpecialAbility.RemoveListeners(TestCard);
            }
            if (OtherCard != null && OtherCard.Creature?.SpecialAbility)
            {
                OtherCard.Creature.SpecialAbility.RemoveListeners(OtherCard);
            }
        }

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

            if (BattleManager.Instance.PlayerDeck == null)
            {
                TestDeckObject = new DeckObject()
                {
                    Creatures = new List<Creature>(),
                };

                BattleManager.Instance.PlayerDeck = new Deck(TestDeckObject, true);
            }

            TestDeck = BattleManager.Instance.PlayerDeck;

            if (ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);

            TestDeck.AddCard(testCard);

            return testCard;
        }

        private void SetObjectIfCorrectAbility<T>(Ability thisAbility, Ability otherAb, ref T toBeSet, T value)
        {
            if (thisAbility == otherAb)
                toBeSet = value;
        }


        [Test]
        public void PlayTriggers()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.This, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Hand), Ability.Verb.ETB),
            };

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();



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

            TestCard = GenerateTestCreature(testAbility);
            var other = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            other.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => triggeredAblity = a);

            other.PlayCard();



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

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Library);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.PlayCard();



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

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            Ability triggeredAblity = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.Damage(1);



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

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.Damage(1);



            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }
        [Test]
        public void NounsTargetsCharacterOther()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Heal, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.Damage(1);



            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
        }

        [Test]
        public void NounsTargetsCharacterAny()
        {

            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.Two, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.Damage(1);

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }
        [Test]
        public void NounsTargetsCharacterItWhenOther()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));


            OtherCard.Damage(1);



            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
        }
        [Test]
        public void NounsTargetsCharacterItWhenThis()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.Damage(1);



            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }

        [Test]
        public void NounsTargetsRaceThis()
        {
            var testAbility = new Ability()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.Withdraw, Ability.Count.All, 1, new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same)),
                TriggerCondition = new Ability.Trigger(new Noun(Noun.CharacterTyp.Any), Ability.Verb.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.Damage(1);



            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
            Assert.IsTrue(targettedCards.Contains(TestCard));
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.Damage(1);



            Assert.IsTrue(TestCard.Alive());

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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityTrigger.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.Damage(1);

            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
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
