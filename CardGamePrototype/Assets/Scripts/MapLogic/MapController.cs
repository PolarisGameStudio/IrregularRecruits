using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{

    public class MapController
    {
        private List<MapNode> Nodes = new List<MapNode>();

        public void CreateMap()
        {
            var settings = MapSettings.Instance;

            int[] nodesAtStep = new int[settings.MapLength];

            nodesAtStep[0] = nodesAtStep[settings.MapLength - 1] = 1;
            nodesAtStep[settings.MapLength - 2] = settings.MaxRoadsFromNode;

            for (int i = 1; i < settings.MapLength - 2; i++)
            {
                nodesAtStep[i] = Mathf.Min(nodesAtStep[i - 1] * settings.MaxRoadsFromNode, Random.Range(settings.MaxRoadsFromNode, settings.MaxNodesAtStep));
            }

            var noOfNodes = nodesAtStep.Sum();

            var locations = settings.LocationObjects.OrderBy(c => Random.value).ToList();

            //only one startnode
            while (locations.Count(l => l.StartNode) > 1)
                locations.Remove(locations.First(l => l.StartNode));

            //only one winnode
            while (locations.Count(l => l.WinNode) > 1)
                locations.Remove(locations.First(l => l.WinNode));

            var nonUniques = locations.Where(l => !l.UniqueNode).ToArray();

            //correct number of locations
            while (locations.Count < noOfNodes)
            {
                locations.Add(nonUniques[Random.Range(0, nonUniques.Length)]);
            }

            while (locations.Count > noOfNodes)
            {
                locations.Remove(locations.First(l => !l.StartNode && !l.WinNode));
            }

            locations = locations.OrderBy(d => d.Difficulty + settings.RandomnessToDifficulty * Random.value).ToList();

            MapNode[] lastStep = { GenerateNode(locations.First(l => l.StartNode), locations) };

            for (int i = 0; i < settings.MapLength - 1; i++)
            {
                if (nodesAtStep[i] < 1)
                    Debug.LogError("Generated pathless map");

                MapNode[] step = new MapNode[nodesAtStep[i]];

                for (int j = 0; j < nodesAtStep[i]; j++)
                {
                    if (i == settings.MapLength - 1)
                        step[j] = GenerateNode(locations.First(l => l.WinNode), locations);
                    else
                        step[j] = GenerateNode(locations.First(l => !l.WinNode), locations);
                }

                var edgesPrNode = step.Length / (float)lastStep.Length;

                while (lastStep.Any(c => c.LeadsTo.Count == 0) || step.Any(s => !Nodes.Any(n => n.LeadsTo.Contains(n))))
                {
                    //Select a node to create the edge from
                    var l =
                        Random.value > 0.5f ?
                        lastStep[Random.Range(0, lastStep.Length)] :
                        lastStep.First(l => l.LeadsTo.Count == lastStep.Min(s => s.LeadsTo.Count));

                    //find the corresponding position of the next step
                    var pos = lastStep.ToList().IndexOf(l);
                    var desiredNodePos = Mathf.Clamp(Mathf.RoundToInt(pos * edgesPrNode), 0, step.Length - 1);

                    var mapNode = step[desiredNodePos];

                    int minimum = step.Min(s => s.LeadsTo.Count);

                    var leftOrRight = Random.value > 0.5f ? 1 : -1;

                    //select the closest minimum
                    while (mapNode.LeadsTo.Count > minimum)
                    {
                        desiredNodePos += leftOrRight;
                        leftOrRight++;
                        leftOrRight *= -1;

                        //wrapping. could also limit
                        if (desiredNodePos > step.Length - 1) desiredNodePos = 0;
                        if (desiredNodePos < 0) desiredNodePos = step.Length - 1;

                        mapNode = step[desiredNodePos];

                    }

                    //establish connection
                    l.LeadsTo.Add(mapNode);
                }

                lastStep = step;
            }

        }

        private MapNode GenerateNode(MapLocation locationObject, List<MapLocation> locations)
        {
            locations.Remove(locationObject);

            var node = new MapNode(locationObject);
            Nodes.Add(node);

            return node;
        }
    }
}