using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;

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
            PopUpDescription = $"{Amount} gold";
        }

        public GainGoldOption(int amount)
        {
            Amount = amount;
            Name = $"Treasure";
            PopUpDescription = $"{Amount} gold";
        }

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            Event.OnPlayerGoldAdd.Invoke(Amount);
        }

    }
}