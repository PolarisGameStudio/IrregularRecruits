using GameLogic;
using MapLogic;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace MapUI
{
    public class LocationUI : Singleton<LocationUI>, IUIWindow
    {
        public Image LocationImage;
        public TextMeshProUGUI Description;
        public Button OptionInstance;
        private List<Button> InstantiatedButtons = new List<Button>();
        private MapNode CurrentNode;
        public GameObject Holder;
        public CanvasGroup FocusGroup;

        //non-effect story events
        public MapLocation StartEvent;
        public MapLocation WinEvent;
        public MapLocation GameOverEvent;



        private void Awake()
        {
            MapNode.OpenEvent.AddListener(Open);
            MapNode.CloseLocation.AddListener(n => UIController.Instance.Close(this));
            OptionInstance.gameObject.SetActive(false);

            Event.OnGameBegin.AddListener(OpenStartEvent);
            Event.OnGameOver.AddListener(OpenGameOverEvent);
            Event.OnGameWin.AddListener(OpenWinEvent);
        }

        public void OpenWinEvent()
        {
            Debug.Log("opening Win screen");

            Open(new MapNode(WinEvent));
        }

        public void OpenGameOverEvent()
        {
            Debug.Log("opening Game Over screen");

            Open(new MapNode(GameOverEvent));

        }
        
        public void OpenStartEvent()
        {
            Debug.Log("opening start event");

            Hero hero = Battle.PlayerDeck.Hero;

            StartEvent.LocationDescription = hero.HeroObject.BackgroundText;

            Open(new MapNode(StartEvent));
        }

        public void Open(MapNode node)
        {
            if (!(node.Location is MapLocation))
            {
                Debug.Log("opening option as location");
                return;
            }

            var location = node.Location as MapLocation;

            UIController.Instance.Open(this);

            LocationImage.sprite = location.LocationImage;

            Description.text = location.LocationDescription;

            foreach (var oldBut in InstantiatedButtons)
            {
                Destroy(oldBut.gameObject);
            }

            InstantiatedButtons.Clear();

            CurrentNode = node;

            foreach (var opt in node.GetOptions())
            {
                CreateButton(opt, node);
            }
        }

        private void CreateButton(MapOption option, MapNode owner)
        {
            var instance = Instantiate(OptionInstance, OptionInstance.transform.parent);

            instance.onClick.AddListener(() => CurrentNode.SelectOption(option));

            instance.GetComponent<TextMeshProUGUI>().text = "- " + option.GetOptionDescription(owner);

            instance.gameObject.SetActive(true);

            InstantiatedButtons.Add(instance);
        }

        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder()
        {
            return Holder;
        }

        public int GetPriority() => 3;
    }
}