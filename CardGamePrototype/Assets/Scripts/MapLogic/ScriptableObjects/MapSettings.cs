using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MapLogic
{
    [CreateAssetMenu]
    public class MapSettings : SingletonScriptableObject<MapSettings>
    {
        public List<MapLocation> EventLocations;
        public List<MapOption> BasicLocations;


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

        [Header("Map Icons")]
        public Sprite MoneyIcon;
        public Sprite XpIcon;
        public Sprite UnknownNodeIcon;
        public Sprite VillageIcon;
        public RaceIcon[] CombatIcons;
        
        [Serializable]
        public struct RaceIcon { public Race Race; public Sprite Icon; }

        public static Sprite GetLocationIcon(IMapLocation location)
        {
            if (location is MapLocation)
                return Instance.UnknownNodeIcon;

            if (location is GainGoldOption)
                return Instance.MoneyIcon;

            if (location is GainXpOption)
                return Instance.XpIcon;

            if(location is CombatOption)
            {
                var combat = location as CombatOption;

                if (Instance.CombatIcons.Any(c => c.Race == combat.MainRace))
                    return Instance.CombatIcons.Single(c => c.Race == combat.MainRace).Icon;     
                else

                    return Instance.CombatIcons.First().Icon;
            }
            if(location is HireUnitOption)
            {
                return Instance.VillageIcon;
            }

            return Instance.UnknownNodeIcon;
        }
        public static string GetLocationInfo(IMapLocation location)
        {
            return "someplace nice, maybe?";
        }
    }
}