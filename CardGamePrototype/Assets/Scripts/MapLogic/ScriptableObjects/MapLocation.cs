using System.Collections;
using UnityEngine;


namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Location", order = 0)]
    public class MapLocation : ScriptableObject
    {
        public string Name;
        public int Difficulty;
        public bool ExecuteRandomCombatOptionAutomatically;
        public bool WinNode;
        public bool StartNode;
        //only once pr. game
        public bool UniqueNode;
        //0 = all options. 
        public int NumberOfApplicableOptionsSelected;
        public string LocationDescription;
        public Sprite LocationIcon;
        public Sprite LocationImage;
        [SerializeField]
        private MapOption[] LocationOptions;
    }
}