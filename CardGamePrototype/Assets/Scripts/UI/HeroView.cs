using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroView : Singleton<HeroView>
    {
        public Material OutlineMaterial;

        public Image HeroImage;
        public TextMeshProUGUI HeroName;
        public TextMeshProUGUI XpText;
        public TextMeshProUGUI RaceText;
        public TextMeshProUGUI ClassText;
        public AbilityUI StartingAbility;

        public LevelAbility[] LevelAbilities;

        [Serializable]
        public struct LevelAbility
        {
            public AbilityUI Race;
            public AbilityUI Class;
        }


    }
}