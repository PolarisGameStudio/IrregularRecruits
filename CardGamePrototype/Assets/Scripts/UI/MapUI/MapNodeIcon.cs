using MapLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MapNodeIcon : MonoBehaviour
    {
        public Button Icon;
        public MapNode Node;
        public ParticleSystem HighlightParticles;

        private void Start()
        {
            Icon.onClick.AddListener(() => MapController.Instance.MoveToNode(Node));

        }
    }

}