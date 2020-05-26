using GameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroUI : AbilityHolderUI
    {
        public GameObject Holder;
        public Button HeroImage;
        public List<AbilityUI> AbilityImages;
        public static HeroUI Instance;
        private Hero CurrentHero;

        private void Awake()
        {
            Instance = this;

            HeroImage.onClick.AddListener(() => HeroView.Open(CurrentHero));

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && CurrentHero != null)
                CurrentHero.AwardXp(Hero.LevelCaps[CurrentHero.CurrentLevel]);
        }


        public void SetHero(Hero hero)
        {
            if (hero == null)
            {
                Holder.SetActive(false);
                return;
            }

            CurrentHero = hero;

            Holder.SetActive(true);

            HeroImage.image.sprite = hero.HeroObject.Portrait;

            AbilityImages.ForEach(a=> a.gameObject.SetActive(false));

            if (AbilityImages.Count < hero.Abilities.Count) 
                Debug.LogError("not enough ability icons for Hero abilities");

            for (int i = 0; i < hero.Abilities.Count; i++)
            {
                var ui = AbilityImages[i];
                var abil = hero.Abilities[i];

                ui.SetAbility(abil,hero);

                ui.gameObject.SetActive(true);

                //TODO: set passive or active ability

                AbilityIcons[abil] = ui;
            }

        }

        public void LockAbilities()
        {
            foreach (var item in AbilityImages)
            {
                item.LockAbility();
            }
        }

        public void UnlockAbilities()
        {
            foreach (var item in AbilityImages)
            {
                item.SetAbilityAsActivable();
            }
        }

        public void Disable()
        {
            Holder.SetActive(false);
        }

    }

}