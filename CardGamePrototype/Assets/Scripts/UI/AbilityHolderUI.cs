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

        internal Image HighlightAbility(Ability ability = null)
        {
            if (!ability || !AbilityIcons.ContainsKey(ability)) return HighlightAnimation(SpecialAbilityIcon);

            return HighlightAnimation(AbilityIcons[ability].AbilityImage);
        }

        private Image HighlightAnimation(Image abilityImage)
        {
            if (!abilityImage) return null;

            LeanTween.scale(abilityImage.rectTransform, Vector3.one * 3.5f, 0.4f).setOnComplete(() =>
                LeanTween.scale(abilityImage.rectTransform, Vector3.one, 0.3f));

            return abilityImage;
        }
    }
}