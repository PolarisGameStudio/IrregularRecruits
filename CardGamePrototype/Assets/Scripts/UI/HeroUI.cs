using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroUI : Singleton<HeroUI>
    {
        public GameObject Holder;
        public Image HeroImage;
        public Image AbilityImagePrefab;


        public void SetHero(Hero hero)
        {
            if (hero == null)
            {
                Holder.SetActive(false);
                return;
            }

            Holder.SetActive(true);

            HeroImage.sprite = hero.HeroObject.Portrait;

            AbilityImagePrefab.gameObject.SetActive(false);

            foreach (var abil in hero.Abilities)
            {
                var ui = Instantiate(AbilityImagePrefab, AbilityImagePrefab.transform.parent);

                ui.sprite = abil.Icon;

                ui.gameObject.SetActive(true);
            }

        }
    }

}