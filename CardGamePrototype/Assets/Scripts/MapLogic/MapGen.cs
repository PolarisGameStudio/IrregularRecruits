using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



namespace MapLogic
{
    public static class MapGen
    {

        public static void CreateMap(Map map)
        {
            var settings = MapSettings.Instance;

            int CurrentDifficulty = settings.StartDifficulty;

            List<MapNode> Nodes = new List<MapNode>();


            int[] nodesAtStep = new int[settings.MapLength];

            nodesAtStep[0] = nodesAtStep[settings.MapLength - 1] = 1;
            nodesAtStep[1] = nodesAtStep[settings.MapLength - 2] = 2;

            for (int i = 2; i < settings.MapLength - 2; i++)
            {

                nodesAtStep[i] = Mathf.Clamp(nodesAtStep[i - 1] + Random.Range(-2, 3), settings.MinNodesAtStep, settings.MaxNodesAtStep);
            }

            var noOfNodes = nodesAtStep.Sum();

            //TODO: should only be Event locations
            var uniqueLocations = settings.EventLocations.OrderBy(c => Random.value).ToList();

            //only one startnode
            while (uniqueLocations.Count(l => l.IsStartNode()) > 1)
                uniqueLocations.Remove(uniqueLocations.First(l => l.IsStartNode()));

            //only one winnode
            while (uniqueLocations.Count(l => l.IsWinNode()) > 1)
                uniqueLocations.Remove(uniqueLocations.First(l => l.IsWinNode()));

            var nonUniques = uniqueLocations.Where(l => !l.IsUniqueNode()).ToArray();

            var nodeTypes = new Dictionary<MapNodeType, int>();

            float totalFrequency = MapSettings.Instance.HardCombatFrequency
                + MapSettings.Instance.StandardCombatFrequency
                + MapSettings.Instance.EventFrequency
                + MapSettings.Instance.VillageFrequency + MapSettings.Instance.GoldFrequency + MapSettings.Instance.XpFrequency;

            nodeTypes[MapNodeType.Treasure] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.GoldFrequency / totalFrequency));
            nodeTypes[MapNodeType.Event] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.EventFrequency / totalFrequency));
            nodeTypes[MapNodeType.HardCombat] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.HardCombatFrequency / totalFrequency));

            //only standard combats at first step, so they don't count
            nodeTypes[MapNodeType.StandardCombat] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.StandardCombatFrequency / totalFrequency)) - nodesAtStep[1];
            nodeTypes[MapNodeType.Xp] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.XpFrequency / totalFrequency));
            nodeTypes[MapNodeType.Village] = Mathf.RoundToInt(noOfNodes * (MapSettings.Instance.VillageFrequency / totalFrequency));

            uniqueLocations = uniqueLocations.OrderBy(d => d.Difficulty + settings.RandomnessToDifficulty * Random.value).ToList();

            MapNode[] lastStep = { GenerateNode(uniqueLocations.Single(l => l.IsStartNode()), uniqueLocations,map) };

            for (int i = 1; i < settings.MapLength; i++)
            {
                CurrentDifficulty += settings.StepDifficultyIncrease;

                if (nodesAtStep[i] < 1)
                    Debug.LogError("Generated pathless map");

                MapNode[] step = new MapNode[nodesAtStep[i]];

                for (int j = 0; j < nodesAtStep[i]; j++)
                {

                    if (i == settings.MapLength - 1)
                        step[j] = GenerateNode(uniqueLocations.Single(l => l.IsWinNode()), uniqueLocations,map);
                    else if (i == 1)
                        step[j] = GenerateNode(MapNodeType.StandardCombat, CurrentDifficulty, uniqueLocations,map);
                    else
                    {
                        MapNodeType nodeType = GetNextNode(nodeTypes);

                        step[j] = GenerateNode(nodeType, CurrentDifficulty, uniqueLocations,map);
                    }
                }


                var extraStepsAtEachPathMin = 1;
                var extraStepsAtEachPathMax = 4;

                var minRoads = Mathf.Max(lastStep.Length, step.Length);
                var maxRoads = minRoads * 2 - 1;

                var roadsToCreate = Mathf.Min(maxRoads, minRoads + Random.Range(extraStepsAtEachPathMin, extraStepsAtEachPathMax));

                var edgesPrNode = step.Length / (float)lastStep.Length;

                var parentIdx = 0;
                var nodeIdx = 0;

                var maxRoadsFrom = 3;

                var extraRoadChance = MapSettings.Instance.ExtraRoadChance;

                lastStep[parentIdx].LeadsTo.Add(step[nodeIdx]);

                //Creating road connections
                do
                {
                    //Debug.Log($"parent: {parentIdx} leads to node: {nodeIdx}");
                    if (extraRoadChance < Random.value)
                    {
                        parentIdx++;
                        nodeIdx++;

                        if (nodeIdx == step.Length) nodeIdx--;
                        if (parentIdx == lastStep.Length) parentIdx--;
                    }
                    //TODO: we can still get too many roads from the last one
                    else if (parentIdx < lastStep.Length - 1 && lastStep[parentIdx].LeadsTo.Count >= maxRoadsFrom)
                    {
                        parentIdx++;
                    }
                    else
                    {
                        var parentChance = Random.value * (lastStep.Length - parentIdx - 1) + (lastStep.Length - parentIdx - 1);
                        var nodeChance = Random.value * (step.Length - nodeIdx - 1) + (step.Length - nodeIdx - 1);

                        //more roads to villages
                        if (step[nodeIdx].Location is VillageShop)
                            parentChance *= 2;


                        if (parentChance >= nodeChance) parentIdx++;
                        else nodeIdx++;
                    }

                    lastStep[parentIdx].LeadsTo.Add(step[nodeIdx]);
                }
                while (nodeIdx < step.Length - 1 || parentIdx < lastStep.Length - 1);

                foreach(var n in lastStep.Where(node => node.GetNodeType() != MapNodeType.StandardCombat && node.LeadsTo.All(child => child.GetNodeType() == node.GetNodeType())  ))
                {
                    var idx = Array.IndexOf(lastStep,n);

                    if (idx > 0 && CanMakeConnectionToAChild(n, lastStep[idx - 1], step, true) is MapNode item && item != null && item.GetNodeType() != n.GetNodeType())
                    {
                        n.LeadsTo.Add(item);

                        Debug.Log($"added child {item} to {n}. pos ({i},idx)");

                        //could also delete connections to old
                    }
                    else if (idx < lastStep.Count() - 1 && CanMakeConnectionToAChild(n, lastStep[idx + 1], step, false) is MapNode item2 && item2 != null && item2.GetNodeType() != n.GetNodeType())
                    {
                        n.LeadsTo.Add(item2);

                        Debug.Log($"added child {item2} to {n}. pos ({i},idx)");
                    }
                    else
                    {
                        if (idx > 0 && lastStep[idx - 1].LeadsTo.Count()>1)
                        {
                            MapNode childe =  TakeChildFrom(n, lastStep[idx - 1], step, true);

                            Debug.Log($"stole child {childe} for {n}. pos ({i},idx)");
                            n.LeadsTo.Add(childe);

                            if (idx < lastStep.Count() - 1) //also delete connections to old
                            {
                                MapNode ike = TakeChildFrom(lastStep[idx + 1], n, step, true);
                                lastStep[idx + 1].LeadsTo.Add(ike);
                            }
                        }
                        else if (idx < lastStep.Count() - 1 && lastStep[idx + 1].LeadsTo.Count() > 1)
                        {
                            MapNode childe = TakeChildFrom(n, lastStep[idx + 1], step, false);

                            Debug.Log($"stole child {childe} for {n}. pos ({i},idx)");
                            n.LeadsTo.Add(childe);

                            if (idx > 0) //also delete connections to old
                            {
                                MapNode ike = TakeChildFrom(lastStep[idx - 1], n, step, false);
                                lastStep[idx - 1].LeadsTo.Add(ike);
                            }
                        }
                    }
                }

                lastStep = step;
            }

            map.CurrentNode = map.Nodes.Single(n => n.IsStartNode());
        }

        private static MapNode TakeChildFrom(MapNode thief, MapNode victimParent, MapNode[] childStep, bool highestChild)
        {
            //first remove all conflicts
            victimParent.LeadsTo.RemoveAll(thief.LeadsTo.Contains);

            if (highestChild)
                return childStep[victimParent.LeadsTo.Max(child => Array.IndexOf(childStep, child))];
            else
                return childStep[victimParent.LeadsTo.Min(child => Array.IndexOf(childStep, child))];
        }

        private static MapNode CanMakeConnectionToAChild(MapNode parent, MapNode otherParent, MapNode[] childStep,bool highestIdxChild)
        {
            if (parent.LeadsTo.Any(otherParent.LeadsTo.Contains)) // connection would create an overlap
                return null;

            if (highestIdxChild)
                return childStep[otherParent.LeadsTo.Max(child => Array.IndexOf(childStep, child))];
            else
                return childStep[otherParent.LeadsTo.Min(child => Array.IndexOf(childStep, child))];
        }

        private static MapNode GenerateNode(MapNodeType type, int CR, List<MapLocation> uniqueLocations, Map map)
        {
            var adjust = Mathf.Min(CR, MapSettings.Instance.RandomnessToDifficulty);

            CR += Random.Range(-adjust, adjust);

            if (CR < MapSettings.Instance.StepDifficultyIncrease) CR = MapSettings.Instance.StepDifficultyIncrease;

            //village
            var civilizedRaces = MapSettings.Instance.CivilizedRaces;

            var enemyRaces = MapSettings.Instance.EnemyRaces;

            var race = enemyRaces[Random.Range(0, enemyRaces.Length)];


            MapNode node;
            switch (type)
            {
                case MapNodeType.StandardCombat:
                    node = new MapNode(new CombatOption(race, CR, false), map);
                    break;
                case MapNodeType.HardCombat:
                    node = new MapNode(new CombatOption(race, CR * 2, true), map);
                    break;
                case MapNodeType.Treasure:
                    node = new MapNode(new GainGoldOption(CR), map);
                    break;
                case MapNodeType.Xp:
                    node = new MapNode(new GainXpOption(CR), map);
                    break;
                case MapNodeType.Village:
                    node = new MapNode(new VillageShop(CR, civilizedRaces[Random.Range(0, civilizedRaces.Length)]), map);
                    break;
                case MapNodeType.Event:
                    node = new MapNode(uniqueLocations[Random.Range(0, uniqueLocations.Count)], map);
                    break;
                default:
                    node = new MapNode(new CombatOption(race, CR, false), map);
                    break;
            }

            map.Nodes.Add(node);

            node.Id = map.Nodes.Count;

            return node;
        }

        private static MapNode GenerateNode(MapLocation locationObject, List<MapLocation> locations, Map map)
        {
            locations.Remove(locationObject);

            var node = new MapNode(locationObject, map);
            map.Nodes.Add(node);

            node.Id = map.Nodes.Count;

            return node;
        }


        private static MapNodeType GetNextNode(Dictionary<MapNodeType, int> nodeTypes)
        {
            var acc = 0;
            var total = nodeTypes.Values.Sum();

            var rnd = Random.Range(1, total + 1);

            foreach (var n in nodeTypes)
            {
                if (rnd <= n.Value + acc)
                {
                    nodeTypes[n.Key]--;
                    return n.Key;
                }
                acc += n.Value;
            }

            throw new Exception("Paw math error, acc: " + acc + ", rnd val: " + rnd + ", total: " + total);

        }

    }
}