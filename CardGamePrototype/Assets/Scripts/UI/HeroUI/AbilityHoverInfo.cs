using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    
    public class AbilityHoverInfo: Singleton<AbilityHoverInfo>, IPointerClickHandler
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public Button LevelUpButton;
        public GameObject Holder;
        public AbilityUI ShowingAbility;
        public LayoutGroup LayoutGroup;

        private void Start()
        {
            LevelUpButton.onClick.AddListener(LevelUpSelect);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Hide();
        }


        private void LevelUpSelect()
        {
            if (!ShowingAbility || !ShowingAbility.HeroViewAbility || !ShowingAbility.OutlineParticles.isPlaying)
                return;

            ShowingAbility.SelectLevelUp();

            Hide();
        }

        public static void Show(AbilityUI card)
        {
            if (Instance.ShowingAbility == card)
                Instance.Hide();
            else
                Instance.ShowAbility(card);
        }

        internal static bool IsActive()
        {
            return Instance.Holder.activeSelf;
        }

        private void ShowAbility(AbilityUI abilityUI)
        {
            ShowingAbility = abilityUI;

            Title.text = abilityUI.Ability.Name;
            Description.text = abilityUI.Ability.Description(abilityUI.Owner);

            LevelUpButton.gameObject.SetActive(abilityUI.HeroViewAbility && abilityUI.OutlineParticles.isPlaying);

            var rect = GetComponent<RectTransform>();
            rect.position = abilityUI.GetComponent<RectTransform>().position;

            Holder.transform.localScale = Vector3.zero;

            LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

            Holder.SetActive(true);

            Canvas.ForceUpdateCanvases();

            //this is ugly and hacky. but apparently the only way to make sure that the layoutgroups sizes are updated correctly.
            //remove, when/if unity fixes this
            LayoutGroup.enabled = false;
            LayoutGroup.enabled = true;
        }

        public  void Hide()
        {
            ShowingAbility = null;

            Holder.SetActive(false);

        }
    }
}