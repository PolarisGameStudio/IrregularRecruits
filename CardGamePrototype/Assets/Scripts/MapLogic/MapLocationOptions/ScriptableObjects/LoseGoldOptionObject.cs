using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Gold Option")]
    public class LoseGoldOptionObject : MapOptionObject
    {
        public override string Name { get; } = "gold loss";
        public int Amount;

        public override bool IsApplicable()
        {
            return base.IsApplicable() && Map.PlayerGold >= Amount;
        }

        public override MapOption InstantiateMapOption()
        {
            return new LoseGoldOption(this);
        }
    }
}