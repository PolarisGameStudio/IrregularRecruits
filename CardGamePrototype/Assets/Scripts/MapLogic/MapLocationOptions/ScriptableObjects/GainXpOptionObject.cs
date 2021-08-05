using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Gain XP Option")]
    public class GainXpOptionObject : MapOptionObject
    {
        public override string Name { get; }
        public int Amount;

        public override MapOption InstantiateMapOption()
        {
            return new GainXpOption(this);
        }
    }
}