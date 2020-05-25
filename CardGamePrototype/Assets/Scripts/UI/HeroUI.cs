using GameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroUI : AbilityHolderUI
    {
        public GameObject Holder;
        public Image HeroImage;
        public List<AbilityUI> AbilityImages;
        public static HeroUI Instance;

        private void Awake()
        {
            Instance = this;

        }


        public void SetHero(Hero hero)
        {
            Debug.Log("updating hero");

            if (hero == null)
            {
                Holder.SetActive(false);
                return;
            }

            Holder.SetActive(true);

            HeroImage.sprite = hero.HeroObject.Portrait;

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