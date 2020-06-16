using GameLogic;
using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Gain Unit Option")]
    public class GainUnitOption : MapOption
    {
        public List<Creature> Units;

        public override void ExecuteOption(MapLocation owner)
        {
            throw new System.NotImplementedException();
        }
    }
}