using UnityEngine;
using UnityEngine.UI;
using GameLogic;
using Event = GameLogic.Event;
using System;
using UI;

namespace UI
{
    public class HeroIcon :MonoBehaviour {
        public Button Portrait;
        public Image LevelupIcon;
        private Hero Hero;

        private void Awake()
        {
            Portrait.onClick.AddListener(HeroView.Open);

            Event.OnHeroSelect.AddListener(UpdateIcon);
            BattleUI.OnLevelUp.AddListener(()=> UpdateIcon());
            Event.OnLevelUp.AddListener(UpdateIcon);
            Event.OnLevelUpSelection.AddListener(UpdateIcon);

        }


        private void UpdateIcon(Hero hero = null)
        {
            if (hero == null)
                hero = Hero;
            else
                Hero = hero;

            Portrait.image.sprite = hero.HeroObject.Portrait;


            var before = LevelupIcon.enabled;
            bool lvlUp = hero.LevelUpPoints > 0;
            
            LevelupIcon.enabled = lvlUp ;

            if (lvlUp //& !before 
                //&& GetComponentInParent<IUIWindow>().GetCanvasGroup().interactable 
                && gameObject.activeInHierarchy)
            {
                AnimationSystem.PlayLevelupFX(transform.position);
                BattleUI.OnLevelAnimation.Invoke();
            }

        }
    }

}