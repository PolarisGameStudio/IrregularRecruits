namespace MapLogic
{
    public interface IMapLocation
    {
        MapOption[] GetLocationOptions();

        string Name { get; set; }
        string PopUpDescription { get; set; }

        void Open(MapNode node);
        bool IsStartNode();
        bool IsWinNode();
        bool IsUniqueNode();
        float Difficulty();
    }
}