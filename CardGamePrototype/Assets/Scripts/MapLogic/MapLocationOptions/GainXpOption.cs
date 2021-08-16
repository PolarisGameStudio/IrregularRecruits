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

        public GainXpOption(MapOptionObject optionObject) : base(optionObject)
        {
            Amount = optionObject.XpAmount;
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

        public override MapNodeType GetLocationType()
        {
            return MapNodeType.Xp;
        }

    }
}