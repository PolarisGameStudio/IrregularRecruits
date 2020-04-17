using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
{
    private Card card;
    public Card Card
    {
        get => card; 
        set
        {
            if(card != value)
            {
                RemoveListeners(card);
                card = value;
                AddListeners(card);
            }
        }
    }

    [Header("UI Refs")]
    public GameObject FrontHolder, CardBackHolder;
    public Image CardImage;
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

    private void AddListeners(Card c)
    {
        if (c == null) return;

        c.OnCreatureChange.AddListener(UpdateCreature);
        c.OnStatChange.AddListener(UpdateStats);
        //c.OnAbilityTrigger.AddListener(CardAnimation.HighlightAbility);
        c.OnStatMod.AddListener(StatModifier);
        c.OnDamage.AddListener(CardAnimation.DamageAnimation.Show);

        UpdateCreature(c.Creature);
        UpdateStats();
    }

    private void RemoveListeners(Card c)
    {
        if (c ==null) return;

        c.OnCreatureChange.RemoveListener(UpdateCreature);
        c.OnStatChange.RemoveListener(UpdateStats);
        //c.OnAbilityTrigger.RemoveListener(CardAnimation.HighlightAbility);
        c.OnStatMod.RemoveListener(StatModifier);
        c.OnDamage.RemoveListener(CardAnimation.DamageAnimation.Show);
    }

    public void UpdateCreature(Creature creature)
    {
        if (!creature) return;

        if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

        CardImage.sprite = creature.Image;
        NameText.text = creature.name;
        name = creature?.name;

        InstantiatedObjects.ForEach(DestroyImmediate);
        InstantiatedObjects.Clear();

        DescriptionText.text = "";

        if (creature.Race)
        {
            var instance = Instantiate(RaceInstance, RaceInstance.transform.parent);
            instance.gameObject.SetActive(true);
            instance.sprite = creature.Race.Icon;

            InstantiatedObjects.Add(instance.gameObject);
        }


        foreach (var a in creature.Traits)
        {
            var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
            instance.gameObject.SetActive(true);
            instance.sprite = a.Icon;

            DescriptionText.text += $"<b>{a.name}</b>\n";

            InstantiatedObjects.Add(instance.gameObject);
        }

        if (creature.SpecialAbility)
        {
            DescriptionText.text += $"{creature.SpecialAbility.Description(Card.Creature)}\n";

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
        if (!Card.Creature) return;

        AttackText.text = Card.Attack.ToString("N0");
        HealthText.text = Card.CurrentHealth.ToString("N0");
        HealthText.color = Card.CurrentHealth < Card.MaxHealth ? Color.red : 
            Card.Creature.Health < Card.MaxHealth ? Color.green : Color.white;

        AttackText.color = Card.Creature.Attack < Card.Attack ? Color.green : Color.white;
    }

    public void Flip()
    {
        //todo: deck view/unflippable bool 
        if (AlwaysFaceUp) return;
        
        CardBackHolder.SetActive(!Card.FaceUp);
        FrontHolder.SetActive(Card.FaceUp);
    }

    #region Input Handling
#if true
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked card " + this);
        //TODO: hack to prevent holding from trigggering clicks
#if !UNITY_EDITOR
        if (CardHighlight.IsActive()) return;
#endif

        Card.Click();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardHighlight.Hide();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ( AlwaysFaceUp || (Card != null && Card.FaceUp))
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
