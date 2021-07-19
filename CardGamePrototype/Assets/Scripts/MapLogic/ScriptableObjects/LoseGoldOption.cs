using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Gold Option")]
    public class LoseGoldOption : MapOption
    {
        public override string Name { get; } = "gold loss";
        public int Amount;

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            Map.PlayerGold -= Amount;
        }

        public override bool IsApplicable()
        {
            return base.IsApplicable() && Map.PlayerGold >= Amount;
        }
    }
}