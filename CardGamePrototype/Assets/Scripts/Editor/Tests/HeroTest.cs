using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public partial class AbilityTest
    {
        public class HeroTest
        {

            private Creature TestCreature;
            private Card TestCard;
            private Card OtherCard;


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

            [Test]
            public void AbilityWorks() {
                Assert.IsTrue(false);
            }

            [Test]
            public void ActiveAbilityWorks() {

                Assert.IsTrue(false);
            }

            [Test]
            public void ActiveAbilityUsesAction()
            {
                Assert.IsTrue(false);
            }

            [Test] public void ThisRaceTriggers()
            {
                Assert.IsTrue(false);
            }

            [Test] public void EnemyTrigger()
            {
                Assert.IsTrue(false);
            }

            [Test] 
            public void EnemyDoesNotTriggerOnFriend()
            {
                Assert.IsTrue(false);
            }

            [Test] 
            public void ActiveAbilityDoesNotTriggerWhenNoActionsLeft()
            {
                Assert.IsTrue(false);
            }
        }
    }
}
