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
            Name = $"Campsite";
            PopUpDescription = $"{Amount} XP";
        }

        public GainXpOption(int amount)
        {
            Amount = amount / 10;
            Name = $"Campsite";
            PopUpDescription = $"{Amount} XP";
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