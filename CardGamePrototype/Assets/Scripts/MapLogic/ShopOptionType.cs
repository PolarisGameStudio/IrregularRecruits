using System;
using System.Collections.Generic;

namespace GameLogic
{
    public enum ShopOptionType
    {
        OwnerRace,
        FriendRace,
        NonEnemyRace
    }

    [Serializable]
    public struct ShopOption
    {
        public List<ShopOptionType> Options;
    }
}