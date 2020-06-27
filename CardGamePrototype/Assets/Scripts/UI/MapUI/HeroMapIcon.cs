using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroMapIcon :MonoBehaviour {
        public Button Portrait;

        private void Start()
        {
            Portrait.onClick.AddListener(HeroView.Open);
        }
    }

}