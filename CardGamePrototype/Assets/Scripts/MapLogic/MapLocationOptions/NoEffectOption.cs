namespace MapLogic
{
    public class NoEffectOption : MapOption
    {
        public NoEffectOption(MapOptionObject optionObject) : base(optionObject)
        {
            Name = "nothing";
            PopUpDescription = "nothing";
        }


        public override float Difficulty()
        {
            return 0;
        }

        public override void ExecuteOption(MapNode owner)
        {
        }
    }
}