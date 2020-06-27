using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class HeroView : Singleton<HeroView>
    {
        public Image HeroImage;
        public TextMeshProUGUI HeroName;
        public TextMeshProUGUI XpText;
        public TextMeshProUGUI RaceText;
        public TextMeshProUGUI ClassText;
        public AbilityUI StartingAbility;

        public TextMeshProUGUI SelectLevelUpText;

        public GameObject Holder;

        public LevelAbility[] LevelAbilities;

        [Header("Ability looks")]
        //TODO: lerp saturation levels
        public Material GrayScaleMaterial;
        public Material NormalMaterial;
        public Color NormalAbilityColor, NotSelectedColor, UnselectableColor;

        [Serializable]
        public struct LevelAbility
        {
            public AbilityUI Race;
            public AbilityUI Class;
        }

        internal static void Close()
        {
            Instance.Holder.SetActive(false);
        }

        private void Start()
        {
            Event.OnLevelUpSelection.AddListener(Open);
            Event.OnLevelUp.AddListener(UpdateIcons);
        }

        public static void Open(Hero hero)
        {
            Instance.ShowHero(hero);
        }
        
        public static void Open()
        {
            if(BattleManager.Instance.PlayerDeck.Hero !=null)  
                Instance.ShowHero(BattleManager.Instance.PlayerDeck.Hero);
        }

        private void ShowHero(Hero hero)
        {
            Holder.SetActive(true);

            var raceopt = hero.HeroObject.RaceOption;
            var classopt = hero.HeroObject.Class;

            HeroImage.sprite = hero.HeroObject.Portrait;

            HeroName.text = hero.GetName();

            XpText.text = $" {hero.Xp} / {Hero.LevelCaps[hero.CurrentLevel]}";

            SelectLevelUpText.text = hero.LevelUpPoints > 0 ? "Select A New Skill!" : "";

            if (raceopt)
                RaceText.text = raceopt.name;

            if (classopt)
                ClassText.text = classopt.name;

            StartingAbility.SetAbility(hero.HeroObject.StartingAbility, hero);

            UpdateIcons(hero);
        }

        private void UpdateIcons(Hero hero)
        {

            var raceopt = hero.HeroObject.RaceOption;
            var classopt = hero.HeroObject.Class;

            for (int i = 0; i < LevelAbilities.Length; i++)
            {
                var opt = LevelAbilities[i];

                if (raceopt && raceopt.Options.Count > i)
                    opt.Race?.SetAbility(raceopt.Options[i], hero);

                if (classopt && classopt.Options.Count > i)
                    opt.Class?.SetAbility(classopt.Options[i], hero);
            }
        }
    }
}