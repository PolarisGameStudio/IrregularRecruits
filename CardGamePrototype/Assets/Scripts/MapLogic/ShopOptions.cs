using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class ShopOptions : SingletonScriptableObject<ShopOptions>
    {
        //how many and what can they be
        public List<ShopOption> Options;

        public int Rerolls = -1;

        public int RerollInitialCost;

        public int RerollCostIncrease;

    }
}