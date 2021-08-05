using System.Collections.Generic;

namespace MapLogic
{
    public class LoseGoldOption : MapOption
    {
        public readonly int Amount;

        public LoseGoldOption(LoseGoldOptionObject optionObject) : base(optionObject)
        {
            Amount = optionObject.Amount;
        }

        public LoseGoldOption(int amount)
        {
            Amount = amount;
        }

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