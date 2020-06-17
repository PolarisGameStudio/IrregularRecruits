using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MapLogic
{
    public class MapNode
    {
        public List<MapNode> LeadsTo = new List<MapNode>();
        public bool Visited;
        public bool Active;
        public readonly MapLocation Location;
        public string name;
        public Dictionary<MapOption, Card> SelectedCards = new Dictionary<MapOption, Card>();

        public class LocationEvent : UnityEvent<MapNode> { }
        public static LocationEvent OpenLocationEvent = new LocationEvent();
        public static LocationEvent CloseLocationEvent = new LocationEvent();

        //public List<MapOption> SelectedOptions = new List<MapOption>();


        public MapNode(MapLocation mapLocation)
        {
            this.Location = mapLocation;
            name = mapLocation.name + " " + Guid.NewGuid();
        }

        public void Visit()
        {
            Visited = true;
        }

        public MapOption[] GetOptions()
        {
            return Location.LocationOptions.Where(o=>o.IsApplicable()).ToArray();
        }

        public void Open()
        {
            Visited = Active =true;

            foreach (var option in Location.LocationOptions)
            {
                FindCandidate(option);
            }

            OpenLocationEvent.Invoke(this);
        }

        private void FindCandidate(MapOption option)
        {
            Func<Card, bool> predicate;

            switch (option.AssociatedUnit)
            {
                case MapOption.UnitCandidate.NoUnit:
                    return;
                case MapOption.UnitCandidate.Strong:
                case MapOption.UnitCandidate.Weak:
                case MapOption.UnitCandidate.Random:
                case MapOption.UnitCandidate.FriendlyRace:
                case MapOption.UnitCandidate.NonFriendlyRace:
                default:
                    {
                        predicate = d => !SelectedCards.Values.Contains(d);

                        break;
                    }
            }

            var unit = BattleManager.Instance.PlayerDeck.AllCreatures().FirstOrDefault(predicate);

            if (unit == null)
                unit = BattleManager.Instance.PlayerDeck.AllCreatures().FirstOrDefault();

            if (unit != null)
                SelectedCards.Add(option, unit);

        }

        public void SelectOption(int i)
        {
            if (i >= Location.LocationOptions.Length)
                return;

            MapOption mapOption = Location.LocationOptions[i];

            mapOption.ExecuteOption(this);

            if (mapOption.ClosesLocationOnSelection)
            {
                Active = false;
                CloseLocationEvent.Invoke(this); 
            }
        }


        public bool IsFinalNode() => Location.WinNode;
        public bool IsStartNode() => Location.StartNode;

        private bool OptionApplicable(MapOption mapOption)
        {
            return mapOption.OnlyForHeroRaces.Count == 0 //|| mapOption.OnlyForHeroRaces.Contains(GameManager.)
                &&
                mapOption.OnlyForAbility.Count == 0 //|| mapOption.OnlyForAbility.Contains
                ;
        }
    }
}