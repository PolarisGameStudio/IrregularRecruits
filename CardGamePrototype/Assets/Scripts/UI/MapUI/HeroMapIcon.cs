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

        private void Awake()
        {
            Portrait.onClick.AddListener(HeroView.Open);

            Event.OnHeroSelect.AddListener(ChangeIcon);

        }

        private void ChangeIcon(Hero hero)
        {
            Portrait.image.sprite = hero.HeroObject.Portrait;
        }
    }

}