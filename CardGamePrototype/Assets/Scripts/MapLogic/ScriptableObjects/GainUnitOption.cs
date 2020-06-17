using GameLogic;
using System.Collections.Generic;
using UnityEngine;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/Gain Unit Option")]
    public class GainUnitOption : MapOption
    {
        public List<Creature> Units;

        public override void ExecuteOption(MapNode owner)
        {
            foreach (var item in Units)
            {
                BattleManager.Instance.PlayerDeck.AddCard(new Card(item));

            }
        }
    }
}