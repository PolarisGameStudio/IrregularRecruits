using GameLogic;
using MapLogic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class MapTest
    {
        [Test]
        public void CombatOptionStartCombat()
        {
            var option = new CombatOption() {
                CRValue = 100
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            var combatStarted = false;

            node.Open();
            node.SelectOption(0);

            Event.OnCombatSetup.AddListener((p, e) => combatStarted = true);

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
                PossibleRaces = new List<Race>() { race },
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
        public void CombatOptionSpawnsCorrectRandomCREnemies()
        {

            Assert.IsTrue(false);

        }
        [Test]
        public void CombatOptionSpawnsCorrectSpecifiedEnemies()
        {
            var TestCreature = new Creature()
            {
                name = "Tester" ,
                Attack = 1,
                Health = 7
            };

            var option = new CombatOption()
            {
                SpawnCreatures = new List<Creature>() { TestCreature}
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
            var option = new GainGoldOption()
            {
                Amount = 69
            };

            var location = new MapLocation()
            {
                LocationOptions = new MapOption[] { option }
            };

            var node = new MapNode(location);

            int playerGold = 0;

            node.Open();
            node.SelectOption(0);

            Assert.IsTrue(false);

        }

        [Test]
        public void LoseGoldRemovesGold()
        {
            Assert.IsTrue(false);

        }
        
        [Test]
        public void GainUnitAddsUnit()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void LoseUnitRemovesUnit()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void LoseGoldNotPossibleWithTooLittleGold()
        {
            Assert.IsTrue(false);

        }
        [Test]
        public void OptionsDoesNotContainIncorrectHeroRaceOptions()
        {
            Assert.IsTrue(false);

        }
        [Test]
        public void OptionsContainsCorrectHeroRaceOptions()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void OptionsDoesNotContainIncorrectAbilityOptions()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void OptionsContainsCorrectAbilityOptions()
        {
            Assert.IsTrue(false);

        }
        [Test]
        public void OptionsDoesNotContainIncorrectDeckOptions()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void OptionsContainsCorrectDeckOptions()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void CompositeOptionAllExecutes()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void CompositeOptionNotPossibleIfOneIsNotPossible()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void CloseBoolClosesMapLocation()
        {
            Assert.IsTrue(false);

        }

        [Test]
        public void CloseNotDoesNotClosesMapLocation()
        {
            Assert.IsTrue(false);

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

            Assert.AreEqual(1,controller.Nodes.Count(n => n.IsStartNode()));
            Assert.AreEqual(1,controller.Nodes.Count(n => n.IsFinalNode()));

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
