using GameLogic;

namespace MapLogic
{
    public class LoseUnitOption : MapOption
    {
        public LoseUnitOption()
        {
        }

        public LoseUnitOption(LoseUnitOptionObject optionObject) : base(optionObject)
        {
        }

        public override float Difficulty()
        {
            return 0;
        }

        public override void ExecuteOption(MapNode owner)
        {
            if (owner.SelectedCards.ContainsKey(this))
                Battle.PlayerDeck.Remove(owner.SelectedCards[this][0]);
        }
    }
}