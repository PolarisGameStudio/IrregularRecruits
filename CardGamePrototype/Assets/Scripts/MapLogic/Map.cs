using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;



namespace MapLogic
{

    public class Map 
    {
        public List<MapNode> Nodes = new List<MapNode>();
        public MapNode CurrentNode;

        public static Event.IntEvent OnPlayerGoldUpdate = new Event.IntEvent();

        private static int playerGold;

        public static int PlayerGold
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

        public Map()
        {
            //TODO: remove the listener on destroy. it is added too often.
            Event.OnPlayerGoldAdd.AddListener(i => PlayerGold += i);

            MapGen.CreateMap(this);
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

    }
}