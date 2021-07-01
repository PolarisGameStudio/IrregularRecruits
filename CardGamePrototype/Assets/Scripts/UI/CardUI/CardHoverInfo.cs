using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{

    public class CardHoverInfo : Singleton<CardHoverInfo>
    {
        public TextMeshProUGUI CardTitleText;
        public TextMeshProUGUI AttackText;
        public TextMeshProUGUI HealthText;
        public ImageTextEntry RaceIcon;
        public ImageTextEntry AbilityIcon;
        public ImageTextEntry TraitPrefab;
        public Image ImageMask, ImageTopCrop, ImageBottomCrop;
        public static Coroutine ShowAfterDelayRoutine;
        private Creature Creature;
        [HideInInspector]
        public UnityEvent OnCardHighlight = new UnityEvent();
        public RectTransform RaceTransform;

        private readonly List<ImageTextEntry> InstantiatedObjects = new List<ImageTextEntry>();

        public GameObject Holder;

        private void Start()
        {
            Hide();

            CardUI.OnUIDestroyed.AddListener(CardDestroyed);
        }

        private void Update()
        {
            if (!Holder.activeInHierarchy) return;

            if (Input.GetMouseButtonDown(0) || (Input.touches.Any(t => t.phase == TouchPhase.Began)))
                Hide();
        }

        private void CardDestroyed(CardUI arg0)
        {
            if (arg0.Creature == Creature)
            {
                //Debug.Log("Card destroyed, hiding hover info");
                Hide();
            }
            //todo should also check if it is the same ui instance.. since different uis can have the same creature
        }

        public static bool IsActive()
        {
            return Instance.Holder.activeSelf;
        }

        public static Creature GetCreature()
        {
            if (!IsActive()) return null;

            return Instance.Creature;
        }

        //DISABLE mask if creature has small image
        //remember color the same as health and attack of card
        public static void Show(CardUI card)
        {
            if (ShowAfterDelayRoutine == null)
                ShowAfterDelayRoutine = Instance.StartCoroutine(Instance.ShowAfterDelay(card));
        }

        private IEnumerator ShowAfterDelay(CardUI card)
        {
            yield return new WaitForSeconds(0.0f);

            ShowCard(card);
        }

        private void ShowCard(CardUI cardUI)
        {
            if(cardUI.BeingDragged)
            {
                Hide();
                return;
            }

            OnCardHighlight.Invoke();

            ShowAfterDelayRoutine = null;

            Creature =  cardUI.Creature;

            InstantiatedObjects.ForEach(t => Destroy(t.gameObject));

            InstantiatedObjects.Clear();

            CardTitleText.text = Creature.name;

            //if (Creature.IconImage)
            //{
            //    Image.sprite = Creature.IconImage;
            //    ImageMask.enabled = false;
            //}
            //else
            //{
                ImageTopCrop.sprite = ImageBottomCrop.sprite =  Creature.Image;

            ImageBottomCrop.enabled = Creature.TopFocused;
            ImageTopCrop.enabled = !Creature.TopFocused;

            //ImageMask.enabled = true;
            //}

            //public TextMeshProUGUI AttackText;
            AttackText.text = cardUI.AttackValueDisplayed.ToString();
            AttackText.color = cardUI.AttackValueDisplayed > Creature.Attack ? Color.green :
                cardUI.AttackValueDisplayed < Creature.Attack ? Color.gray : Color.white;

            //public TextMeshProUGUI HealthText;
            HealthText.text = $"{ cardUI.HealthValueDisplayed}/{cardUI.MaxHealthValueDisplayed}";
            HealthText.color = cardUI.HealthValueDisplayed < cardUI.MaxHealthValueDisplayed ? Color.red :
                cardUI.MaxHealthValueDisplayed > Creature.Health ? Color.green :  
                cardUI.MaxHealthValueDisplayed < Creature.Health ? Color.gray :  
                Color.white;


            //public ImageTextEntry RaceIcon;
            RaceIcon.Text.text = Creature.Race.name;
            RaceIcon.Image.sprite = Creature.Race.Icon;

            //public ImageTextEntry AbilityIcon;
            AbilityIcon.gameObject.SetActive(Creature.SpecialAbility);

            AbilityIcon.gameObject.SetActive(Creature.SpecialAbility);

            if (Creature.SpecialAbility )
            {
                AbilityIcon.Text.text = Creature.SpecialAbility?.Description(Creature);

                if (Creature.SpecialAbility is PassiveAbility ability)
                    AbilityIcon.Image.sprite = IconLibrary.GetAbilityIconSprite(ability.ResultingAction.ActionType);
                
                if (Creature.SpecialAbility is EffectDoublerAbility ability1)
                    AbilityIcon.Image.sprite = IconLibrary.GetAbilityIconSprite(ability1.Effect);
                
                if (Creature.SpecialAbility is TriggerDoublerAbility ability2)
                    AbilityIcon.Image.sprite = IconLibrary.GetAbilityIconSprite(ability2.EffectTrigger);

            }

            //public ImageTextEntry TraitPrefab;
            foreach (var t in Creature.Traits)
            {
                var trait = Instantiate(TraitPrefab, TraitPrefab.transform.parent);
                trait.Text.text = $" <b>{t.name}</b> <i>({t.Description})</i>";
                trait.gameObject.SetActive(true);
                trait.Image.sprite = t.Icon;

                InstantiatedObjects.Add(trait);
            }

            RaceTransform?.SetAsLastSibling();

            var rect = GetComponent<RectTransform>();
            rect.position = cardUI.GetComponent<RectTransform>().position;

            Holder.transform.localScale = Vector3.zero;

            LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

            var left = transform.position.x > 0;
            var top = transform.position.y < 0;

            var rectHolder =
                Holder.GetComponent<RectTransform>();

            rectHolder.pivot = new Vector2(left ? 1 : 0, top ? 0 : 1);
            rectHolder.anchorMin = rectHolder.anchorMax = new Vector2(left ? 0 : 1, top ? 0 : 1);


            Holder.SetActive(true);

            Canvas.ForceUpdateCanvases();

            //this is ugly and hacky. but apparently the only way to make sure that the layoutgroups sizes are updated correctly.
            //remove, when/if unity fixes this
            AbilityIcon.LayoutGroup.enabled = false;
            AbilityIcon.LayoutGroup.enabled = true;
            InstantiatedObjects.ForEach(o => o.LayoutGroup.enabled = false);
            InstantiatedObjects.ForEach(o => o.LayoutGroup.enabled = true);
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