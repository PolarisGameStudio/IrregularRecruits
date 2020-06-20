using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

namespace MapLogic
{
    public class MapController 
    {
        private static MapController instance;

        public List<MapNode> Nodes = new List<MapNode>();
        public MapNode CurrentNode;
        //TODO. either use this or battleManager.PlayerDeck
        //public Deck PlayerDeck;
        public int PlayerGold;

        public static MapController Instance { get {
                if (instance == null)
                    instance = new MapController();
                return instance;
            }
        }

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

            MapNode[] lastStep = { GenerateNode(locations.Single(l => l.StartNode), locations) };

            for (int i = 1; i < settings.MapLength ; i++)
            {
                if (nodesAtStep[i] < 1)
                    Debug.LogError("Generated pathless map");

                MapNode[] step = new MapNode[nodesAtStep[i]];

                for (int j = 0; j < nodesAtStep[i]; j++)
                {
                    if (i == settings.MapLength - 1)
                        step[j] = GenerateNode(locations.Single(l => l.WinNode), locations);
                    else
                        step[j] = GenerateNode(locations.First(l => !l.WinNode), locations);
                }

                var edgesPrNode = step.Length / (float)lastStep.Length;

                while (lastStep.Any(c => c.LeadsTo.Count == 0) || step.Any(s => !Nodes.Any(n => n.LeadsTo.Contains(s))))
                {
                    //Select a node to create the edge from
                    var l =
                        Random.value > 0.7f ?
                        lastStep[Random.Range(0, lastStep.Length)] :
                        lastStep.First(pro => pro.LeadsTo.Count == lastStep.Min(s => s.LeadsTo.Count));

                    //if the randomly selected node already leads to all
                    if (l.LeadsTo.Count == step.Length)
                        continue;

                    //find the corresponding position of the next step
                    var pos = lastStep.ToList().IndexOf(l);
                    var desiredNodePos = Mathf.Clamp(Mathf.RoundToInt(pos * edgesPrNode), 0, step.Length - 1);

                    var mapNode = step[desiredNodePos];

                    int minimum = step.Min(s => Nodes.Count(n => n.LeadsTo.Contains(s)));

                    var leftOrRight = Random.value > 0.5f ? 1 : -1;

                    //select the closest minimum
                    while (l.LeadsTo.Contains(mapNode)  && Mathf.Abs(leftOrRight) < step.Length)
                    {
                        desiredNodePos += leftOrRight;
                        if (leftOrRight > 0)
                            leftOrRight++;
                        else
                            leftOrRight--;
                        leftOrRight *= -1;

                        //wrapping. could also limit
                        if (desiredNodePos > step.Length - 1)
                        {
                            mapNode = step.Last(n => !l.LeadsTo.Contains(n));
                            break;
                        }
                        if (desiredNodePos < 0)
                        {
                            mapNode = step.First(n => !l.LeadsTo.Contains(n));
                            break;
                        }

                        mapNode = step[desiredNodePos];

                    }

                    if (l.LeadsTo.Contains(mapNode)) //overflow
                        Debug.LogError("creating path That is already created");

                    //establish connection
                    l.LeadsTo.Add(mapNode);
                }

                lastStep = step;
            }

            CurrentNode = Nodes.Single(n => n.IsStartNode());

        }

        public void MoveToNode(MapNode node)
        {
            if (!CurrentNode.LeadsTo.Contains(node))
                return;
            if (CurrentNode.Active)
                return;

            CurrentNode = node;
            node.Open();
        }

        public void StartCombat(Deck enemyDeck)
        {

            Event.OnCombatSetup.Invoke(BattleManager.Instance. PlayerDeck, enemyDeck);
        }

        private MapNode GenerateNode(MapLocation locationObject, List<MapLocation> locations)
        {
            locations.Remove(locationObject);

            var node = new MapNode(locationObject);
            Nodes.Add(node);

            node.Id = Nodes.Count;

            return node;
        }
    }
}