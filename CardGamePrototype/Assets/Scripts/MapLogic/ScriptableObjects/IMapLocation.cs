namespace MapLogic
{
    public interface IMapLocation
    {
        MapOption[] GetLocationOptions();

        void Open(MapNode node);
        bool IsStartNode();
        bool IsWinNode();
        bool IsUniqueNode();
        float Difficulty();
    }
}