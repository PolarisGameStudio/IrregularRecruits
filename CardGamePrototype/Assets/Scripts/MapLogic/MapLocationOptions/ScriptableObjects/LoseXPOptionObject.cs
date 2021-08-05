using GameLogic;
using UnityEngine;

namespace MapLogic
{
        [CreateAssetMenu(menuName = "Create Map Objects/Lose XP Option")]
    public class LoseXPOptionObject : MapOptionObject
    {
        public override string Name { get; } = "xp loss";
        public int Amount;

        public override bool IsApplicable()
        {
            return base.IsApplicable() && Battle.PlayerDeck.Hero.Xp >= Amount;
        }

        public override MapOption InstantiateMapOption()
        {
            return new LoseXPOption(this);
        }
    }
}