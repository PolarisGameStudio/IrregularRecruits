using System.Collections;
using System.Collections.Generic;

namespace MapLogic
{
    public class MapNode 
    {
        public List<MapNode> LeadsTo;
        public bool Visited;        
        private readonly MapLocation Location;


        public MapNode(MapLocation mapLocation)
        {
            this.Location = mapLocation;
        }
    }
}