using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public abstract class TestFixture
    {
        protected Creature TestCreature;
        protected Card TestCard;
        protected Card OtherCard;
        protected Hero TestHero;
        private bool AiControlledPlayerValue;

        [TearDown]
        public void CleanAbility()
        {
            AbilityWithEffect.AbilityStackCount = 0;

            Event.ResetEvents();

            GameSettings.Instance.AiControlledPlayer = AiControlledPlayerValue;
        }
        [SetUp]
        public void Setup()
        {
            BattleManager.Init();

            AiControlledPlayerValue = GameSettings.Instance.AiControlledPlayer;
            GameSettings.Instance.AiControlledPlayer = true;
        }

        protected Creature GenerateSummon()
        {
            Trait trait = new Trait()
            {
                name = "Summoned"
            };

            var testCreature = new Creature()
            {
                name = "TesterOne",
                Attack = 2,
                Health = 30,
                Traits = new List<Trait>()
                {
                    trait
                }

            };

            return testCreature;
        }

        protected Card GenerateTestCreatureWithAbility(SpecialAbility ability, Race race = null,bool playerdeck = true, Trait trait = null)
        {
            if(trait == null)
                 trait = new Trait()
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

                    BattleManager.SetPlayerDeck(TestDeckObject);
                }

                testDeck = BattleManager.Instance.PlayerDeck;
                testDeck.DeckController = new AI(testDeck);
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
                testDeck.DeckController = new AI(testDeck);
            }

            if (ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);

            testDeck.AddCard(testCard);

            return testCard;
        }



        protected Card GenerateTestCreatureWithTrait(string traitName, int attack = 2, int health = 10)
        {
            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = traitName
            };

            var TestCreature = new Creature()
            {
                name = "Tester" + UnityEngine.Random.Range(0, 1000),
                Attack = attack,
                Health = health,
                Traits = string.IsNullOrEmpty(traitName) ? new List<Trait>() : new List<Trait>()
                {
                    trait
                }
            };

            var testCard = new Card(TestCreature);

            return testCard;
        }

        protected static void SetObjectIfCorrectAbility<T>(AbilityWithEffect thisAbility, AbilityWithEffect otherAb, ref T toBeSet, T value)
        {
            if (thisAbility == otherAb)
                toBeSet = value;
        }
        protected Deck GenerateTestDeck(int creatures)
        {
            var testDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            var testDeck = new Deck(testDeckObject);

            for (int i = 0; i < creatures; i++)
            {
                var c = GenerateTestCreatureWithTrait("");

                testDeck.AddCard(c);
            }


            return testDeck;
        }


        protected Hero GenerateHero(AbilityWithEffect ability, Race race = null,List<AbilityWithEffect> heroRaceAbilities = null)
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

                BattleManager.SetPlayerDeck(TestDeckObject);

                var ai = new AI(BattleManager.Instance.PlayerDeck);

                BattleManager.Instance.PlayerDeck.DeckController = ai;
            }

            testDeck = BattleManager.Instance.PlayerDeck;

            testDeck.Hero = hero;

            hero.InDeck = testDeck;

            return hero;
        }

        protected static Creature CreateCreature()
        {

            Trait trait = new Trait()
            {
                Description = "Testing a trait",
                name = "TestTrait"
            };


            return new Creature()
            {
                name = "Testeron",
                Attack = 2,
                Health = 3,
                Traits = new List<Trait>()
                {
                    trait
                }
            };
        }

    }
}
