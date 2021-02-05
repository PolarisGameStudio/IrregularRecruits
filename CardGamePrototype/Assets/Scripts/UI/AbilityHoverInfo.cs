using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    
    public class AbilityHoverInfo: Singleton<AbilityHoverInfo>
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public GameObject Holder;

        public static Coroutine ShowAfterDelayRoutine;

        //DISABLE mask if creature has small image
        //remember color the same as health and attack of card
        public static void Show(AbilityUI card)
        {
            if (ShowAfterDelayRoutine == null)
                ShowAfterDelayRoutine = Instance.StartCoroutine(Instance.ShowAfterDelay(card));
        }

        private IEnumerator ShowAfterDelay(AbilityUI ability)
        {
            yield return new WaitForSeconds(0.3f);

            ShowAbility(ability);
        }

        internal static bool IsActive()
        {
            return Instance.Holder.activeSelf;
        }

        private void ShowAbility(AbilityUI abilityUI)
        {
            ShowAfterDelayRoutine = null;

            Title.text = abilityUI.Ability.Name;
            Description.text = abilityUI.Ability.Description(abilityUI.Owner);
            

            var rect = GetComponent<RectTransform>();
            rect.position = abilityUI.GetComponent<RectTransform>().position;

            Holder.transform.localScale = Vector3.zero;

            LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

            Holder.SetActive(true);
        }

        public static void Hide()
        {
            Instance.Holder.SetActive(false);

            if (ShowAfterDelayRoutine != null)
            {
                Instance.StopCoroutine(ShowAfterDelayRoutine);
                ShowAfterDelayRoutine = null;
            }
        }


    }
}