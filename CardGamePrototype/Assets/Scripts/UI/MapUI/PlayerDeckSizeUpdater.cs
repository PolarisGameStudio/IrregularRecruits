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

            UpdateText(Battle.PlayerDeck);

            Event.OnDeckSizeChange.AddListener(UpdateText);

        }

        private void UpdateText(Deck deck)
        {
            if (deck != Battle.PlayerDeck)
                return;

            string count = Battle.PlayerDeck?.AllCreatures().Count.ToString();

            Text.text = count;
        }

    }
}