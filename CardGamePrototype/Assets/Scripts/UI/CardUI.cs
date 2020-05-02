using GameLogic;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

        //Not equal to Card.health, since UI may be behind
        public int HealthValueDisplayed;

        public void SetCard(Card c)
        {
            UpdateCreature(c.Creature);
            UpdateStats();
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

        internal void StatModifier(int amount)
        {
            if (amount > 0)
                CardAnimation.StatPlusAnimation.Show(amount);
            else if (amount < 0)
                CardAnimation.StatMinusAnimation.Show(amount);
        }

        private void UpdateStats()
        {
            if (!Creature) return;

            AttackText.text = Creature.Attack.ToString("N0");
            HealthText.text = Creature.Health.ToString("N0");
            //HealthText.color = Card.CurrentHealth < Card.MaxHealth ? Color.red :
            //    Card.Creature.Health < Card.MaxHealth ? Color.green : Color.white;

            //AttackText.color = Card.Creature.Attack < Card.Attack ? Color.green : Color.white;
        }

        public bool FaceUp() => FrontHolder.activeInHierarchy;

        //public void Flip()
        //{
        //    //todo: deck view/unflippable bool 
        //    if (AlwaysFaceUp) return;

        //    CardBackHolder.SetActive(!Card.FaceUp);
        //    FrontHolder.SetActive(Card.FaceUp);
        //}

            

        #region Input Handling
#if true
        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("Clicked card " + this);
            //TODO: hack to prevent holding from trigggering clicks
#if !UNITY_EDITOR
        if (CardHighlight.IsActive()) return;
#endif
            if (Interactable )//&& Card.BattleRepresentation == this)
                BattleUI.Clicked(this);
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