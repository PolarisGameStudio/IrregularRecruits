using MapLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using UnityEngine.UI;

namespace UI
{

    public class MapUI : Singleton<MapUI>
    {
        public MapNodeIcon NodeIconPrefab;
        public MapEdge LinePrefab;
        public List<MapNodeIcon> Nodes = new List<MapNodeIcon>();
        private List<MapNodeIcon> OldUnusedNodes = new List<MapNodeIcon>();
        public GameObject Holder;
        [Range(0.5f, 5)]
        public float MapSize;

        private void Start()
        {
            if (MapController.Instance.Nodes.Count == 0)
                MapController.Instance.CreateMap();

            MapNode.OpenLocationEvent.AddListener(UpdateNodes);

            Event.OnGameBegin.AddListener(Open);
            Event.OnCombatSetup.AddListener((e, v) => Close());
            Event.OnBattleFinished.AddListener((winner) => Open());

            UpdateNodes(MapController.Instance.CurrentNode);
        }

        private void UpdateNodes(MapNode current)
        {
            DrawMap(current,MapSettings.Instance.VisibleSteps);

            foreach (var n in Nodes)
            {
                n.Icon.interactable = (current.LeadsTo.Contains(n.Node));

                if (n.Node == current) n.HighlightParticles.Play();
                else n.HighlightParticles.Stop();
            }
        }

        public void Open()
        {
            Holder.SetActive(true);
        }
        public void Close()
        {
            Holder.SetActive(false);
        }

        private void DrawMap(MapNode startNode, int shownSteps = 1000)
        {
            OldUnusedNodes = Nodes.ToList();

            Nodes.Clear();

            CreateNode(startNode, transform.position);

            DrawStepRecursive(startNode.LeadsTo, 1, shownSteps);

            foreach (var n in OldUnusedNodes)
                DestroyNode(n);
        }

        private static void DestroyNode(MapNodeIcon n)
        {
            Destroy(n.gameObject);
        }

        private void DrawStepRecursive(List<MapNode> nodes, int degree, int shownSteps)
        {
            var r = degree * MapSize;

            var angleDiff = 0.5f * Mathf.PI / nodes.Count;

            var angle = 0f;
            var rnd = 0.1f;

            if (nodes.Count == 1)
                angle += angleDiff / 2;

            foreach (var node in nodes)
            {
                var x = r * Mathf.Cos(angle + Random.Range(-rnd, rnd));
                var y = r * Mathf.Sin(angle + Random.Range(-rnd, rnd));
                var pos = new Vector3(x, y);

                CreateNode(node, transform.position + pos);

                angle += angleDiff;
            }

            var combinedLeadsTo = nodes.SelectMany(n => n.LeadsTo).Distinct().OrderBy(n => n.Id).ToList();

            if (combinedLeadsTo.Any() && shownSteps > degree)
            {
                DrawStepRecursive(combinedLeadsTo, degree + 1, shownSteps);
            }
        }

        private void CreateNode(MapNode node, Vector3 position)
        {
            MapNodeIcon instance;

            if (OldUnusedNodes.Any(n => n.Node == node))
            {
                var n = OldUnusedNodes.Single(old => old.Node == node);

                instance = n;

                OldUnusedNodes.Remove(n);
            }
            else
            {
                instance = Instantiate(NodeIconPrefab, Holder.transform);

                instance.transform.position = position;

                instance.Icon.image.sprite = node.Location.LocationIcon;

                instance.Node = node;

                foreach (var parent in Nodes.Where(n => n.Node.LeadsTo.Contains(node)))
                    DrawLine(parent, instance);
            }

            Nodes.Add(instance);
        }

        private void DrawLine(MapNodeIcon start, MapNodeIcon finish)
        {
            //Debug.DrawLine(start.transform.position, finish.transform.position, Color.black, 100000);

            var line = Instantiate(LinePrefab, start.transform);

            var targetDir = finish.transform.position - start.transform.position;

            line.transform.localScale = new Vector3(targetDir.magnitude*0.8f, 1);

            targetDir = targetDir.normalized;

            float dot = Vector3.Dot(targetDir, new Vector3(1,0));

            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (targetDir.y < 0) angle *= -1;

            line.transform.Rotate(new Vector3(0, 0, angle));

        }
    }

}