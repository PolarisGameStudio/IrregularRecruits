using GameLogic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityHighlight : Singleton<AbilityHighlight>
    {
        public TextMeshProUGUI CardTitleText;
        public TextMeshProUGUI AbilityDescription;
        public TextMeshProUGUI AbilityTypeText;
        public ImageTextEntry AbilityIcon;
        public static Coroutine ShowAfterDelayRoutine;
        private Ability Ability;

        public GameObject Holder;

        private void Start()
        {
            Hide();
        }

        public static bool IsActive()
        {
            return Instance.Holder.activeSelf;
        }

        //DISABLE mask if creature has small image
        //remember color the same as health and attack of card
        public static void Show(AbilityUI ability)
        {
            if (ShowAfterDelayRoutine == null)
                ShowAfterDelayRoutine = Instance.StartCoroutine(Instance.ShowAfterDelay(ability));
        }

        private IEnumerator ShowAfterDelay(AbilityUI abili)
        {
            yield return new WaitForSeconds(0.3f);

            ShowCard(abili);
        }

        private void ShowCard(AbilityUI abilityui)
        {
            ShowAfterDelayRoutine = null;


            var rect = GetComponent<RectTransform>();
            rect.position = abilityui.GetComponent<RectTransform>().position;

            Instance.Holder.transform.localScale = Vector3.zero;

            LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

            Instance.Holder.SetActive(true);
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