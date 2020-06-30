namespace MapLogic
{
    public interface IMapLocation
    {
        MapOption[] GetLocationOptions();

        void Open(MapNode node);
    }
}