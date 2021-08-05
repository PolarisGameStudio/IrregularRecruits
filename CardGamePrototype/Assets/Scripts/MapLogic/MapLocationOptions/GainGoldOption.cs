using Event = GameLogic.Event;

namespace MapLogic
{
    public class GainGoldOption : MapOption
    {
        public readonly int Amount;

        public GainGoldOption(int amount)
        {
            Amount = amount;
            Name = $"Treasure";
            PopUpDescription = $"{Amount} gold";
        }

        public GainGoldOption(GainGoldOptionObject optionObject) : base(optionObject)
        {
            Amount = optionObject.Amount;
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