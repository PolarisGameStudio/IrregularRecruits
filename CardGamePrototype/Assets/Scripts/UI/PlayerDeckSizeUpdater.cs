using GameLogic;
using MapLogic;
using TMPro;
using UnityEngine;
using Event = GameLogic.Event;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerDeckSizeUpdater : MonoBehaviour
    {
        private TextMeshProUGUI Text;

        private void Start()
        {
            Text = GetComponent<TextMeshProUGUI>();
            UpdateText();

            Event.OnHireMinions.AddListener(m=> UpdateText());
            Event.OnBattleFinished.AddListener(m=> UpdateText());

        }

        private void UpdateText()
        {
            Text.text = BattleManager.Instance.PlayerDeck?.AllCreatures().Count.ToString();
        }

    }
}