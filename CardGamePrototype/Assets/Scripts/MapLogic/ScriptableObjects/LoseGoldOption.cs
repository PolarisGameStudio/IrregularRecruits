using GameLogic;
using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Gold Option")]
    class LoseGoldOption : MapOption
    {
        public List<Creature> Creatures;

        public override void ExecuteOption(MapLocation owner)
        {
            throw new System.NotImplementedException();
        }
    }
}