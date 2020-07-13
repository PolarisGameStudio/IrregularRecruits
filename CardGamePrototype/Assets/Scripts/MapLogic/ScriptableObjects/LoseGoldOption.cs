using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Gold Option")]
    public class LoseGoldOption : MapOption
    {
        public int Amount;

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            MapController.Instance.PlayerGold -= Amount;
        }

        public override bool IsApplicable()
        {
            return base.IsApplicable() && MapController.Instance.PlayerGold >= Amount;
        }
    }
}