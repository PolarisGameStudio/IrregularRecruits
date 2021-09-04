using GameLogic;
using MapLogic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{

    public class MapTest : TestFixture
    {
        [SetUp]
        public void PlayerDeckSetup()
        {
            var hero = new Hero(new HeroObject()
            {
                name = "Testeron",
                StartingAbility = new PassiveAbility()
                {
                    Name = "HeRoBility"
                },
                Race = new Race()
                {
                    name = "HeroRace"
                },
            });

            var TestDeckObject = new DeckObject()
            {
                Creatures = new List<Creature>(),
            };

            Battle.SetPlayerDeck(TestDeckObject);

            var ai = new DeckAI(Battle.PlayerDeck,false);

            Battle.PlayerDeck.DeckController = ai;

            var deck = Battle.PlayerDeck;

            deck.Hero = hero;

            hero.InDeck = deck;

        }

        [Test]
        public void CombatOptionStartCombat()
        {
            var option = new MapOptionObject()
            {
                CRValue = 100,
                StartsCombat = true
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());

            var combatStarted = false;

            Event.OnCombatStart.AddListener(() => combatStarted = true);

            node.Open();
            node.SelectOption(0);

            Assert.IsTrue(combatStarted);
        }

        [Test]
        public void CombatOptionSpawnsCorrectRandomEnemyTypes()
        {
            Race race = new Race
            {
                name = "TestRace"
            };
            Race unusedRace = new Race
            {
                name = "TestRace2"
            };

            //add creatures to assetmanager

            var option = new MapOptionObject()
            {
                MainRace =race ,
                CRValue = 100,
                StartsCombat = true
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());

            Deck eDeck = null;

            Event.OnCombatStart.AddListener(() => eDeck = Battle.EnemyDeck);

            node.Open();
            node.SelectOption(0);

            Assert.IsNotNull(eDeck);
            Assert.IsTrue(eDeck.AllCreatures().All(c => c.GetRace() == race));
        }


        [Test]
        public void CombatOptionSpawnsCorrectSpecifiedEnemies()
        {
            var TestCreature = new Creature()
            {
                name = "Tester",
                Attack = 1,
                Health = 7
            };

            var option = new MapOptionObject()
            {
                SpawnCreatures = new List<Creature>() { TestCreature },
                StartsCombat = true
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());

            Deck eDeck = null;

            Event.OnCombatStart.AddListener(() => eDeck = Battle.EnemyDeck);

            node.Open();
            node.SelectOption(0);

            Assert.IsNotNull(eDeck);

            Assert.Contains(TestCreature, eDeck.AllCreatures());

        }

        [Test]
        public void GainGoldAddsGold()
        {
            var amount = 69;

            var option = new MapOptionObject()
            {
                GoldAmount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());

            int playerGold = Map.PlayerGold;



            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(amount + playerGold, Map.PlayerGold);

        }
        
        [Test]
        public void GainXpAddsXp()
        {
            var amount = 69;

            var option = new GainXpOption(amount);

            var node = new MapNode(option, new Map());

            int xp = Battle.PlayerDeck.Hero.Xp;
                       
            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(amount + xp, Battle.PlayerDeck.Hero.Xp);

        }

        [Test]
        public void LoseGoldRemovesGold()
        {
            var amount = 69;

            var option = new LoseGoldOption(amount);

            var node = new MapNode(option, new Map());

            int playerGold = 1000;

            Map.PlayerGold = playerGold;

            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(playerGold - amount, Map.PlayerGold);
        }

        [Test]
        public void LoseXpRemovesXp()
        {
            var amount = 69;

            var option = new LoseXPOption(amount);

            var node = new MapNode(option, new Map());

            int xp = 1000;

            Battle.PlayerDeck.Hero.AwardXp(xp);
            
            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(xp - amount, Battle.PlayerDeck.Hero.Xp);
        }


        [Test]
        public void LoseGoldNotApplicableWithTooLittleGold()
        {

            var amount = 1001;

            var option = new LoseGoldOption(amount);

            var node = new MapNode(option, new Map());

            int playerGold = 1000;

            Map.PlayerGold = playerGold;

            Assert.IsFalse(node.GetOptions().Contains(option));

            node.Open();
            node.SelectOption(option);

            Assert.
                AreEqual(playerGold , Map.PlayerGold);


        }
        [Test]
        public void LoseXPNotApplicableWithTooLittleXp()
        {
            var amount = 69;

            var option = new LoseXPOption(amount);

            var node = new MapNode(option, new Map());

            int xp = 68;

            Battle.PlayerDeck.Hero.AwardXp(xp);

            Assert.IsFalse(node.GetOptions().Contains(option));

            node.Open();
            node.SelectOption(option);

            Assert.
                AreEqual(xp , Battle.PlayerDeck.Hero.Xp);

        }

        [Test]
        public void GainUnitAddsUnit()
        {
            var giftCreature = new Creature()
            {
                name = "GiftHorse",
                Attack = 1000,
                Health = 700000
            };
#pragma warning disable UNT0011 // ScriptableObject instance creation
            var giftCreature2 = new Creature()
            {
                name = "GiftPny",
                Attack = 10,
                Health = 500
            };
#pragma warning restore UNT0011 // ScriptableObject instance creation

            var option = new GainUnitOption(new List<Creature>()
                {
                    giftCreature,
                    giftCreature2,
                    giftCreature2
                });

            var node = new MapNode(option, new Map());

            var units = Battle.PlayerDeck.AllCreatures();

            var count = units.Count;

            //check thát they are not already there
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature));
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature2));

            node.Open();
            node.SelectOption(0);

            var addedUnits = Battle.PlayerDeck.AllCreatures();


            Assert.AreEqual(count + 3, addedUnits.Count);
            Assert.IsTrue(addedUnits.Any(c => c.Creature == giftCreature));
            Assert.IsTrue(addedUnits.Any(c => c.Creature == giftCreature2));

        }

        [Test]
        public void LoseUnitRemovesUnit()
        {

            var giftCreature = new Creature()
            {
                name = "Boris Johnsen",
                Attack = 1000,
                Health = 700000
            };
            var giftCreature2 = new Creature()
            {
                name = "Isabeel",
                Attack = 10,
                Health = 500
            };

            var option = new LoseUnitOption()
            {
                //TODO: test each of these
                AssociatedUnit = MapOption.UnitCandidate.Random
            };

            var node = new MapNode(option, new Map());

            Battle.PlayerDeck.AddCard(new Card(giftCreature));

            var units = Battle.PlayerDeck.AllCreatures();
            
            var count = units.Count;

            //check thát there is only the one to randomly select
            Assert.AreEqual(1, count);
            Assert.IsTrue(units.Any(c => c.Creature == giftCreature));
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature2));

            node.Open();
            node.SelectOption(0);

            var removedUnits = Battle.PlayerDeck.AllCreatures();

            Assert.AreEqual(count -1, removedUnits.Count);
            Assert.IsFalse(removedUnits.Any(c => c.Creature == giftCreature));

        }

        [Test]
        public void OptionsDoesNotContainIncorrectHeroRaceOptions()
        {
            var option1 = new MapOptionObject()
            {
                GoldAmount = 1000,
                OnlyForHeroRaces = new List<Race>() { new Race()
                {
                    name = "richRace"
                } }
            };
            var option3 = new MapOptionObject()
            {
                GoldAmount = -50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option1,  option3 }
            };

            var node = new MapNode(location, new Map());

            node.Open();

            Assert.IsTrue(node.GetOptions().Any(o=>o is LoseGoldOption));
            Assert.IsFalse(node.GetOptions().Any(o=> o is GainGoldOption));

        }
        [Test]
        public void OptionsContainsCorrectHeroRaceOptions()
        {
            var option2 = new MapOptionObject()
            {
                GoldAmount = 17,
                OnlyForHeroRaces = new List<Race>() { Battle.PlayerDeck.Hero.GetRace() }

            };
            var option3 = new MapOptionObject()
            {
                GoldAmount = -50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option2,option3 }
            };

            var node = new MapNode(location, new Map());

            node.Open();

            Assert.IsTrue(node.GetOptions().Any(o => o is LoseGoldOption));
            Assert.IsTrue(node.GetOptions().Any(o => o is GainGoldOption));

        }

        [Test]
        public void OptionsDoesNotContainIncorrectAbilityOptions()
        {
            var option1 = new MapOptionObject()
            {
                GoldAmount = 1000,
                OnlyForAbility = new List<AbilityWithEffect>() { new PassiveAbility()
                {
                    name = "ability"
                } }
            };
            var option3 = new MapOptionObject()
            {
                GoldAmount= -50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option1, option3 }
            };

            var node = new MapNode(location,new Map());

            node.Open();

            Assert.IsTrue(node.GetOptions().Any(o => o is LoseGoldOption));
            Assert.IsFalse(node.GetOptions().Any(o => o is GainGoldOption));

        }

        [Test]
        public void OptionsContainsCorrectAbilityOptions()
        {
            var option1 = new MapOptionObject()
            {
                GoldAmount= 1000,
                OnlyForAbility = new List<AbilityWithEffect>() { Battle.PlayerDeck.Hero.Abilities.FirstOrDefault() }
            };
            var option3 = new MapOptionObject()
            {
                GoldAmount = -50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option1, option3 }
            };

            var node = new MapNode(location, new Map());

            node.Open();

            Assert.IsTrue(node.GetOptions().Any(o => o is LoseGoldOption));
            Assert.IsTrue(node.GetOptions().Any(o => o is GainGoldOption));
        }

        [Test]
        public void CompositeOptionAllExecutes()
        {
            var option1 = new GainGoldOption(1000);

            var option2 = new LoseGoldOption(17);

            var composite = new MapOptionComposite (
                new List<MapOption>()
                {
                    option1,option2,option2,option2
                });

            var node = new MapNode(composite, new Map());

            int playerGold = Map.PlayerGold;

            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(option1.Amount + 3 * option2.Amount + playerGold, Map.PlayerGold);

        }

        [Test]
        public void CompositeOptionNotPossibleIfOneIsNotPossible()
        {
            var option1 = new GainGoldOption(1000)
            {
                OnlyForHeroRaces = new List<Race>() { new Race()
                {
                    name = "testrace"
                } }
            };
            var option2 = new LoseGoldOption(17);

            var composite = new MapOptionComposite (new List<MapOption>()
                {
                    option2,option1,option2,option2
                });

            Assert.IsFalse(composite.IsApplicable());

        }

        [Test]
        public void CloseBoolClosesMapLocation()
        {
            var option = new MapOptionObject()
            {
                GoldAmount = 1,
                ClosesLocationOnSelection = true
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());
            MapNode closed = null;


            MapNode.CloseLocation.AddListener(m => closed = m);

            node.Open();
            node.SelectOption(0);


            Assert.IsNotNull(closed);
            Assert.AreEqual(node, closed);

        }

        [Test]
        public void CloseNotBoolDoesNotClosesMapLocation()
        {


            var option = new MapOptionObject()
            {
                GoldAmount = 1,
                ClosesLocationOnSelection = false
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOptionObject[] { option }
            };

            var node = new MapNode(location, new Map());
            MapNode closed = null;

            MapNode.CloseLocation.AddListener(m => closed = m);

            node.Open();
            node.SelectOption(0);


            Assert.IsNull(closed);

        }

        private bool NodeEndsInWinRecursiveCheck(MapNode node)
        {
            if (node.IsFinalNode()) return true;

            return node.LeadsTo.All(NodeEndsInWinRecursiveCheck);
        }

        [Test]
        public void CanTraverseAMap()
        {
            MapSettings settings = MapSettings.Instance;
            settings.MapLength = 5;

            var controller = new Map();

            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsStartNode()));
            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsFinalNode()));

            var startNode = controller.Nodes.First(n => n.IsStartNode());

            Assert.IsTrue(NodeEndsInWinRecursiveCheck(startNode));

        }

        [Test]
        public void CanTraverseABigMap()
        {
            MapSettings settings = MapSettings.Instance;
            settings.MapLength = 30;

            var controller = new Map();

            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsStartNode()));
            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsFinalNode()));

            var startNode = controller.Nodes.First(n => n.IsStartNode());

            Assert.IsTrue(NodeEndsInWinRecursiveCheck(startNode));

        }

    }
}
