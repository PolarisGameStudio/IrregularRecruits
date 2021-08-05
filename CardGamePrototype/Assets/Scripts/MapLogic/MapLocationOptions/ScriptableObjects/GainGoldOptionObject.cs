using System.Collections.Generic;
using UnityEngine;
using GameLogic;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Gain Gold Option")]
    public class GainGoldOptionObject : MapOptionObject
    {
        public override string Name { get; }
        public int Amount;

        public override MapOption InstantiateMapOption()
        {
            return new GainGoldOption(this);
        }
    }
}