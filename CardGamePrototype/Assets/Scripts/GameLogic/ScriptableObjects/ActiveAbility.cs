using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class ActiveAbility : Ability
    {
        public int Cost = 1;
        public bool OncePrCombat;
    }
}