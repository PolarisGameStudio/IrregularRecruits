using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;



namespace MapLogic
{

    public class MapController 
    {
        private static MapController instance;

        public List<MapNode> Nodes = new List<MapNode>();
        public MapNode CurrentNode;

        //TODO: only use event.OnPlayerGoldAdd
        public class AmountEvent : UnityEvent<int> { }
        public AmountEvent OnPlayerGoldUpdate = new AmountEvent();

        public static MapController Instance { get {
                if (instance == null)
                    instance = new MapController();
                return instance;
            }
        }

        private int playerGold;

        public MapController()
        {
            Event.OnPlayerGoldAdd.AddListener(i => PlayerGold += i);

        }

        public int PlayerGold
        {
            get => playerGold; 
            set
            {
                if (value == playerGold)
                    return;

                playerGold = value;

                OnPlayerGoldUpdate.Invoke(value);
            }
        }

        public void CreateMap()
        {
            var settings = MapSettings.Instance;

            int CurrentDifficulty = settings.StartDifficulty;


            int[] nodesAtStep = new int[settings.MapLength];

            nodesAtStep[0] = nodesAtStep[settings.MapLength - 1] = 1;
            nodesAtStep[settings.MapLength - 2] = settings.MinNodesAtStep;

            for (int i = 1; i < settings.MapLength - 2; i++)
            {
                nodesAtStep[i] = Mathf.Min(nodesAtStep[i - 1] * settings.MinNodesAtStep, Random.Range(settings.MinNodesAtStep, settings.MaxNodesAtStep));
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

            nodeTypes[MapNodeType.Treasure] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.GoldFrequency / totalFrequency));
            nodeTypes[MapNodeType.Event] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.EventFrequency/ totalFrequency));
            nodeTypes[MapNodeType.HardCombat] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.HardCombatFrequency/ totalFrequency));

            //only standard combats at first step, so they don't count
            nodeTypes[MapNodeType.StandardCombat] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.StandardCombatFrequency/ totalFrequency)) - nodesAtStep[1];
            nodeTypes[MapNodeType.Xp] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.XpFrequency/ totalFrequency));
            nodeTypes[MapNodeType.Village] =Mathf.RoundToInt(  noOfNodes * (MapSettings.Instance.VillageFrequency/ totalFrequency));

            uniqueLocations = uniqueLocations.OrderBy(d => d.Difficulty + settings.RandomnessToDifficulty * Random.value).ToList();

            MapNode[] lastStep = { GenerateNode(uniqueLocations.Single(l => l.IsStartNode()), uniqueLocations) };

            for (int i = 1; i < settings.MapLength; i++)
            {
                CurrentDifficulty += settings.StepDifficultyIncrease;

                if (nodesAtStep[i] < 1)
                    Debug.LogError("Generated pathless map");

                MapNode[] step = new MapNode[nodesAtStep[i]];

                for (int j = 0; j < nodesAtStep[i]; j++)
                {

                    if (i == settings.MapLength - 1)
                        step[j] = GenerateNode(uniqueLocations.Single(l => l.IsWinNode()), uniqueLocations);
                    else if (i == 1)
                        step[j] = GenerateNode(MapNodeType.StandardCombat,CurrentDifficulty,uniqueLocations);
                    else {
                        MapNodeType nodeType = GetNextNode(nodeTypes);

                        step[j] = GenerateNode(nodeType , CurrentDifficulty,uniqueLocations);
                    } 
                }


                var extraStepsAtEachPathMin = 1;
                var extraStepsAtEachPathMax = 4;

                var minRoads = Mathf.Max(lastStep.Length, step.Length);
                var maxRoads = minRoads* 2 - 1;

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
                    else if(parentIdx < lastStep.Length-1 && lastStep[parentIdx].LeadsTo.Count >= maxRoadsFrom )
                    {
                        parentIdx++;
                    }
                    else
                    {
                        var parentChance = Random.value * (lastStep.Length - parentIdx - 1)+ (lastStep.Length - parentIdx - 1);
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

                lastStep = step;
            }

            Debug.Log("unused nodes left: " + nodeTypes.Count);

            CurrentNode = Nodes.Single(n => n.IsStartNode());
        }

        private MapNodeType GetNextNode(Dictionary<MapNodeType, int> nodeTypes)
        {
            var acc = 0;
            var total = nodeTypes.Values.Sum();

            var rnd = Random.Range(1, total+1);

            foreach(var n in nodeTypes)
            {
                if (rnd <= n.Value + acc)
                {
                    nodeTypes[n.Key]--;
                    return n.Key;
                }
                acc += n.Value;
            }

            throw new Exception("Paw math error, acc: "+ acc + ", rnd val: "+ rnd + ", total: "+ total);

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


        private MapNode GenerateNode(MapNodeType type,int CR, List<MapLocation> uniqueLocations)
        {
            MapNode node;

            var adjust = Mathf.Min(CR, MapSettings.Instance.RandomnessToDifficulty);

            CR += Random.Range(-adjust, adjust);

            if (CR < MapSettings.Instance.StepDifficultyIncrease) CR = MapSettings.Instance.StepDifficultyIncrease;

            //village
            var civilizedRaces = MapSettings.Instance.CivilizedRaces;

            var enemyRaces = MapSettings.Instance.EnemyRaces;

            var race = enemyRaces[Random.Range(0, enemyRaces.Length)];

            switch (type)
            {
                case MapNodeType.HardCombat:
                    node = new MapNode(new CombatOption(race, CR * 2, true));
                    break;
                case MapNodeType.Treasure:
                    //gold
                    node = new MapNode(new GainGoldOption(CR));
                    break;
                case MapNodeType.Xp:
                    //xp
                    node = new MapNode(new GainXpOption(CR));
                    break;
                case MapNodeType.Event:
                    node = new MapNode(uniqueLocations[Random.Range(0, uniqueLocations.Count)]);
                    break;
                case MapNodeType.Village:
                    node = new MapNode(new VillageShop(CR, civilizedRaces[Random.Range(0, civilizedRaces.Length)]));
                    break;
                case MapNodeType.StandardCombat:
                default:
                    node = new MapNode(new CombatOption(race, CR, false));
                    break;
            }


            Nodes.Add(node);

            node.Id = Nodes.Count;

            return node;
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