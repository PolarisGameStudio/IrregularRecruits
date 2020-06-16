using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Option Composite")]
    public class MapOptionComposite : MapOption
    {
        public List<MapOption> Options;

        public override void ExecuteOption(MapLocation owner)
        {
            foreach (var item in Options)
            {
                item.ExecuteOption(owner);
            }
        }
    }
}