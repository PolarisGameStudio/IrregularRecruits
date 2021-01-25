using MapLogic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MapNodeIcon : MonoBehaviour
    {
        public Button Icon;
        public MapNode Node;
        public Text DebugText;
        [SerializeField]
        private ParticleSystem HighlightParticles;
        public CanvasGroup CanvasGroup;
        public bool Revealed;

        private void Start()
        {
            Icon.onClick.AddListener(() =>StartCoroutine( MoveToRoutine()));
        }

        private IEnumerator MoveToRoutine()
        {
            MapUI.Instance.MoveHero(this);

            yield return new WaitForSeconds(1.8f);

            MapController.Instance.MoveToNode(Node);

            //TODO: this is just a hack untill the other options are implemented
            if (!(Node.Location is CombatOption))
                MapUI.Instance.Open();
        }

        internal bool Reachable()
        {
            return MapController.Instance.CurrentNode == Node || MapController.Instance.CurrentNode.CanReach(Node);
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