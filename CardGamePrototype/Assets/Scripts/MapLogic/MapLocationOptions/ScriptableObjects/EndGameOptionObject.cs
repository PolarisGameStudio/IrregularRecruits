using UnityEngine;

namespace MapLogic
{

    [CreateAssetMenu(menuName = "Create Map Objects/End Game Option")]
    public class EndGameOptionObject : MapOptionObject
    {
        public bool Win;

        public override string Name => "end";

        public override MapOption InstantiateMapOption()
        {
            return new EndGameOption(this);
        }
    }
}