using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;

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
            nodesAtStep[settings.MapLength - 2] = settings.MinNodesAtStep;

            for (int i = 1; i < settings.MapLength - 2; i++)
            {
                nodesAtStep[i] = Mathf.Min(nodesAtStep[i - 1] * settings.MinNodesAtStep, Random.Range(settings.MinNodesAtStep, settings.MaxNodesAtStep));
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

            for (int i = 1; i < settings.MapLength; i++)
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

                //create one road from each
                while (lastStep.Any(c => c.LeadsTo.Count == 0))
                {
                    //Select a node to create the edge from
                    var l = lastStep.First(c => c.LeadsTo.Count == 0);

                    var idx = lastStep.ToList().IndexOf(l);
                    var desiredNodePos = Mathf.Clamp(Mathf.RoundToInt(idx * edgesPrNode), 0, step.Length - 1);

                    var mapNode = step[desiredNodePos];

                    if (l.LeadsTo.Contains(mapNode)) //overflow
                        Debug.LogError("creating path That is already created");

                    //establish connection
                    l.LeadsTo.Add(mapNode);
                }

                var parentIdx = 0;
                var nodeIdx = 0;
                var noRoadExtraChance = 0.5f;

                //Creating road connections
                do
                {
                    //Debug.Log($"parent: {parentIdx} leads to node: {nodeIdx}");
                    lastStep[parentIdx].LeadsTo.Add(step[nodeIdx]);

                    if(noRoadExtraChance > Random.value)
                    {
                        parentIdx++;
                        nodeIdx++;

                        if (nodeIdx == step.Length) nodeIdx--;
                        if (parentIdx == lastStep.Length) parentIdx--;
                        continue;
                    }

                    var parentChance = Random.value * (lastStep.Length - parentIdx - 1);
                    var nodeChance = Random.value * (step.Length - nodeIdx - 1);

                    if (parentChance > nodeChance) parentIdx++;
                    else nodeIdx++;

                }
                while (nodeIdx < step.Length-1  || parentIdx < lastStep.Length-1 );


                lastStep[parentIdx].LeadsTo.Add(step[nodeIdx]);


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