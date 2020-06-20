using MapLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{

    public class MapUI : Singleton<MapUI>
    {
        public MapNodeIcon NodeIconPrefab;
        public List<MapNodeIcon> Nodes = new List<MapNodeIcon>();

        private void Start()
        {
            if (MapController.Instance.Nodes.Count == 0)
                MapController.Instance.CreateMap();

            DrawMap(MapController.Instance.CurrentNode);

            MapNode.OpenLocationEvent.AddListener(UpdateNodes);

            UpdateNodes(MapController.Instance.CurrentNode);
        }

        private void UpdateNodes(MapNode current)
        {
            foreach(var n in Nodes)
            {
                n.Icon.interactable = (current.LeadsTo.Contains(n.Node));

                if (n.Node == current) n.HighlightParticles.Play();
                else n.HighlightParticles.Stop();
            }
        }

        private void DrawMap(MapNode startNode)
        {
            CreateNode(startNode, transform.position);

            DrawStepRecursive(startNode.LeadsTo,1);
        }

        private void DrawStepRecursive(List<MapNode> nodes,int degree)
        {
            var r = degree * 1.4f;

            var angleDiff =  Mathf.PI/ nodes.Count;

            var angle = 0f;
            var rnd = 0.25f;

            if (nodes.Count == 1)
                angle += angleDiff / 2;

            foreach (var node in nodes)
            {
                var x = r * Mathf.Cos(angle + Random.Range(-rnd,rnd));
                var y = r * Mathf.Sin(angle + Random.Range(-rnd, rnd));
                var pos = new Vector3(x, y);

                CreateNode(node, transform.position + pos);

                angle += angleDiff;
            }

            var combinedLeadsTo = nodes.SelectMany(n => n.LeadsTo).Distinct().OrderBy(n=>n.Id).ToList();

            if(combinedLeadsTo.Any())
                DrawStepRecursive(combinedLeadsTo, degree + 1);
        }

        private void CreateNode(MapNode node, Vector3 position)
        {
            var instance = Instantiate(NodeIconPrefab, this.transform);

            instance.transform.position = position;

            instance.Icon.image.sprite = node.Location.LocationIcon;

            instance.Node = node;

            foreach (var parent in Nodes.Where(n => n.Node.LeadsTo.Contains(node)))
                DrawLine(parent, instance);

            Nodes.Add(instance);
        }

        private void DrawLine(MapNodeIcon start,MapNodeIcon finish)
        {
            Debug.DrawLine(start.transform.position, finish.transform.position, Color.black,100000);
        }
    }

}