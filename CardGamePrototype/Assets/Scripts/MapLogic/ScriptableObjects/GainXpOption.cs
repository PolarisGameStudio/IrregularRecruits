using GameLogic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain XP Option")]
    public class GainXpOption : MapOption
    {
        public override string Name { get; }
        public int Amount;

        public GainXpOption()
        {
            Name = $"Gain {Amount} Xp";
        }

        public GainXpOption(int amount)
        {
            Amount = amount;
            Name = $"Gain {amount} Xp";
        }

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            BattleManager.Instance.PlayerDeck.Hero?.AwardXp(Amount);
        }

    }
}