using GameLogic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain XP Option")]
    public class GainXpOption : MapOption
    {
        public int Amount;

        public override void ExecuteOption(MapNode owner)
        {
            BattleManager.Instance.PlayerDeck.Hero?.AwardXp(Amount);
        }

    }
}