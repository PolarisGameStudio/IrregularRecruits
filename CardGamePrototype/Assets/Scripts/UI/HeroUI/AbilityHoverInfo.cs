using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    
    public class AbilityHoverInfo: Singleton<AbilityHoverInfo>
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public Button LevelUpButton;
        public GameObject Holder;
        public AbilityUI ShowingAbility;
        public LayoutGroup LayoutGroup;
        private Coroutine HidingRoutine;
        public CanvasGroup CanvasGroup;

        private void Start()
        {
            LevelUpButton.onClick.AddListener(LevelUpSelect);
        }

        private void Update()
        {
            if (!Holder.activeInHierarchy) return;

            if (Input.GetMouseButtonDown(0) || (Input.touches.Any(t => t.phase == TouchPhase.Began)))
                Hide();
        }


        private void LevelUpSelect()
        {
            if (!ShowingAbility || !ShowingAbility.IsLevelUpOption)
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
            //so we don't hide the window we are opnening.
            if (HidingRoutine != null)
                StopCoroutine(HidingRoutine);

            ShowingAbility = abilityUI;

            Title.text = abilityUI.Ability.Name;
            Description.text = abilityUI.Ability.Description(abilityUI.Owner);

            LevelUpButton.gameObject.SetActive(abilityUI.IsLevelUpOption);

            var rect = GetComponent<RectTransform>();
            rect.position = abilityUI.GetComponent<RectTransform>().position;

            Holder.transform.localScale = Vector3.zero;

            CanvasGroup.alpha = 1f;

            LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

            Holder.SetActive(true);

            Canvas.ForceUpdateCanvases();

            //this is ugly and hacky. but apparently the only way to make sure that the layoutgroups sizes are updated correctly.
            //remove, when/if unity fixes this
            LayoutGroup.enabled = false;
            LayoutGroup.enabled = true;
        }

        public void Hide()
        {
            HidingRoutine = StartCoroutine(HideRoutine());
        }

        private IEnumerator HideRoutine()
        {
            CanvasGroup.alpha = 0f;

            yield return new WaitForSeconds(0.2f);

            ShowingAbility = null;

            Holder.SetActive(false);

            HidingRoutine = null;

        }
    }
}