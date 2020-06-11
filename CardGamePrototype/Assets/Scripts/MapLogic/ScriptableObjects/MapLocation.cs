using System.Collections;
using UnityEngine;


namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Location", order = 0)]
    public class MapLocation : ScriptableObject
    {
        public string Name;
        public bool ExecuteRandomCombatOptionAutomatically;
        //0 = all options. 
        public int NumberOfApplicableOptionsSelected;
        public string LocationDescription;
        public Sprite LocationIcon;
        public Sprite LocationImage;
        [SerializeField]
        private MapOption[] LocationOptions;
    }
}