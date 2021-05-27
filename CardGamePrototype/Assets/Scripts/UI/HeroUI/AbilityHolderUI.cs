using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract partial class AbilityHolderUI : MonoBehaviour
    {
        protected List<Image> SpecialAbilityIcon = new List<Image>();
        public Dictionary<AbilityWithEffect, AbilityUI> AbilityIcons = new Dictionary<AbilityWithEffect, AbilityUI>();

        internal Image GetAbilityImage(AbilityWithEffect ability = null)
        {
            if (!ability || !AbilityIcons.ContainsKey(ability)) return SpecialAbilityIcon.FirstOrDefault(a => a.isActiveAndEnabled);

            return AbilityIcons[ability].AbilityImage;
        }

    }
}