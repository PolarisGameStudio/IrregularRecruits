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
        public int Id;
        public bool Visited;
        public bool Active;
        public readonly IMapLocation Location;
        public string name;
        public Dictionary<MapOption, List<Card>> SelectedCards = new Dictionary<MapOption, List<Card>>();

        //REcursive check
        public bool CanReach(MapNode node)
        {
            if (LeadsTo.Contains(node)) return true;

            return LeadsTo.Any(n => n.CanReach(node));

        }

        public class LocationEvent : UnityEvent<MapNode> { }
        public static LocationEvent OpenEvent = new LocationEvent();
        public static LocationEvent CloseLocation = new LocationEvent();

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
            return Location.GetLocationOptions().Where(o => o.IsApplicable()).ToArray();
        }

        public void Open()
        {
            Visited = Active = true;

            foreach (var option in Location.GetLocationOptions())
            {
                option.FindCandidate(this);
            }

            Location.Open(this);
        }

        public void AddAssociation(MapOption option, Card unit)
        {
            if (!SelectedCards.ContainsKey(option))
                SelectedCards.Add(option, new List<Card>() );
            SelectedCards[option].Add(unit);
        }

        public void SelectOption(int i)
        {
            if (i >= Location.GetLocationOptions().Length)
                return;

            MapOption mapOption = Location.GetLocationOptions()[i];
            SelectOption(mapOption);
        }

        public bool SelectOption(MapOption mapOption)
        {
            if (!Location.GetLocationOptions().Contains(mapOption) || !mapOption.IsApplicable())
                return false;

            mapOption.ExecuteOption(this);

            if (mapOption.ClosesLocationOnSelection)
            {
                Active = false;
                CloseLocation.Invoke(this);
            }

            return true;
        }

        public bool IsFinalNode() => Location is MapLocation && (Location as MapLocation).WinNode;
        public bool IsStartNode() => Location is MapLocation && (Location as MapLocation).StartNode;

    }
}