using System.Collections.Generic;
using System.Linq;
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

        public override string GetOptionDescription(MapNode owner, string overrideDescription = "")
        {
            var str = overrideDescription != "" ? overrideDescription : OptionDescription;

            foreach (var item in Options)
            {
                str = item.GetOptionDescription(owner, str);
            }

            return str;

        }

        internal override void FindCandidate(MapNode owner)
        {

            foreach (var item in Options)
            {
                item.FindCandidate(owner);
            }
        }

        public override float Difficulty()
        {
            return Options.Sum(o => o.Difficulty());
        }
    }
}