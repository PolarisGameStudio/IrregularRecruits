using GameLogic;
using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain Unit Option")]
    public class GainUnitOptionObject : MapOptionObject
    {
        public override string Name { get; } = "gain units";
        public List<Creature> Units;

        public override MapOption InstantiateMapOption()
        {
            return new GainUnitOption(this);
        }
    }
}