using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu]
    public class MapSettings : SingletonScriptableObject<MapSettings>
    {
        public List<MapLocation> LocationObjects;

        [Range(1, 100)]
        public int MapLength;
        [Range(2, 4)]
        public int MinNodesAtStep = 4;
        //Has to be bigger than max roads from node
        [Range(4, 16)]
        public int MaxNodesAtStep = 8;
        [Range(-1,4)]
        public int VisibleSteps;//-1 = all visible
        [Range(0f,1f)]
        public float HiddenNodesPct;
        //[Range(0f,1f)]
        //public float ChanceForReconnectingPaths;
        [Range(0f,200f)]
        public float RandomnessToDifficulty;

        [Range(0f,1f)]
        public float ExtraRoadChance;
    }
}