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
        [SerializeField]
        private ParticleSystem HighlightParticles;
        public CanvasGroup CanvasGroup;
        public bool Revealed;

        private void Start()
        {
            Icon.onClick.AddListener(() => MapController.Instance.MoveToNode(Node));

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