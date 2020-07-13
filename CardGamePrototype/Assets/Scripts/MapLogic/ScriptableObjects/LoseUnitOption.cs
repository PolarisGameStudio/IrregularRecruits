using GameLogic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Unity Option")]
    public class LoseUnitOption : MapOption
    {
        public override float Difficulty()
        {
            return 0;
        }

        public override void ExecuteOption(MapNode owner)
        {
            if (owner.SelectedCards.ContainsKey(this))
                BattleManager.Instance.PlayerDeck.Remove(owner.SelectedCards[this][0]);
        }
    }
}