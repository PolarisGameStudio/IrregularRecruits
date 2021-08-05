namespace MapLogic
{
    public class EndGameOption : MapOption
    {
        public bool Win;

        public EndGameOption(EndGameOptionObject optionObject) : base(optionObject)
        {
            Win = optionObject.Win;
        }

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