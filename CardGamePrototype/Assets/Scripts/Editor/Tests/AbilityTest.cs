using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
namespace Tests
{
    public  class AbilityTest
    {
        private Creature TestCreature;
        private Card TestCard;
        private Card OtherCard;


        [TearDown]
        public void CleanAbility()
        {
            //if (TestCard != null && TestCard.Creature?.SpecialAbility)
            //{
            //    TestCard.Creature.SpecialAbility.RemoveListeners();
            //}
            //if (OtherCard != null && OtherCard.Creature?.SpecialAbility)
            //{
            //    OtherCard.Creature.SpecialAbility.RemoveListeners();
            //}

            AbilityWithEffect.AbilityStackCount = 0;

            Event.ResetEvents();
            BattleManager.Instance.PackUp(null);
        }

        private Card GenerateTestCreature(PassiveAbility ability, Race race = null,bool playerdeck = true)
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

                    BattleManager.SetPlayerDeck(TestDeckObject);
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
        private Creature GenerateSummon()
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

        private void SetObjectIfCorrectAbility<T>(AbilityWithEffect thisAbility, AbilityWithEffect otherAb, ref T toBeSet, T value)
        {
            if (thisAbility == otherAb)
                toBeSet = value;
        }


        [Test]
        public void TriggerPlayTriggersThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.ETB),
            };

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Hand);

            AbilityWithEffect triggeredAblity = null;

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

            TestCard = GenerateTestCreature(testAbility);
            var other = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            other.ChangeLocation(Deck.Zone.Hand);

            AbilityWithEffect triggeredAblity = null;

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

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.StatModifier(50);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killer = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnKill.AddListener((c) => killer = c);

            TestCard.AttackCard(OtherCard);

            Assert.IsTrue(triggered);
            Assert.NotNull(killer);
            Assert.AreEqual(killer,TestCard);
        }
        [Test]
        public void TriggerKillsTriggersOnBeingAttacked()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.KILLS),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.StatModifier(50);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killer = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnKill.AddListener((c) => killer = c);

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
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card death = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnDeath.AddListener((c) => death = c);

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

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Library);

            AbilityWithEffect triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.PlayCard();



            Assert.IsFalse(triggeredAblity == testAbility);
        }


        [Test]
        public void TriggerDamageTriggers()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);

            AbilityWithEffect triggeredAblity = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => SetObjectIfCorrectAbility(testAbility, a, ref triggeredAblity, a));

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggeredAblity == testAbility);
        }

        [Test]
        public void NounsTargetsCharacterThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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
        public void NounsTargetsCharacterOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.One, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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
        public void NounsTargetsCharacterAny()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.Two, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);


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
        public void OwnerOnHandDoesNotTriggerAbility()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.AreEqual(OtherCard.MaxHealth - 1, OtherCard.CurrentHealth);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void NounsTargetsCharacterItWhenOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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
        public void NounsTargetsCharacterItWhenThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

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
        public void NounsTargetsRaceThis()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, race);

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
        public void NounsTargetsRaceSameButNoTargets()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

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
        public void NounsTargetsRaceOther()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            List<Card> targettedCards = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => targettedCards = ts);

            TestCard.HealthChange(-1);

            Assert.IsTrue(TestCard.Alive());

            Assert.IsNotNull(targettedCards);
            Assert.IsTrue(targettedCards.Count == 1);
            Assert.IsTrue(targettedCards.Contains(OtherCard));
        }
        [Test]
        public void NounsTriggersCharacterThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered  = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void NounsTriggersCharacterThisNotOnOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered  = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void NounsTriggersCharacterOther()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void NounsTriggersCharacterOtherNotOnThis()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void NounsTriggersCharacterAny()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            int triggered = 0;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered++);

            OtherCard.HealthChange(-1);
            TestCard.HealthChange(-1);

            Assert.AreEqual(triggered, 2);
        }
        [Test]
        public void NounsTriggersNotCharacterIt()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.It), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);            
            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }

        [Test]
        public void NounsTriggersRaceThis()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void NounsTriggersRaceOther()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
        }
        [Test]
        public void NounsTriggersRaceThisNotOnOther()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, otherRace);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void NounsTriggersRaceOtherNotOnThis()
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

            TestCard = GenerateTestCreature(testAbility, race);
            OtherCard = GenerateTestCreature(null, race);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsFalse(triggered);
        }
        [Test]
        public void ActionWithdrawExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card withdrawn = null;

            Event.OnWithdraw.AddListener(c => withdrawn = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(withdrawn);
        }
        [Test]
        public void ActionCharmExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Charm, Count.All, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null,null,false);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            Assert.AreNotEqual(TestCard.InDeck, OtherCard.InDeck);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(TestCard.InDeck, OtherCard.InDeck);
        }

        [Test]
        public void ActionDealDamageExecutes()
        {
            int dmg = 1;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.All, dmg, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Library), TriggerType.Withdraw),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card damaged = null;

            Event.OnDamaged.AddListener((c) => damaged = c);
            Event.OnWithdraw.AddListener((c) => triggered = true);

            OtherCard.Withdraw();

            Assert.IsTrue(triggered);
            Assert.IsNotNull(damaged);
            Assert.AreEqual(damaged, TestCard);
            Assert.AreEqual(TestCard.MaxHealth - dmg, TestCard.CurrentHealth);

        }
        [Test]
        public void ActionDrawExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Draw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Library);

            bool triggered = false;
            Card drawn = null;

            Event.OnDraw.AddListener(c => drawn = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(drawn);
            Assert.AreEqual(drawn.Location, Deck.Zone.Hand);
        }
        [Test]
        public void ActionHealExecutes()
        {
            int amount = 1;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.All, amount, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card healed = null;

            Event.OnHealed.AddListener((c, i) => healed = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-amount);

            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.MaxHealth == OtherCard.CurrentHealth);
            Assert.IsNotNull(healed);
        }
        [Test]
        public void ActionKillExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Kill, Count.All, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killed = null;

            Event.OnDeath.AddListener(c => killed = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(killed, OtherCard);
            Assert.IsFalse(OtherCard.Alive());
        }
        [Test]
        public void ActionResurrectExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Graveyard)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card death = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnDeath.AddListener((c) => death = c);

            OtherCard.Die();

            Assert.NotNull(death);
            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.Alive());
        }
        [Test]
        public void TriggerThisDiesTriggers()
        {

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.Die();

            Assert.IsTrue(triggered);
            Assert.IsTrue(TestCard.Alive());
        }

        [Test]
        public void ActionStatminusExecutes()
        {
            const int amount = 2;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.StatMinus, Count.One, amount, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var expectedAttack = OtherCard.Attack - amount;
            var expectedHealth = OtherCard.CurrentHealth - amount;
            var expectedMaxHealth = OtherCard.MaxHealth- amount;

            bool triggered = false;


            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(expectedAttack, OtherCard.Attack);
            Assert.AreEqual(expectedHealth, OtherCard.CurrentHealth);
            Assert.AreEqual(expectedMaxHealth, OtherCard.MaxHealth);

        }
        
        [Test]
        public void ActionSummonExecutes()
        {
            var summon = GenerateSummon();

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Summon, Count.All, 1, new Noun(Noun.CharacterTyp.Any),summon),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreature(testAbility);
            OtherCard = GenerateTestCreature(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card summoned = null;

            Event.OnSummon.AddListener(c => summoned = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(summoned);
            Assert.AreEqual(summoned.Creature, summon);
        }
    }
}
