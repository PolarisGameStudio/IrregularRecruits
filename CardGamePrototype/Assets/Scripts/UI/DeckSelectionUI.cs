using GameLogic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{

    public class DeckSelectionUI : Singleton<DeckSelectionUI>, IUIWindow
    {
        public Icon DeckIconInstance;
        public Image DeckImage;
        public TextMeshProUGUI DeckTitle, DeckDescription;
        public Button ViewDeckButton;
        public GameObject Holder;
        public Button SettingsButton;

        private List<Icon> InstantiatedIcons = new List<Icon>();
        private DeckObject SelectedDeck;
        private Dictionary<DeckObject, Deck> Decks = new Dictionary<DeckObject, Deck>();
        private Dictionary<DeckObject, Icon> DeckIcons = new Dictionary<DeckObject, Icon>();

        public CanvasGroup FocusGroup;

        private void Awake()
        {
            foreach (var deck in DeckLibrary.GetDecks())
            {
                var icon = Instantiate(DeckIconInstance, DeckIconInstance.transform.parent);

                icon.Image.sprite = deck.DeckIcon;

                icon.Button.onClick.AddListener(() => SelectDeck( deck));

                InstantiatedIcons.Add(icon);

                Decks.Add(deck, new Deck(deck,true));
                DeckIcons[deck] = icon;
            }

            Destroy(DeckIconInstance.gameObject);

            InstantiatedIcons[0].Button.onClick.Invoke();

            SettingsButton.onClick.AddListener(() => UIController.Instance.Open(GameSettingsUI.Instance));
        }

        private void Start()
        {
            Event.OnGameOpen.Invoke();
        }

        public void SelectDeck( DeckObject d)
        {
            //TODO: have a dictionary and only pass the deckobject
            if (DeckIcons.ContainsKey(d))
            {
                var icon = DeckIcons[d];

                foreach (var ic in InstantiatedIcons)
                {
                    ic.ParticleSystem.gameObject.SetActive(ic == icon);
                }
            }


            SelectedDeck = d;

            DeckTitle.text = d.name;
            DeckDescription.text = d.Description;

            ViewDeckButton.onClick.RemoveAllListeners();
            ViewDeckButton.onClick.AddListener(() => DeckViewerUI.View(Decks[d]));

            DeckImage.sprite = d.DeckImage;
            LeanTween.scale(DeckImage.gameObject, Vector3.one, 2f);

        }

        public void Submit()
        {
            BattleManager.SetPlayerDeck(SelectedDeck);

            HeroSelectionUI.Instance.SubmitHero();

            Event.OnGameBegin.Invoke();

            UIController.Instance.Close(this);

            foreach(var d in Decks)
            {
                d.Value.Reset();
            }

            Decks.Clear();

            Destroy(gameObject);
            //LeanTween.alpha(gameObject, 0, 2f).setOnComplete(() => Destroy(gameObject));
        }

        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder()
        {
            return Holder;
        }

        public int GetPriority() => 5;
    }
}