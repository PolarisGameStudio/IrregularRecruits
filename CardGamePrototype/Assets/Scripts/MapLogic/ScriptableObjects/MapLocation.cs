using System.Collections;
using UnityEngine;


namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Location", order = 0)]
    public class MapLocation : ScriptableObject
    {
        public string Name;
        public string LocationDescription;
        public Sprite LocationIcon;
        public Sprite LocationImage;
        [SerializeField]
        private MapOption[] LocationOptions;
    }
}