using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MapLogic
{
    public class MapNode 
    {
        public List<MapNode> LeadsTo = new List<MapNode>();
        public bool Visited;        
        private readonly MapLocation Location;
        public string name;

        public class LocationEvent : UnityEvent<MapNode> { }
        public static LocationEvent OpenLocationEvent = new LocationEvent();
        public static LocationEvent CloseLocationEvent = new LocationEvent();
        //public List<MapOption> SelectedOptions = new List<MapOption>();


        public MapNode(MapLocation mapLocation)
        {
            this.Location = mapLocation;
            name = mapLocation.name +" "+ Guid.NewGuid();
        }

        public void Visit()
        {
            Visited = true;
        }

        public MapOption[] GetOptions()
        {
            return Location.LocationOptions;
        }

        public void Open()
        {
            Visited = true;

            OpenLocationEvent.Invoke(this);
        }

        public void SelectOption(int i )
        {
            if (i >= Location.LocationOptions.Length)
                return;

            MapOption mapOption = Location.LocationOptions[i];

            mapOption.ExecuteOption(Location);

            if (mapOption.ClosesLocationOnSelection)
                CloseLocationEvent.Invoke(this);
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