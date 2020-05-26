using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public class HeroTest
    {

        private Creature TestCreature;
        private Card TestCard;
        private Card OtherCard;
        private Hero TestHero;

        [TearDown]
        public void CleanListeners()
        {
            if (TestCard != null && TestCard.Creature?.SpecialAbility)
            {
                TestCard.Creature.SpecialAbility.RemoveListeners();
            }
            if (OtherCard != null && OtherCard.Creature?.SpecialAbility)
            {
                OtherCard.Creature.SpecialAbility.RemoveListeners();
            }
            if (TestHero != null)
            {
                foreach (var a in TestHero.Abilities)
                    (a as PassiveAbility)?.RemoveListeners();
            }


            BattleManager.Instance.PackUp(null);
        }

        private Card GenerateTestCreature(PassiveAbility ability, Race race = null, bool playerdeck = true)
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

            Deck testDeck = null;

            if (playerdeck)
            {
                if (BattleManager.Instance.PlayerDeck == null)
                {
                    var TestDeckObject = new DeckObject()
                    {
                        Creatures = new List<Creature>(),
                    };

                    BattleManager.Instance.PlayerDeck = new Deck(TestDeckObject);
                }

                testDeck = BattleManager.Instance.PlayerDeck;
            }
            else
            {
                if (BattleManager.Instance.EnemyDeck == null)
                {
                    var TestDeckObject = new DeckObject()
                    {
                        Creatures = new List<Creature>(),
                    };

                    BattleManager.Instance.EnemyDeck = new Deck(TestDeckObject);
                }

                testDeck = BattleManager.Instance.EnemyDeck;
            }

            if (ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);

            testDeck.AddCard(testCard);

            return testCard;
        }

        private Hero GenerateHero(Ability ability, Race race = null,List<Ability> heroRaceAbilities = null)
        {
            var hero = new Hero(new HeroObject()
            {
                StartingAbility = ability,
                name = "Testeron",
                Race = race,
                RaceOption = new LevelOption() { Options = heroRaceAbilities}
            });

            Deck testDeck = null;

            if (BattleManager.Instance.PlayerDeck == null)
            {
                var TestDeckObject = new DeckObject()
                {
                    Creatures = new List<Creature>(),
                };

                BattleManager.Instance.PlayerDeck = new Deck(TestDeckObject);

                var ai = new AI();

                ai.ControlledDeck = BattleManager.Instance.PlayerDeck;

                BattleManager.Instance.PlayerDeck.DeckController = ai;
            }

            testDeck = BattleManager.Instance.PlayerDeck;

            testDeck.Hero = hero;

            hero.InDeck = testDeck;

            return hero;
        }


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
                ResultingAction = new Ability.Action(PassiveAbility.ActionType.DealDamage, PassiveAbility.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any), PassiveAbility.Verb.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

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
                ResultingAction = new Ability.Action(PassiveAbility.ActionType.DealDamage, PassiveAbility.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any), PassiveAbility.Verb.ETB),
            };
            var testAbility2 = new PassiveAbility()
            {
                ResultingAction = new Ability.Action(PassiveAbility.ActionType.DealDamage, PassiveAbility.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any), PassiveAbility.Verb.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestHero.AddAbility(testAbility2);

            TestCard = GenerateTestCreature(null);

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
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            Ability triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            TestHero.InDeck.DeckController.ResetActions();

            testAbility.ActivateAbility(TestHero);

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(TestCard.CurrentHealth, TestCard.MaxHealth - 1);
        }

        [Test]
        public void ActiveAbilityUsesAction()
        {
            var testAbility = new ActiveAbility()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            Ability triggeredAblity = null;

            TestHero.InDeck.DeckController.ResetActions();

            var actions = TestHero.InDeck.DeckController.ActionsLeft();

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            testAbility.ActivateAbility(TestHero);

            Assert.IsNotNull(triggeredAblity);
            Assert.IsTrue(triggeredAblity == testAbility);
            Assert.AreEqual(actions-1, TestHero.InDeck.DeckController.ActionsLeft());
        }

        [Test]
        public void PassiveAbilityThisRaceTriggers()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Same), PassiveAbility.Verb.ETB),
            };

            var testRace = new Race()
            {
                name = "Testos"
            };

            TestHero = GenerateHero(testAbility,testRace);

            TestCard = GenerateTestCreature(null, testRace);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

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
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Same), PassiveAbility.Verb.ETB),
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

            TestCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

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
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Enemy), PassiveAbility.Verb.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null,null,false);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

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
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new PassiveAbility.Trigger(new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Enemy), PassiveAbility.Verb.ETB),
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null, null, true);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            Ability triggeredAblity = null;

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
                ResultingAction = new Ability.Action(Ability.ActionType.DealDamage, Ability.Count.One, 1, new Noun(Noun.CharacterTyp.Any))
            };

            TestHero = GenerateHero(testAbility);

            TestCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            Ability triggeredAblity = null;

            TestHero.InDeck.DeckController.ResetActions();

            TestHero.InDeck.DeckController.UsedAction(TestHero.InDeck);
            TestHero.InDeck.DeckController.UsedAction(TestHero.InDeck);

            var actions = TestHero.InDeck.DeckController.ActionsLeft();

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggeredAblity = a);

            testAbility.ActivateAbility(TestHero);

            Assert.IsNull(triggeredAblity);
            Assert.AreEqual(actions, 0);
            Assert.AreEqual(actions, TestHero.InDeck.DeckController.ActionsLeft()); 
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
            var levelOptions = new List<Ability> { 
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

            var levelOptions = new List<Ability> {
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
            var levelOptions = new List<Ability> {
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
            var levelOptions = new List<Ability> {
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
