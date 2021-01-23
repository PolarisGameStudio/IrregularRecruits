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

            UpdateText(BattleManager.Instance.PlayerDeck);

            Event.OnDeckSizeChange.AddListener(UpdateText);

        }

        private void UpdateText(Deck deck)
        {
            if (deck != BattleManager.Instance.PlayerDeck)
                return;

            string count = BattleManager.Instance.PlayerDeck?.AllCreatures().Count.ToString();

            Debug.Log("updating text to " + count);

            Text.text = count;
        }

    }
}