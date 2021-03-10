using UnityEngine;
using UnityEngine.UI;
using GameLogic;
using Event = GameLogic.Event;
using System;
using UI;

namespace MapUI
{
    public class HeroMapIcon :MonoBehaviour {
        public Button Portrait;
        public Image LevelupIcon;

        private void Awake()
        {
            Portrait.onClick.AddListener(HeroView.Open);

            Event.OnHeroSelect.AddListener(UpdateIcon);
            Event.OnLevelUp.AddListener(UpdateIcon);
            Event.OnLevelUpSelection.AddListener(UpdateIcon);

        }

        private void UpdateIcon(Hero hero)
        {
            Portrait.image.sprite = hero.HeroObject.Portrait;
            LevelupIcon.enabled = hero.LevelUpPoints > 0;
        }
    }

}