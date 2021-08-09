using MapLogic;
using System;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MapUI
{
    public class MapNodeIcon : MonoBehaviour
    {
        public Button Icon;
        public MapNode Node;
        public Text DebugText;
        public TextMeshProUGUI NameText;
        [SerializeField]
        private ParticleSystem HighlightParticles;
        public CanvasGroup CanvasGroup;
        public bool Revealed;
        public static UnityEvent OnMapButtonClick = new UnityEvent();
        //public static UnityEvent OnHeroWalking = new UnityEvent();

        private void Start()
        {
            Icon.onClick.AddListener(() =>StartCoroutine( MoveToRoutine()));
            OnMapButtonClick.AddListener(() => SetInteractable(false));
        }

        private IEnumerator MoveToRoutine()
        {
            OnMapButtonClick.Invoke();

            MapUI.Instance.MoveHero(this);

            yield return new WaitForSeconds(1.8f);

            Node.Map.MoveToNode(Node);

            if (!string.IsNullOrEmpty(Node.Location.PopUpDescription))
                PopupTextController.Instance.DisplayText(Node.Location.PopUpDescription, transform.position);

            //TODO: this is just a hack untill the other options are implemented
            if (!(Node.Location is CombatOption))
                MapUI.Instance.Open();
        }

        internal bool Reachable()
        {
            return Node.Map.CurrentNode == Node || Node.Map.CurrentNode.CanReach(Node);
        }

        internal void SetInteractable(bool interactable)
        {
            Icon.interactable = interactable;

            if (interactable)
            {
                HighlightParticles.Play();
            }
            else HighlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        }

           

    }
}