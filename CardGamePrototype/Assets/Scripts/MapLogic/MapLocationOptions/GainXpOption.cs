using GameLogic;

namespace MapLogic
{
    public class GainXpOption : MapOption
    {
        public readonly int Amount;

        public GainXpOption(int amount)
        {
            Amount = amount / 10;
            Name = $"Campsite";
            PopUpDescription = $"{Amount} XP";
        }

        public GainXpOption(GainXpOptionObject optionObject) : base(optionObject)
        {
            Amount = optionObject.Amount;
            Name = $"Campsite";
            PopUpDescription = $"{Amount} XP";
        }

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            Battle.PlayerDeck.Hero?.AwardXp(Amount);
        }

    }
}