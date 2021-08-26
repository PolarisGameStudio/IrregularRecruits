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
        public Button ViewDeckButton;
        public GameObject Holder;
        public Button SettingsButton;
        public Button AchievementsButton;

        private DeckObject SelectedDeck;

        public CanvasGroup FocusGroup;

        private void Start()
        {
            Event.OnGameOpen.Invoke();
        }

        private void Awake()
        {
            AchievementsButton.onClick.AddListener(() => LegacyUI.Instance.Open());
        }

        public void SelectDeck( DeckObject d)
        {
            SelectedDeck = d;

            ViewDeckButton.onClick.RemoveAllListeners();
            ViewDeckButton.onClick.AddListener(() => DeckViewerUI.View(d));

        }

        public void Submit()
        {
            Battle.SetPlayerDeck(SelectedDeck);

            HeroSelectionUI.Instance.SubmitHero();

            Event.OnGameBegin.Invoke();

            UIController.Instance.Close(this);

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