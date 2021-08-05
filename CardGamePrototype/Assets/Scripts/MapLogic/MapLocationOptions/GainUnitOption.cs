using GameLogic;
using System.Collections.Generic;
using System.Linq;

namespace MapLogic
{
    public class GainUnitOption : MapOption
    {
        public readonly List<Creature> Units;

        public GainUnitOption(GainUnitOptionObject optionObject) : base(optionObject)
        {
            Name = "gain units";
            Units = optionObject.Units;
        }

        public GainUnitOption(List<Creature> creatures)
        {
            Units = creatures;
        }

        public override float Difficulty()
        {
            return Units.Sum(u => u.CR);
        }

        public override void ExecuteOption(MapNode owner)
        {
            foreach (var item in Units)
            {
                Battle.PlayerDeck.AddCard(new Card(item));

            }
        }
    }
}