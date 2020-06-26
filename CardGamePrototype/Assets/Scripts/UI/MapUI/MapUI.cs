using MapLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using UnityEngine.UI;
using System.Collections;

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
        public Sprite[] Linetypes;
        public MapNode CurrentNode;
 
        private void Start()
        {
            if (MapController.Instance.Nodes.Count == 0)
                MapController.Instance.CreateMap();


            //TODO: make sure that this is called
            Event.OnGameBegin.AddListener(Open);
            Event.OnCombatSetup.AddListener((e, v) => Close());
            Event.OnBattleFinished.AddListener((winner) => Open());
            LocationUI.Instance.OnClose.AddListener(Open);
            LocationUI.Instance.OnOpen.AddListener(Close);

            UpdateNodes();
        }

        private void UpdateNodes()
        {
            StartCoroutine(DrawMap(MapController.Instance.CurrentNode, MapSettings.Instance.VisibleSteps));
        }

        public void Open()
        {
            Holder.SetActive(true);
            UpdateNodes();
        }
        public void Close()
        {
            Holder.SetActive(false);
            foreach (var item in Nodes)
            {
                item.HighlightParticles.Stop();
            }
        }

        private IEnumerator DrawMap(MapNode startNode, int shownSteps = 1000)
        {
            OldUnusedNodes = Nodes.ToList();

            foreach (var n in Nodes)
            {
                n.Icon.interactable = false;
                n.HighlightParticles.Stop();
            }
            
            Nodes.Clear();

            yield return CreateNode(startNode, transform.position);

            yield return DrawStepRecursive(startNode.LeadsTo, 1, shownSteps,Nodes.Single());

            foreach (var n in OldUnusedNodes)
                StartCoroutine(DestroyNode(n));

        }

        private static IEnumerator DestroyNode(MapNodeIcon n)
        {
            const float fadeTime = 0.7f;

            n.CanvasGroup.LeanAlpha(0f, fadeTime);

            yield return new WaitForSeconds(fadeTime);

            Destroy(n.gameObject);
        }

        private IEnumerator DrawStepRecursive(List<MapNode> nodes, int degree, int shownSteps,MapNodeIcon startNode)
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

                yield return CreateNode(node, startNode.transform.position + pos,degree == 1);

                angle += angleDiff;
            }

            var combinedLeadsTo = nodes.SelectMany(n => n.LeadsTo).Distinct().OrderBy(n => n.Id).ToList();

            if (combinedLeadsTo.Any() && shownSteps > degree)
            {
                yield return DrawStepRecursive(combinedLeadsTo, degree + 1, shownSteps,startNode);
            }
        }

        private IEnumerator CreateNode(MapNode node, Vector3 position,bool interactable = false)
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

                instance.transform.localScale = Vector3.zero;

                instance.transform.LeanScale(Vector3.one, 1f).setEaseInCubic();


                foreach (var parent in Nodes.Where(n => n.Node.LeadsTo.Contains(node)))
                    yield return DrawLine(parent, instance);
            }

            instance.Icon.interactable = interactable;

            if (interactable) instance.HighlightParticles.Play();

            Nodes.Add(instance);
        }

        private IEnumerator DrawLine(MapNodeIcon start, MapNodeIcon finish)
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

            yield return line.DrawLine();

        }

        public static Sprite GetRandomLineSprite()
        {
            var lines = Instance.Linetypes;

            return lines[Random.Range(0, lines.Length)];
        }
    }

}