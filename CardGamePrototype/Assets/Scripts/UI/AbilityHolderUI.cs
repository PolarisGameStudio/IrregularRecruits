using GameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract partial class AbilityHolderUI : MonoBehaviour
    {
        protected Image SpecialAbilityIcon;
        public Dictionary<Ability, AbilityUI> AbilityIcons = new Dictionary<Ability, AbilityUI>();

        internal Image GetAbilityImage(Ability ability = null)
        {
            if (!ability || !AbilityIcons.ContainsKey(ability)) return SpecialAbilityIcon;

            return AbilityIcons[ability].AbilityImage;
        }

    }
}