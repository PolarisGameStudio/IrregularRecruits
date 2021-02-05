﻿using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        public Image ImageMask, Image;
        public static Coroutine ShowAfterDelayRoutine;
        private Creature Creature;

        private readonly List<ImageTextEntry> InstantiatedObjects = new List<ImageTextEntry>();

        public GameObject Holder;

        private void Start()
        {
            Hide();

            CardUI.OnUIDestroyed.AddListener(CardDestroyed);
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
            yield return new WaitForSeconds(0.5f);

            ShowCard(card);
        }

        private void ShowCard(CardUI cardUI)
        {
            if(cardUI.BeingDragged)
            {
                Hide();
                return;
            }

            ShowAfterDelayRoutine = null;

            Creature =  cardUI.Creature;

            InstantiatedObjects.ForEach(t => Destroy(t.gameObject));

            InstantiatedObjects.Clear();

            CardTitleText.text = Creature.name;

            if (Creature.IconImage)
            {
                Image.sprite = Creature.IconImage;
                ImageMask.enabled = false;
            }
            else
            {
                Image.sprite = Creature.Image;
                ImageMask.enabled = true;
            }

            //public TextMeshProUGUI AttackText;
            AttackText.text = //(cardUI.Card != null ? cardUI.Card.Attack.ToString("N0") :
                Creature.Attack.ToString("N0");
            AttackText.color = //(cardUI.Card != null ? cardUI.AttackText.color : 
                Color.white;

            //public TextMeshProUGUI HealthText;
            HealthText.text =
                //(cardUI.Card != null ? cardUI.Card.CurrentHealth.ToString("N0") + "/" + cardUI.Card.MaxHealth.ToString("N0") :
                Creature.Health.ToString("N0");
            HealthText.color = //(cardUI.Card != null ? cardUI.HealthText.color : 
                Color.white;

            //public ImageTextEntry RaceIcon;
            RaceIcon.Text.text = Creature.Race.name;
            RaceIcon.Image.sprite = Creature.Race.Icon;

            //public ImageTextEntry AbilityIcon;
            AbilityIcon.gameObject.SetActive(Creature.SpecialAbility);

            AbilityIcon.gameObject.SetActive(Creature.SpecialAbility);

            if (Creature.SpecialAbility)
            {
                AbilityIcon.Text.text = Creature.SpecialAbility?.Description(Creature);
                AbilityIcon.Image.sprite = IconLibrary.GetAbilityIconSprite(Creature.SpecialAbility.ResultingAction.ActionType);
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

            var rect = GetComponent<RectTransform>();
            rect.position = cardUI.GetComponent<RectTransform>().position;

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