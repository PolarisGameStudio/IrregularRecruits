using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/End Game Option")]
    public class EndGameOption : MapOption
    {
        public bool Win;

        public override string Name => "end";

        public override float Difficulty()
        {
            return 0f;
        }

        public override void ExecuteOption(MapNode owner)
        {
            GameLogic.Event.OnStatScreen.Invoke();
        }
    }
}