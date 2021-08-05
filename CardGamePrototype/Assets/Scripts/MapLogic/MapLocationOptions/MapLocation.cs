using System;
using System.Collections;
using System.Linq;
using UnityEngine;


namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Location", order = 0)]
    public class MapLocation : ScriptableObject,IMapLocation
    {
        public string Name { get; set; }
        public string PopUpDescription { get ; set; }

        public int Difficulty;
        public bool ExecuteRandomCombatOptionAutomatically;
        public bool WinNode;
        public bool StartNode;
        //only once pr. game
        public bool UniqueNode;
        //0 = all options. 
        [TextArea]
        public string LocationDescription;
        public Sprite LocationIcon;
        public Sprite LocationImage;
        public MapOptionObject[] LocationOptions;
        internal MapOption[] LocationOptionsInstantiated;

        private void OnEnable()
        {
            LocationOptionsInstantiated = null;
        }

        public MapOption[] GetLocationOptions()
        {
            if(LocationOptionsInstantiated == null)
            {
                LocationOptionsInstantiated = LocationOptions.Select(loc => loc.InstantiateMapOption()).ToArray();
            }

            return LocationOptionsInstantiated;

        }

        public bool IsStartNode()
        {
            return StartNode;
        }

        public bool IsUniqueNode()
        {
            return true;
        }

        public bool IsWinNode()
        {
            return WinNode;
        }

        public void Open(MapNode node)
        {
            MapNode.OpenEvent.Invoke(node);
        }

        float IMapLocation.Difficulty()
        {
            return Difficulty;
        }
    }
}