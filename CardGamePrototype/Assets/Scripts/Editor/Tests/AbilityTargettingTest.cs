using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public class AbilityTargettingTest : TestFixture
    {

        [Test]
        public void TarrgetsTraitTest()
        {

            var defender = new Trait()
            {
                name = "Defender",

            };

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Battlefield, defender)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };


            TestCard = GenerateTestCreatureWithAbility(null, null, true, defender);
            OtherCard = GenerateTestCreatureWithAbility(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards = ts);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }

        [Test]
        public void TargetsCharacterThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.HealthChange(-1);



            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }
        [Test]
        public void TargetsCharacterOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.One, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.HealthChange(-1);

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
        }

        [Test]
        public void TargetsCharacterAny()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.Two, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);


            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));

            OtherCard.HealthChange(-1);

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }

        [Test]
        public void TargetsCharacterItWhenOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref targettedCards, ts));


            OtherCard.HealthChange(-1);



            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
        }
        [Test]
        public void TargetsCharacterItWhenThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.HealthChange(-1);



            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }

        [Test]
        public void TargetsRaceThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };

            TestCard = GenerateTestCreatureWithAbility(testAbility, race);
            OtherCard = GenerateTestCreatureWithAbility(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.HealthChange(-1);



            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 2);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
            Assert.IsTrue(targettedCards.Contains(TestCard));
        }

        [Test]
        public void TargetsRaceSameButNoTargets()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Other, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };

            var otherRace = new Race()
            {
                name = "otherrace",

            };

            TestCard = GenerateTestCreatureWithAbility(testAbility, race);
            OtherCard = GenerateTestCreatureWithAbility(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.HealthChange(-1);



            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 0);
        }
        [Test]
        public void TargetsRaceOther()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Other, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Different)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            var race = new Race()
            {
                name = "testrace",

            };

            var otherRace = new Race()
            {
                name = "otherrace",

            };

            TestCard = GenerateTestCreatureWithAbility(testAbility, race);
            OtherCard = GenerateTestCreatureWithAbility(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = new List<Card>();

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards.AddRange(ts));

            TestCard.HealthChange(-1);

            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
            Assert.IsFalse(targettedCards.Contains(TestCard));
            Assert.IsTrue(targettedCards.Count == 1);
        }

    }
}
