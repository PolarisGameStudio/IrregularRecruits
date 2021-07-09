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
            BattleManager.Init();

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

            BattleManager.SetPlayerDeck(TestDeckObject);

            var ai = new AI(BattleManager.Instance.PlayerDeck);

            BattleManager.Instance.PlayerDeck.DeckController = ai;

            var deck = BattleManager.Instance.PlayerDeck;

            deck.Hero = hero;

            hero.InDeck = deck;

        }

        [Test]
        public void CombatOptionStartCombat()
        {
            var option = new CombatOption()
            {
                CRValue = 100
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            var combatStarted = false;

            Event.OnCombatSetup.AddListener((p, e) => combatStarted = true);

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

            var option = new CombatOption()
            {
                MainRace =race ,
                CRValue = 100
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            Deck eDeck = null;

            Event.OnCombatSetup.AddListener((p, e) => eDeck = e);

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

            var option = new CombatOption()
            {
                SpawnCreatures = new List<Creature>() { TestCreature }
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            Deck eDeck = null;

            Event.OnCombatSetup.AddListener((p, e) => eDeck = e);

            node.Open();
            node.SelectOption(0);

            Assert.IsNotNull(eDeck);

            Assert.Contains(TestCreature, eDeck.AllCreatures());

        }

        [Test]
        public void GainGoldAddsGold()
        {
            var amount = 69;

            var option = new GainGoldOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int playerGold = MapController.Instance.PlayerGold;



            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(amount + playerGold, MapController.Instance.PlayerGold);

        }
        
        [Test]
        public void GainXpAddsXp()
        {
            var amount = 69;

            var option = new GainXpOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int xp = BattleManager.Instance.PlayerDeck.Hero.Xp;
                       
            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(amount + xp, BattleManager.Instance.PlayerDeck.Hero.Xp);

        }

        [Test]
        public void LoseGoldRemovesGold()
        {
            var amount = 69;

            var option = new LoseGoldOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int playerGold = 1000;

            MapController.Instance.PlayerGold = playerGold;

            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(playerGold - amount, MapController.Instance.PlayerGold);
        }

        [Test]
        public void LoseXpRemovesXp()
        {
            var amount = 69;

            var option = new LoseXPOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int xp = 1000;

            BattleManager.Instance.PlayerDeck.Hero.AwardXp(xp);
            
            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(xp - amount, BattleManager.Instance.PlayerDeck.Hero.Xp);
        }


        [Test]
        public void LoseGoldNotApplicableWithTooLittleGold()
        {

            var amount = 1001;

            var option = new LoseGoldOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int playerGold = 1000;

            MapController.Instance.PlayerGold = playerGold;

            Assert.IsFalse(node.GetOptions().Contains(option));

            node.Open();
            node.SelectOption(option);

            Assert.
                AreEqual(playerGold , MapController.Instance.PlayerGold);


        }
        [Test]
        public void LoseXPNotApplicableWithTooLittleXp()
        {
            var amount = 69;

            var option = new LoseXPOption()
            {
                Amount = amount
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int xp = 68;

            BattleManager.Instance.PlayerDeck.Hero.AwardXp(xp);

            Assert.IsFalse(node.GetOptions().Contains(option));

            node.Open();
            node.SelectOption(option);

            Assert.
                AreEqual(xp , BattleManager.Instance.PlayerDeck.Hero.Xp);

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
            var giftCreature2 = new Creature()
            {
                name = "GiftPny",
                Attack = 10,
                Health = 500
            };

            var option = new GainUnitOption()
            {
                Units = new List<Creature>()
                {
                    giftCreature,
                    giftCreature2,
                    giftCreature2
                }
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            var units = BattleManager.Instance.PlayerDeck.AllCreatures();

            var count = units.Count;

            //check thát they are not already there
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature));
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature2));

            node.Open();
            node.SelectOption(0);

            var addedUnits = BattleManager.Instance.PlayerDeck.AllCreatures();


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

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            BattleManager.Instance.PlayerDeck.AddCard(new Card(giftCreature));

            var units = BattleManager.Instance.PlayerDeck.AllCreatures();
            
            var count = units.Count;

            //check thát there is only the one to randomly select
            Assert.AreEqual(1, count);
            Assert.IsTrue(units.Any(c => c.Creature == giftCreature));
            Assert.IsFalse(units.Any(c => c.Creature == giftCreature2));

            node.Open();
            node.SelectOption(0);

            var removedUnits = BattleManager.Instance.PlayerDeck.AllCreatures();

            Assert.AreEqual(count -1, removedUnits.Count);
            Assert.IsFalse(removedUnits.Any(c => c.Creature == giftCreature));

        }

        [Test]
        public void OptionsDoesNotContainIncorrectHeroRaceOptions()
        {
            var option1 = new GainGoldOption()
            {
                Amount = 1000,
                OnlyForHeroRaces = new List<Race>() { new Race()
                {
                    name = "richRace"
                } }
            };
            var option3 = new LoseGoldOption()
            {
                Amount = 50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option1,  option3 }
            };

            var node = new MapNode(location);

            node.Open();

            Assert.Contains(option3, node.GetOptions());
            Assert.IsFalse(node.GetOptions().Contains(option1));

        }
        [Test]
        public void OptionsContainsCorrectHeroRaceOptions()
        {
            var option2 = new LoseGoldOption()
            {
                Amount = 17,
                OnlyForHeroRaces = new List<Race>() { BattleManager.Instance.PlayerDeck.Hero.GetRace() }

            };
            var option3 = new LoseGoldOption()
            {
                Amount = 50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option2,option3 }
            };

            var node = new MapNode(location);

            node.Open();

            Assert.Contains(option3, node.GetOptions());
            Assert.Contains(option2, node.GetOptions());

        }

        [Test]
        public void OptionsDoesNotContainIncorrectAbilityOptions()
        {
            var option1 = new GainGoldOption()
            {
                Amount = 1000,
                OnlyForAbility = new List<AbilityWithEffect>() { new PassiveAbility()
                {
                    name = "ability"
                } }
            };
            var option3 = new LoseGoldOption()
            {
                Amount = 50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option1, option3 }
            };

            var node = new MapNode(location);

            node.Open();

            Assert.Contains(option3, node.GetOptions());
            Assert.IsFalse(node.GetOptions().Contains(option1));

        }

        [Test]
        public void OptionsContainsCorrectAbilityOptions()
        {
            var option1 = new GainGoldOption()
            {
                Amount = 1000,
                OnlyForAbility = new List<AbilityWithEffect>() { BattleManager.Instance.PlayerDeck.Hero.Abilities.FirstOrDefault() }
            };
            var option3 = new LoseGoldOption()
            {
                Amount = 50
            };


            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option1, option3 }
            };

            var node = new MapNode(location);

            node.Open();

            Assert.Contains(option3, node.GetOptions());
            Assert.Contains(option1, node.GetOptions());

        }

        [Test]
        public void CompositeOptionAllExecutes()
        {
            var option1 = new GainGoldOption()
            {
                Amount = 1000
            };
            var option2 = new LoseGoldOption()
            {
                Amount = 17
            };

            var composite = new MapOptionComposite()
            {
                Options = new List<MapOption>()
                {
                    option1,option2,option2,option2
                }
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { composite, option1 }
            };

            var node = new MapNode(location);

            int playerGold = MapController.Instance.PlayerGold;

            node.Open();
            node.SelectOption(0);

            Assert.
                AreEqual(option1.Amount - 3 * option2.Amount + playerGold, MapController.Instance.PlayerGold);



        }

        [Test]
        public void CompositeOptionNotPossibleIfOneIsNotPossible()
        {
            var option1 = new GainGoldOption()
            {
                Amount = 1000,
                OnlyForHeroRaces = new List<Race>() { new Race()
                {
                    name = "testrace"
                } }
            };
            var option2 = new LoseGoldOption()
            {
                Amount = 17

            };

            var composite = new MapOptionComposite()
            {
                Options = new List<MapOption>()
                {
                    option2,option1,option2,option2
                }
            };

            Assert.IsFalse(composite.IsApplicable());

        }

        [Test]
        public void CloseBoolClosesMapLocation()
        {
            var option = new LoseGoldOption()
            {
                Amount = 1,
                ClosesLocationOnSelection = true
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);
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


            var option = new LoseGoldOption()
            {
                Amount = 1,
                ClosesLocationOnSelection = false
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);
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
            var controller = new MapController();

            MapSettings settings = MapSettings.Instance;
            settings.MapLength = 5;

            controller.CreateMap();

            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsStartNode()));
            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsFinalNode()));

            var startNode = controller.Nodes.First(n => n.IsStartNode());

            Assert.IsTrue(NodeEndsInWinRecursiveCheck(startNode));

        }

        [Test]
        public void CanTraverseABigMap()
        {
            var controller = new MapController();

            MapSettings settings = MapSettings.Instance;
            settings.MapLength = 30;

            controller.CreateMap();

            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsStartNode()));
            Assert.AreEqual(1, controller.Nodes.Count(n => n.IsFinalNode()));

            var startNode = controller.Nodes.First(n => n.IsStartNode());

            Assert.IsTrue(NodeEndsInWinRecursiveCheck(startNode));

        }

    }
}
