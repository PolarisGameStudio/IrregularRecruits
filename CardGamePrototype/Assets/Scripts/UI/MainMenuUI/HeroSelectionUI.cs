using GameLogic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class HeroSelectionUI : Singleton<HeroSelectionUI>
    {
        public TextMeshProUGUI HeroName;
        public TextMeshProUGUI RaceText, ClassText;
        public Button PreviousButton, NextButton;
        public Image HeroImage;
        public Button HeroButton;
        private Hero SelectedHero;
        private List<HeroObject> SelectableHeroes;
        private int Chosen;
        private Dictionary<HeroObject, Hero> InstantiatedHeroes = new Dictionary<HeroObject, Hero>(); 

        private void Start()
        {
            LoadHeroes();

            LegacySystem.Instance.OnLegaciesLoaded.AddListener(LoadHeroes);

            PreviousButton.onClick.AddListener(Previous);
            NextButton.onClick.AddListener(Next);

            Chosen = SelectableHeroes.Count;

            HeroButton.onClick.AddListener(() => HeroView.Open(SelectedHero));

            Next();
        }

        private void LoadHeroes()
        {
            SelectableHeroes = DeckLibrary.GetHeroes(false);
        }

        private void Next()
        {
            Chosen++;

            if (Chosen >= SelectableHeroes.Count) Chosen = 0;

            ChooseHero(Chosen);
        }
        private void Previous()
        {
            Chosen--;

            if (Chosen < 0) Chosen = SelectableHeroes.Count-1;

            ChooseHero(Chosen);
        }

        private void ChooseHero(int i)
        {
            if(i >= SelectableHeroes.Count)
            {
                HeroImage.gameObject.SetActive(false);
                HeroName.text = "No Hero";
            }
            else
            {
                var chosen = SelectableHeroes[i];

                if (!InstantiatedHeroes.ContainsKey(chosen))
                    InstantiatedHeroes[chosen] = new Hero(chosen);

                SelectedHero = InstantiatedHeroes[chosen];

                HeroImage.gameObject.SetActive(true);

                HeroImage.sprite = chosen.Portrait;
                HeroName.text = chosen.name;

                RaceText.text = chosen.RaceOption.name;
                RaceText.color = chosen.RaceOption.Color;
                ClassText.text = chosen.Class.name;
                ClassText.color = chosen.Class.Color;

                if (chosen.Deck)
                    DeckSelectionUI.Instance.SelectDeck(chosen.Deck);

            }

        }

        public void SubmitHero()
        {
            if (SelectedHero!= null)
                Battle.SetPlayerHero(SelectedHero);
        }
    }
}