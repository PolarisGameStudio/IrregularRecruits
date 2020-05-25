using GameLogic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class HeroSelectionUI : MonoBehaviour
    {
        public TextMeshProUGUI HeroName;
        public Button PreviousButton, NextButton;
        public Button HeroImage;
        private HeroObject SelectedHero;
        private List<HeroObject> AllHeroes;
        private int Chosen;

        private void Start()
        {
            AllHeroes = DeckLibrary.GetHeroes();

            PreviousButton.onClick.AddListener(Previous);
            NextButton.onClick.AddListener(Next);

            //HeroImage.onClick.AddListener(Showhero);

            Event.OnGameBegin.AddListener(SelectHero);

            Chosen = AllHeroes.Count;
        }

        private void Next()
        {
            Chosen++;

            if (Chosen > AllHeroes.Count) Chosen = 0;

            ChooseHero(Chosen);
        }
        private void Previous()
        {
            Chosen--;

            if (Chosen < 0) Chosen = AllHeroes.Count;

            ChooseHero(Chosen);
        }

        private void ChooseHero(int i)
        {
            Debug.Log("chosen:" + i);


            if(i >= AllHeroes.Count)
            {
                HeroImage.gameObject.SetActive(false);
                HeroName.text = "No Hero";
            }
            else
            {
                var chosen = AllHeroes[i];

                HeroImage.gameObject.SetActive(true);

                HeroImage.image.sprite = chosen.Portrait;
                HeroName.text = chosen.name;
            }

        }

        private void SelectHero()
        {
            if (Chosen < AllHeroes.Count)
                CombatPrototype.SetPlayerHero(AllHeroes[Chosen]);
        }
    }
}