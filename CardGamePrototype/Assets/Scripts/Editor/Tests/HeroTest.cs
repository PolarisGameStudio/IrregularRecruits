using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public class HeroTest : TestFixture
    {
        [Test]
        public void HeroHasAbility()
        {
            var testAbility = new PassiveAbility();

            TestHero = GenerateHero(testAbility);

            Assert.IsNotNull(TestHero);
            Assert.IsTrue(TestHero.Abilities.Contains(testAbility));
        }


        [Test]
        public void PassiveAbilityWorks()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth-1);
        }
        
        [Test]
        public void PassiveAbilityWorksWhenSeveralAbilities()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.ETB),
            };
            var testAbility2 = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestHero.AddAbility(testAbility2);

            TestCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            int triggered = 0;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered++);

            TestCard.PlayCard();

            Assert.AreEqual(triggered, 2);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth - 2);
        }

        [Test]
        public void ActiveAbilityWorks()
        {
            var testAbility = new ActiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestHero.InDeck.DeckController.ResetActions();

            testAbility.ActivateAbility(TestHero);

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth - 1);
        }

        [Test]
        public void ActiveAbilityOnlyOneTimePrRound()
        {
            var testAbility = new ActiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            int triggeredAblity = 0;

            TestHero.InDeck.DeckController.ResetActions();

            var actions = TestHero.InDeck.DeckController.ActionsLeft;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity++);

            testAbility.ActivateAbility(TestHero);
            testAbility.ActivateAbility(TestHero);

            Assert.AreEqual(1, triggeredAblity);

        }

        [Test]
        public void PassiveAbilityThisRaceTriggers()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Same), TriggerType.ETB),
            };

            var testRace = new Race()
            {
                name = "Testos"
            };

            TestHero = GenerateHero(testAbility,testRace);

            TestCard = GenerateTestCreatureWithAbility(null, testRace);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth - 1);
        }
        [Test]
        public void PassiveAbilityThisRaceTriggersNotOnWrongRace()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Same), TriggerType.ETB),
            };

            var testRace = new Race()
            {
                name = "Testos"
            };
            var otherRace = new Race()
            {
                name = "NotTestos"
            };

            TestHero = GenerateHero(testAbility, testRace);

            TestCard = GenerateTestCreatureWithAbility(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNull(triggeredAblity);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth);
        }

        [Test]
        public void PassiveAbilityEnemyTrigger()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Enemy), TriggerType.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null,null,false);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth - 1);
        }

        [Test]
        public void PassiveAbilityEnemyDoesNotTriggerOnFriend()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Enemy), TriggerType.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null, null, true);


            TestCard.ChangeLocation(Deck.Zone.Hand);

            SpecialAbility triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestCard.PlayCard();

            Assert.IsNull(triggeredAblity);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth);
        }

        [Test]
        public void ActiveAbilityDoesNotTriggerWhenNoActionsLeft()
        {
            var testAbility = new ActiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            SpecialAbility triggeredAblity = null;

            TestHero.InDeck.DeckController.ResetActions();

            TestHero.InDeck.DeckController.UsedAction(TestHero.InDeck);
            TestHero.InDeck.DeckController.UsedAction(TestHero.InDeck);

            var actions = TestHero.InDeck.DeckController.ActionsLeft;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            testAbility.ActivateAbility(TestHero);

            Assert.IsNull(triggeredAblity);
            Assert.AreEqual(actions, 0);
            Assert.AreEqual(actions, TestHero.InDeck.DeckController.ActionsLeft); 
        }

        [Test]
        public void XpTriggersLevelUp()
        {
            TestHero = GenerateHero(null);

            bool lvlUp = false;

            var heroLevel = TestHero.CurrentLevel;

            var levelPoints = TestHero.LevelUpPoints;

            var xp = Hero.LevelCaps[heroLevel];

            Event.OnLevelUp.AddListener(c => lvlUp = true);

            TestHero.AwardXp(xp);

            Assert.IsTrue(lvlUp);
            Assert.AreEqual(heroLevel + 1, TestHero.CurrentLevel);
            Assert.AreEqual(levelPoints + 1, TestHero.LevelUpPoints);
        }

        [Test]
        public void XpTriggersMultipleLevelUps()
        {
            TestHero = GenerateHero(null);

            bool lvlUp = false;

            var heroLevel = TestHero.CurrentLevel;
            var levelPoints = TestHero.LevelUpPoints;

            var xp = Hero.LevelCaps[heroLevel + 2];

            Event.OnLevelUp.AddListener(c => lvlUp = true);

            TestHero.AwardXp(xp);

            Assert.IsTrue(lvlUp);
            Assert.AreEqual(heroLevel + 3, TestHero.CurrentLevel);
            Assert.AreEqual(levelPoints + 3, TestHero.LevelUpPoints);
        }

        [Test]
        public void XpDoesNotTriggerLevelUp()
        {

            TestHero = GenerateHero(null);

            bool lvlUp = false;

            var heroLevel = TestHero.CurrentLevel;
            var levelPoints = TestHero.LevelUpPoints;

            Event.OnLevelUp.AddListener(c => lvlUp = true);

            TestHero.AwardXp(1);

            Assert.IsFalse(lvlUp);
            Assert.AreEqual(heroLevel , TestHero.CurrentLevel);
            Assert.AreEqual(levelPoints , TestHero.LevelUpPoints);
        }

        [Test]
        public void AbilitySelectWhenLevelUpPoints()
        {
            var levelOptions = new List<AbilityWithEffect> { 
                new PassiveAbility() { Name = "passive0" }, 
                new ActiveAbility() { Name = "active1" }, 
                new ActiveAbility() { Name = "active2" },
                new PassiveAbility() { Name = "passive3" },
                new ActiveAbility() { Name = "active4" } 
            };

            TestHero = GenerateHero(null,null, levelOptions);

            var xp = Hero.LevelCaps[TestHero.CurrentLevel + 1];

            TestHero.AwardXp(xp);

            var levelPoints = TestHero.LevelUpPoints;

            TestHero.SelectLevelUpAbility(levelOptions[0]);

            Assert.IsTrue(TestHero.Abilities.Contains(levelOptions[0]));
            Assert.AreEqual(levelPoints-1, TestHero.LevelUpPoints);
        }

        [Test]
        public void AbilitySelectNotTooHighLevel()
        {

            var levelOptions = new List<AbilityWithEffect> {
                new PassiveAbility() { Name = "passive0" },
                new ActiveAbility() { Name = "active1" },
                new ActiveAbility() { Name = "active2" },
                new PassiveAbility() { Name = "passive3" },
                new ActiveAbility() { Name = "active4" }
            };

            TestHero = GenerateHero(null, null, levelOptions);

            var xp = Hero.LevelCaps[TestHero.CurrentLevel + 1];

            TestHero.AwardXp(xp);

            var levelPoints = TestHero.LevelUpPoints;

            TestHero.SelectLevelUpAbility(levelOptions[TestHero.CurrentLevel]);

            Assert.IsFalse(TestHero.Abilities.Contains(levelOptions[0]));
            Assert.AreEqual(levelPoints, TestHero.LevelUpPoints);
        }

        [Test]
        public void AbilitySelectNotPossibleWhenNoPoints()
        {
            var levelOptions = new List<AbilityWithEffect> {
                new PassiveAbility() { Name = "passive0" },
                new ActiveAbility() { Name = "active1" },
                new ActiveAbility() { Name = "active2" },
                new PassiveAbility() { Name = "passive3" },
                new ActiveAbility() { Name = "active4" }
            };

            TestHero = GenerateHero(null, null, levelOptions);

            var levelPoints = TestHero.LevelUpPoints;

            TestHero.SelectLevelUpAbility(levelOptions[0]);

            Assert.AreEqual(levelPoints, 0);
            Assert.IsFalse(TestHero.Abilities.Contains(levelOptions[0]));
            Assert.AreEqual(levelPoints, TestHero.LevelUpPoints);
        }

        [Test]
        public void AbilitySelectNotPossibleWhenAlreadyHas()
        {
            var levelOptions = new List<AbilityWithEffect> {
                new PassiveAbility() { Name = "passive0" },
                new ActiveAbility() { Name = "active1" },
                new ActiveAbility() { Name = "active2" },
                new PassiveAbility() { Name = "passive3" },
                new ActiveAbility() { Name = "active4" }
            };

            TestHero = GenerateHero(levelOptions[0], null, levelOptions);

            var xp = Hero.LevelCaps[TestHero.CurrentLevel + 1];

            TestHero.AwardXp(xp);

            var levelPoints = TestHero.LevelUpPoints;

            TestHero.SelectLevelUpAbility(levelOptions[0]);

            Assert.IsTrue(TestHero.Abilities.Contains(levelOptions[0]));
            //Not used points
            Assert.AreEqual(levelPoints , TestHero.LevelUpPoints);
        }
    }

}
