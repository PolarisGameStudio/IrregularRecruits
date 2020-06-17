using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Option Composite")]
    public class MapOptionComposite : MapOption
    {
        public List<MapOption> Options;

        public override void ExecuteOption(MapNode owner)
        {
            foreach (var item in Options)
            {
                item.ExecuteOption(owner);
            }
        }

        public override bool IsApplicable()
        {
            return Options.TrueForAll(o => o.IsApplicable());
        }
    }
}