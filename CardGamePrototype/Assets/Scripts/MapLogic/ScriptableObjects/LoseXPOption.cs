﻿using GameLogic;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose XP Option")]
    public class LoseXPOption : MapOption
    {

        public override string Name { get; } = "xp loss";
        public int Amount;

        public override float Difficulty()
        {
            return Amount;
        }

        public override void ExecuteOption(MapNode owner)
        {
            BattleManager.Instance.PlayerDeck.Hero?.RemoveXp(Amount);
        }

        public override bool IsApplicable()
        {
            return base.IsApplicable() && BattleManager.Instance.PlayerDeck.Hero.Xp >= Amount;
        }
    }
}