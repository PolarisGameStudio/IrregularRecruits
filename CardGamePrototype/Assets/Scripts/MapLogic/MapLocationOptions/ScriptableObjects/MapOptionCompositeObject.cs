using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Option Composite")]
    public class MapOptionCompositeObject : MapOptionObject
    {
        public override string Name { get {
                return Options.Select(s => s.Name).Aggregate((s, n) => s + ", " + n);
            } }
        public List<MapOptionObject> Options;

        public override bool IsApplicable()
        {
            return Options.TrueForAll(o => o.IsApplicable());
        }

        public override MapOption InstantiateMapOption()
        {
            return new MapOptionComposite(this);
        }
    }
}