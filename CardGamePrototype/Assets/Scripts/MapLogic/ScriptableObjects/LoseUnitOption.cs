using GameLogic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Unity Option")]
    public class LoseUnitOption : MapOption
    {
        public override string Name { get; } = "unit loss";
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