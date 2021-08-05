using GameLogic;

namespace MapLogic
{
    public class LoseXPOption : MapOption
    {
        public readonly int Amount;

        public LoseXPOption(MapOptionObject optionObject) : base(optionObject)
        {
            Amount = -optionObject.XpAmount;

            //should be positive
            if (Amount < 0)
                Amount = -Amount;
        }

        public LoseXPOption(int amount)
        {
            Amount = amount;
        }

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            Battle.PlayerDeck.Hero?.RemoveXp(Amount);
        }

        public override bool IsApplicable()
        {
            return base.IsApplicable() && Battle.PlayerDeck.Hero.Xp >= Amount;
        }
    }
}