using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{


    [CreateAssetMenu(menuName = "Create Map Objects/Gain Unit Option")]
    public class GainUnitOption : MapOption
    {
        public override string Name { get; } = "gain units";
        public List<Creature> Units;

        public override float Difficulty()
        {
            return Units.Sum(u => u.CR);
        }

        public override void ExecuteOption(MapNode owner)
        {
            foreach (var item in Units)
            {
                BattleManager.Instance.PlayerDeck.AddCard(new Card(item));

            }
        }
    }
}