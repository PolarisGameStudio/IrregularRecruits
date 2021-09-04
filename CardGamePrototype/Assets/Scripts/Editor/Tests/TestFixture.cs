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

            GameSettings.Instance.AiControlledPlayer.Value = AiControlledPlayerValue;
        }
        [SetUp]
        public void Setup()
        {
            AiControlledPlayerValue = GameSettings.Instance.AiControlledPlayer.Value;
            GameSettings.Instance.AiControlledPlayer.Value = true;
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
            if (trait == null)
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

            if (ability)
                TestCreature.SpecialAbility = ability;

            var testCard = new Card(TestCreature);
            AddCardToDeck(playerdeck, testCard);

            return testCard;
        }

        protected Card GenerateTestCreatureWithTrait(string traitName, int attack = 2, int health = 10, bool playerDeck = true)
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

            AddCardToDeck(playerDeck, testCard);

            return testCard;
        }

        private static void AddCardToDeck(bool playerdeck, Card testCard)
        {
            Deck testDeck = null;

            if (playerdeck)
            {
                if (Battle.PlayerDeck == null)
                {
                    var TestDeckObject = new DeckObject()
                    {
                        Creatures = new List<Creature>(),
                    };

                    Battle.SetPlayerDeck(TestDeckObject);
                }

                testDeck = Battle.PlayerDeck;
                testDeck.DeckController = new DeckAI(testDeck,false);
            }
            else
            {
                if (Battle.EnemyDeck == null)
                {
                    var TestDeckObject = new DeckObject()
                    {
                        Creatures = new List<Creature>(),
                    };

                    Battle.EnemyDeck = new Deck(TestDeckObject);
                }

                testDeck = Battle.EnemyDeck;
                testDeck.DeckController = new DeckAI(testDeck,true);
            }

            testDeck.AddCard(testCard);
        }

        protected static void SetObjectIfCorrectAbility<T>(SpecialAbility thisAbility, SpecialAbility otherAb, ref T toBeSet, T value)
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

            if (Battle.PlayerDeck == null)
            {
                var TestDeckObject = new DeckObject()
                {
                    Creatures = new List<Creature>(),
                };

                Battle.SetPlayerDeck(TestDeckObject);

                var ai = new DeckAI(Battle.PlayerDeck,false);

                Battle.PlayerDeck.DeckController = ai;
            }

            testDeck = Battle.PlayerDeck;

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
