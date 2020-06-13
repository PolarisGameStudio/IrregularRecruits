using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu]
    public class MapSettings : SingletonScriptableObject<MapSettings>
    {
        public List<MapLocation> LocationObjects;
        public int MapLength;
        public int MaxRoadsFromNode = 4;
        //Has to be bigger than max roads from node
        public int MaxNodesAtStep = 8;


    }
}