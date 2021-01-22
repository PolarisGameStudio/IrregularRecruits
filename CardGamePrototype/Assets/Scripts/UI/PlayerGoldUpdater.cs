using MapLogic;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerGoldUpdater : MonoBehaviour
    {
        private TextMeshProUGUI Text;

        private void Start()
        {
            Text = GetComponent<TextMeshProUGUI>();

            Text.text = MapController.Instance.PlayerGold.ToString();

            MapController.Instance.OnPlayerGoldUpdate.AddListener(UpdateText);
        }

        private void UpdateText(int arg0)
        {
            Text.text = arg0.ToString();
        }
    }
}