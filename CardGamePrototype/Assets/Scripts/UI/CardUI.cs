﻿using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
    {
        public Creature Creature;

        [Header("UI Refs")]
        public GameObject FrontHolder, CardBackHolder;
        public Image CardImage;
        public Image IconImage;
        public Image RaceInstance;
        //TODO: replace with clickable
        public Image AttributeInstance;
        public CardAnimation CardAnimation;
        public TextMeshProUGUI AttackText;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI DescriptionText;
        public List<GameObject> InstantiatedObjects = new List<GameObject>();
        //For deck view and the like
        public bool AlwaysFaceUp;
        public bool Interactable = true;
        private UnityEvent OnClick = new UnityEvent();

        //Not equal to Card.health, since UI may be behind
        public int HealthValueDisplayed;
        private Color ReducedStatsColor = new Color(0.75f, 0.75f, 0.75f);

        public void SetCard(Card c)
        {
            UpdateCreature(c.Creature);
            UpdateStats(c.Attack,c.CurrentHealth,c.Damaged());
            OnClick.AddListener(c.Click);
        }

        public void UpdateCreature(Creature creature)
        {
            if (!creature) return;

            Creature = creature;

            if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

            if (IconImage)
                IconImage.sprite = creature.IconImage ? creature.IconImage : creature.Image;
            if (CardImage)
                CardImage.sprite = creature.Image;
            if (NameText)
                NameText.text = creature.name;

            name = creature?.name;

            InstantiatedObjects.ForEach(DestroyImmediate);
            InstantiatedObjects.Clear();

            if (DescriptionText)
                DescriptionText.text = "";

            if (RaceInstance)
                if (creature.Race)
                {
                    var instance = Instantiate(RaceInstance, RaceInstance.transform.parent);
                    instance.gameObject.SetActive(true);
                    instance.sprite = creature.Race.Icon;

                    InstantiatedObjects.Add(instance.gameObject);
                }


            if (AttributeInstance)
                foreach (var a in creature.Traits)
                {
                    var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                    instance.gameObject.SetActive(true);
                    instance.sprite = a.Icon;

                    DescriptionText.text += $"<b>{a.name}</b>\n";

                    InstantiatedObjects.Add(instance.gameObject);
                }

            if (AttributeInstance)
                if (creature.SpecialAbility)
                {
                    DescriptionText.text += $"{creature.SpecialAbility.Description(Creature)}\n";

                    var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                    instance.gameObject.SetActive(true);
                    instance.sprite = IconManager.GetAbilityIconSprite(creature.SpecialAbility.ResultingAction.ActionType);

                    InstantiatedObjects.Add(instance.gameObject);

                    CardAnimation.SpecialAbilityIcon = instance;
                }
        }

        public void StatModifier(int amount)
        {
            if (amount > 0)
                CardAnimation.StatPlusAnimation.Show(amount);
            else if (amount < 0)
                CardAnimation.StatMinusAnimation.Show(amount);
        }

        private void UpdateStats(int attack,int health,bool damaged = false)
        {
            if (!Creature) return;

            UpdateHealth(health, damaged);
            UpdateAttack(attack);
        }

        public void UpdateAttack(int attack)
        {
            AttackText.text = attack.ToString("N0");

            AttackText.color = Creature.Attack < attack ? Color.green :
                attack < Creature.Attack ? ReducedStatsColor :
                Color.white;
        }

        public void UpdateHealth(int health, bool damaged)
        {
            HealthText.text = health.ToString("N0");

            HealthText.color = damaged ? Color.red :
                health > Creature.Health ? Color.green :
                health < Creature.Health ? Color.gray :
                Color.white;
        }

        public bool FaceUp() => FrontHolder.activeInHierarchy;



        #region Input Handling
#if true
        public void OnPointerClick(PointerEventData eventData)
        {
#if !UNITY_EDITOR
        if (CardHighlight.IsActive()) return;
#endif
            if (Interactable && UIFlowController.Instance.EmptyQueue())
                OnClick.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardHighlight.Hide();

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AlwaysFaceUp || (FaceUp()))
                CardHighlight.Show(this);

        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        internal IEnumerator Flip(bool faceDown)
        {
            //already correct face up
            if (faceDown != FaceUp()) yield break;
            
            if (AlwaysFaceUp) yield break;

            var flipTime = 0.2f;

            gameObject.LeanScaleX(0, flipTime);
            
            yield return new WaitForSeconds(flipTime);

            CardBackHolder.SetActive(faceDown);
            FrontHolder.SetActive(!faceDown);

            gameObject.LeanScaleX(1, flipTime);

            yield return new WaitForSeconds(flipTime);
        }
#endif
#if false
    public void Update()
    {
        var touch = Input.GetTouch(0);

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            // ui touched
            CardHighlight.Show(this);
        }
        else if (touch.phase == TouchPhase.Ended)
            CardHighlight.Hide();

    }
#endif
        #endregion
    }
}