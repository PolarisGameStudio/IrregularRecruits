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
        public Dictionary<SpecialAbility, AbilityUI> AbilityIcons = new Dictionary<SpecialAbility, AbilityUI>();

        internal Image GetAbilityImage(SpecialAbility ability = null)
        {
            if (!ability || !AbilityIcons.ContainsKey(ability)) return SpecialAbilityIcon.FirstOrDefault(a => a.isActiveAndEnabled);

            return AbilityIcons[ability].AbilityImage;
        }

    }
}