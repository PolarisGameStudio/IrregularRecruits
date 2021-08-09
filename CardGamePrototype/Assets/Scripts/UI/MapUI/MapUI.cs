using MapLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using System.Collections;
using UnityEngine.Events;
using UI;

namespace MapUI
{

    public class MapUI : Singleton<MapUI>, IUIWindow
    {
        public MapNodeIcon NodeIconPrefab;
        public MapEdge LinePrefab;
        public HeroIcon HeroIcon; 
        public List<MapNodeIcon> Nodes = new List<MapNodeIcon>();
        private List<MapNodeIcon> OldNodes = new List<MapNodeIcon>();
        public GameObject Holder;
        public CanvasGroup FocusGroup;
        public GameObject NodeHolder;
        public GameObject MapHolder;
        [Range(0.5f, 5)]
        public float MapSizeX;
        [Range(0.5f, 20)]
        public float MapSizeY;

        public float MapEdgeLength = 0.8f;
        public Sprite[] Linetypes;
        public Color MapDrawColor;
        public Color LineDrawColor;
        public RectTransform MapStartPosition;
        public Vector3 PositionToCenterDifferience;
        public float NodeFadeTime = 2f;
        public static UnityEvent OnMapOpen = new UnityEvent();
        public Map Map;

        private void Start()
        {
            Event.OnGameBegin.AddListener(CreateMap);

            Event.OnGameBegin.AddListener(Open);
            BattleSummary.Instance.OnClose.AddListener(Open);
            Event.OnCombatStart.AddListener(() => Close());

        }

        private void CreateMap()
        {
            Map = new Map();

            PositionToCenterDifferience = transform.position - MapStartPosition.position;
        }

        private void UpdateNodes()
        {
            StartCoroutine(DrawMap(Map.CurrentNode, MapSettings.Instance.VisibleSteps));
        }

        public void Open()
        {
            HeroIcon.Portrait.image.sprite = Battle.PlayerDeck.Hero.HeroObject.Portrait;

            OnMapOpen.Invoke();

            UIController.Instance.Open(this);


            //if (MapController.Instance.CurrentNode.Visited && MapController.Instance.CurrentNode.LeadsTo.Count == 0)
            //    LocationUI.Instance.OpenWinEvent();

            UpdateNodes();
        }
        public void Close()
        {

            UIController.Instance.Close(this);

            //foreach (var item in Nodes)
            //{
            //    item.SetInteractable(false);
            //}
        }

        public void MoveHero(MapNodeIcon node)
        {
            LeanTween.moveLocal(HeroIcon.gameObject, node.transform.localPosition, 1.5f);
        }

        private IEnumerator DrawMap(MapNode startNode, int shownSteps = 1000)
        {

            if (shownSteps < 0) shownSteps = 1000;

            OldNodes = Nodes.ToList();

            foreach (var n in Nodes)
            {
                n.SetInteractable(false);
            }

            foreach (var n in Nodes.Where(nod => !nod.Reachable()))
                StartCoroutine(DestroyNode(n));

            Nodes.Clear();
            
            //create from node
            yield return CreateNode(startNode, MapStartPosition.position);
            
            LeanTween.moveX(MapHolder.gameObject, MapHolder.transform.position.x-Nodes.First().transform.position.x - PositionToCenterDifferience.x, 3f).setEaseInExpo();

            yield return DrawStepRecursive(startNode.LeadsTo, 1, shownSteps,Nodes.Single());

            foreach(var n in Nodes.Where(nod => startNode.LeadsTo.Contains(nod.Node)))
            {
                n.SetInteractable(true);
            }
        }

        private  IEnumerator DestroyNode(MapNodeIcon n)
        {
            n.CanvasGroup.LeanAlpha(0f, NodeFadeTime);

            yield return new WaitForSeconds(NodeFadeTime);

            //Destroy(n.gameObject);
        }

        private IEnumerator DrawStepRecursive(List<MapNode> nodes, int degree, int shownSteps,MapNodeIcon startNode)
        {
            var r = degree * MapSizeX;

            //position should be all current node position + mapsize * direction

            var yPos = -0.5f * MapSizeY;

            var yDiff =  MapSizeY/ Mathf.Max(1, (nodes.Count-1));

            var yRnd = yDiff / 10f;

            var xRnd = 0.3f;

            //check: maybe if nodes is uneven + the diff/2
            if (nodes.Count == 1)
                yPos += MapSizeY / 2;
            else if(nodes.Count < 5) // bring the map closer together
            {
                var cut = yDiff / (nodes.Count * 2);
                yPos += cut/2f;
                yDiff -= cut;

            }

            //always centered on the y position
            var anchorPos = new Vector3(startNode.transform.position.x, MapStartPosition.position.y);

            foreach (var node in nodes)
            {
                var x = r + Random.Range(-xRnd, xRnd);
                var y = yPos + Random.Range(-yRnd, yRnd);
                var pos = new Vector3(x, y);

                yield return CreateNode(node, anchorPos + pos,degree == 1);

                yPos += yDiff;
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

            if (OldNodes.Any(n => n.Node == node))
            {
                var n = OldNodes.Single(old => old.Node == node);

                instance = n;

                OldNodes.Remove(n);

            }
            else
            {
                instance = Instantiate(NodeIconPrefab, NodeHolder.transform);

                instance.NameText.text = node.ToString();

                instance.transform.position = position;

                instance.Icon.image.sprite = MapSettings.GetLocationIcon(node.Location);

                instance.Node = node;

                instance.transform.localScale = Vector3.zero;

                instance.transform.LeanScale(Vector3.one, 1f).setEaseInCubic();

            instance.SetInteractable(false);

                foreach (var parent in Nodes.Where(n => n.Node.LeadsTo.Contains(node)))
                    yield return DrawLine(parent, instance);
            }

            instance.Icon.interactable = interactable;
            
            instance.SetInteractable(interactable);


            Nodes.Add(instance);
        }

        private IEnumerator DrawLine(MapNodeIcon start, MapNodeIcon finish)
        {
            //Debug.DrawLine(start.transform.position, finish.transform.position, Color.black, 100000);

            var line = Instantiate(LinePrefab, start.transform);

            var targetDir = finish.transform.position - start.transform.position;

            line.transform.localScale = new Vector3(targetDir.magnitude* MapEdgeLength, 1);

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

        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder()
        {
            return Holder;
        }

        public int GetPriority() => 2;
    }

}