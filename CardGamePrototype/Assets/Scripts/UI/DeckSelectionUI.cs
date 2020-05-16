using GameLogic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class DeckSelectionUI : MonoBehaviour
    {
        public Icon DeckIconInstance;
        public Image DeckImage;
        public TextMeshProUGUI DeckTitle, DeckDescription;
        public Button ViewDeckButton;

        private List<Icon> InstantiatedIcons = new List<Icon>();
        private DeckObject SelectedDeck;
        private Dictionary<DeckObject, Deck> Decks = new Dictionary<DeckObject, Deck>();

        private void Awake()
        {
            foreach (var deck in DeckLibrary.GetDecks())
            {
                var icon = Instantiate(DeckIconInstance, DeckIconInstance.transform.parent);

                icon.Image.sprite = deck.DeckIcon;

                icon.Button.onClick.AddListener(() => SelectDeck(icon, deck));

                InstantiatedIcons.Add(icon);

                Decks.Add(deck, new Deck(deck));
            }

            Destroy(DeckIconInstance.gameObject);

            InstantiatedIcons[0].Button.onClick.Invoke();
        }

        private void Start()
        {
            Event.OnGameOpen.Invoke();
        }

        private void SelectDeck(Icon i, DeckObject d)
        {
            SelectedDeck = d;

            foreach (var ic in InstantiatedIcons)
            {
                ic.ParticleSystem.gameObject.SetActive(ic == i);
            }

            DeckTitle.text = d.name;
            DeckDescription.text = d.Description;

            ViewDeckButton.onClick.RemoveAllListeners();
            ViewDeckButton.onClick.AddListener(() => DeckViewerUI.View(Decks[d]));

            DeckImage.sprite = d.DeckImage;
            LeanTween.scale(DeckImage.gameObject, Vector3.one, 2f);

        }

        public void Submit()
        {
            Event.OnGameBegin.Invoke();

            CombatPrototype.SetPlayerDeck(Decks[SelectedDeck]);

            Destroy(gameObject);
            //LeanTween.alpha(gameObject, 0, 2f).setOnComplete(() => Destroy(gameObject));
        }
    }
}