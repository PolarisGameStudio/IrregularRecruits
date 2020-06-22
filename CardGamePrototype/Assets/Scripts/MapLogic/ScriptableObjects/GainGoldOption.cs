using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain Gold Option")]
    public class GainGoldOption : MapOption
    {
        public int Amount;

        public override void ExecuteOption(MapNode owner)
        {
            MapController.Instance.PlayerGold += Amount;
        }

    }
}