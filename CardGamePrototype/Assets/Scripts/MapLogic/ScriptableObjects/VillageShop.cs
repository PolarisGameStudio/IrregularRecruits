using GameLogic;

namespace MapLogic
{
    public class VillageShop : MapOption
    {
        public int CR;
        public Race Race;

        public VillageShop(int cR, Race race)
        {
            CR = cR;
            Race = race;
        }

        public override float Difficulty()
        {
            return CR;
        }

        public override void ExecuteOption(MapNode owner)
        {
            new Shop(Race);
        }
    }
}