using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain Gold Option")]
    public class GainGoldOption : MapOption
    {
        public override string Name { get; }
        public int Amount;

        public GainGoldOption()
        {
            Name = $"Treasure";
        }

        public GainGoldOption(int amount)
        {
            Amount = amount;
            Name = $"Treasure";
        }

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            MapController.Instance.PlayerGold += Amount;
        }

    }
}