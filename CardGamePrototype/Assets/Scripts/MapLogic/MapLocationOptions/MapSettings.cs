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
        [SerializeField]
        public List<MapLocation> EventLocations;
        [SerializeField]
        private List<MapOptionObject> BasicLocations;



        [Header("Generation")]
        [Range(1, 100)]
        public int MapLength;
        [Range(2, 4)]
        public int MinNodesAtStep = 4;
        [Range(4, 16)]
        public int MaxNodesAtStep = 8;

        [Range(-1,6)]
        public int VisibleSteps;//-1 = all visible

        //TODO: turn all of these into a list of MapNodeType to int tuples
        [Range(1, 100)]
        public int HardCombatFrequency;
        [Range(1, 100)]
        public int StandardCombatFrequency;
        [Range(0, 100)]
        public int EventFrequency;
        [Range(1, 100)]
        public int VillageFrequency;
        [Range(1, 100)]
        public int GoldFrequency;
        [Range(1, 100)]
        public int XpFrequency;
        //[Range(0f,1f)]
        //public float ChanceForReconnectingPaths;
        [Range(0,200)]
        public int RandomnessToDifficulty;

        [Range(0.0f,0.95f)]
        public float NonCombatNodeChance = 0.25f;
        public int StepDifficultyIncrease = 20;
        public int StartDifficulty = 30;

        public Race[] CivilizedRaces;
        public Race[] EnemyRaces;

        //public enum LocationType { Event, Village, StandardCombat, HardCombat, Xp, Gold }
        //public struct LocationTypeChance
        //{
        //    public LocationType Type;
        //    [Range(0,10)]
        //    public int Chance;
        //}


        [Range(0f,1f)]
        public float ExtraRoadChance;

        [Header("Map Icons")]
        public Sprite MoneyIcon;
        public Sprite XpIcon;
        public Sprite UnknownNodeIcon;
        public Sprite VillageIcon;
        public Sprite GraveYardIcon;

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

                return combat.MainRace.Icon;

            }
            else if( location is VillageShop)
            {
                //TODO: different icons for different villages?

                if (location is VillageShop shop && shop.Race.name == "Undead")
                    return Instance.GraveYardIcon;

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