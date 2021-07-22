using GameLogic;
using NUnit.Framework;
namespace Tests
{
    public class AbilityTriggerTest : TestFixture
    {

        [Test]
        public void TriggerThisDiesTriggers()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.Die();

            Assert.IsTrue(triggered);
            Assert.IsTrue(TestCard.Alive());
        }

        [Test]
        public void TriggerAnyDiesTriggersOnThis()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.Die();

            Assert.IsTrue(triggered);
            Assert.IsTrue(TestCard.Alive());
        }

        [Test]
        public void TriggersTraits()
        {

            var defender = new Trait()
            {
                name = "Defender",

            };

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Battlefield, defender), TriggerType.IsDAMAGED),
            };


            TestCard = GenerateTestCreatureWithAbility(testAbility, null, true, defender);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            SpecialAbility ability = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => ability = a);

            TestCard.HealthChange(-1);

            Assert.IsNotNull(ability);
            Assert.AreEqual(testAbility, ability);
        }


        [Test]
        public void TriggersCharacterThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void TriggersCharacterThisNotOnOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void TriggersCharacterOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void TriggersCharacterOtherNotOnThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void TriggersCharacterAny()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            int triggered = 0;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered++);

            OtherCard.HealthChange(-1);
            TestCard.HealthChange(-1);

            Assert.AreEqual(triggered, 2);
        }
        [Test]
        public void TriggersNotCharacterIt()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.It), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);
            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void TriggersRaceThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same), TriggerType.IsDAMAGED),
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
            OtherCard = GenerateTestCreatureWithAbility(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void TriggersRaceOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Different), TriggerType.IsDAMAGED),
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

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void TriggersRaceThisNotOnOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Same), TriggerType.IsDAMAGED),
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

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void TriggersRaceOtherNotOnThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Different), TriggerType.IsDAMAGED),
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
            OtherCard = GenerateTestCreatureWithAbility(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void TriggerPlayTriggersThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.ETB),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
        }
        [Test]
        public void TriggerPlayTriggersOnOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.ETB),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            var other = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            other.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            other.PlayCard();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
        }

        [Test]
        public void TriggerKillsTriggersOnAttack()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.KILLS),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.StatModifier(50);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killer = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnKill.AddListener((c,l) => killer = c);

            TestCard.AttackCard(OtherCard);

            Assert.IsTrue(triggered);
            Assert.NotNull(killer);
            Assert.AreEqual(killer, TestCard);
        }
        [Test]
        public void TriggerKillsTriggersOnBeingAttacked()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.KILLS),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.StatModifier(50);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killer = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnKill.AddListener((c,l) => killer = c);

            OtherCard.AttackCard(TestCard);

            Assert.IsTrue(triggered);
            Assert.NotNull(killer);
            Assert.AreEqual(killer, TestCard);
        }

        [Test]
        public void TriggerDeathTriggers()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card death = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnDeath.AddListener((c,l) => death = c);

            OtherCard.Die();

            Assert.NotNull(death);
            Assert.IsTrue(triggered);
        }

        [Test]
        public void TriggerETBTriggersNotFromLibrary()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.ETB),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Library);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.PlayCard();

            Assert.IsFalse(triggeredAblity == testAbility);
        }
        
        [Test]
        public void TriggersIfLocationIsChangedAfter()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other), TriggerType.ETB),
            };

            var testAbility2 = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.ETB),
            };

            var scaredCat = GenerateTestCreatureWithAbility(testAbility2);
            TestCard = GenerateTestCreatureWithAbility(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            scaredCat.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;
            SpecialAbility triggeredAblity2 = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));
            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility2, a, ref triggeredAblity2, a));

            scaredCat.PlayCard();

            Assert.AreEqual(testAbility2, triggeredAblity2);
            Assert.AreEqual(testAbility,triggeredAblity);
        }


        [Test]
        public void TriggerDamageTriggers()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggeredAblity == testAbility);
        }

        [Test]
        public void OwnerOnHandDoesNotTriggerAbility()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Hand);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.AreEqual(OtherCard.MaxHealth - 1, OtherCard.CurrentHealth);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void OwnerInDeckDoesNotTriggerAbility()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Library);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.AreEqual(OtherCard.MaxHealth - 1, OtherCard.CurrentHealth);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void OwnerOnBattlefieldTriggersAbility()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.AreEqual(OtherCard.MaxHealth - 1, OtherCard.CurrentHealth);

            Assert.IsTrue(triggered);
        }

    }
}
