using UnityEngine;


namespace MapLogic
{
    abstract class MapOption : ScriptableObject
    {
        public abstract void ExecuteOption(MapLocation owner);
    }
}