using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    //controls the miniature deck in the top right
    [RequireComponent(typeof(Button))]
    public class DeckIcon : MonoBehaviour
    {
        public Button DeckButton;
        public int CardWidth;
        public int CardHeight;


        private void Awake()
        {
            DeckButton = GetComponent<Button>();

            DeckButton.onClick.AddListener(DeckViewerUI.ViewPlayerDeck);

        }

    }

}