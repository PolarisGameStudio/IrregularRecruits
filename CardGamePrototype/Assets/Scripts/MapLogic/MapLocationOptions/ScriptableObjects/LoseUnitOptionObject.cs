using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Lose Unity Option")]
    public class LoseUnitOptionObject : MapOptionObject
    {
        public override string Name { get; } = "unit loss";

        public override MapOption InstantiateMapOption()
        {
            return new LoseUnitOption(this);
        }
    }
}