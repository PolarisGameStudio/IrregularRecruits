using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform))]
public class Card : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IDragHandler
{
    [SerializeProperty("Creature")]
    [SerializeField]
    private Creature creature;
    public Creature Creature
    {
        get => creature; set
        {
            creature = value;
            UpdateCreature(value);            
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

    [Header("Battle specific")]
    public Deck InDeck;
    public int MaxHealth;
    private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (currentHealth > value)
            {
                CardAnimation.DamageAnimation.Show(value- currentHealth);
                Event.OnDamaged.Invoke(this);
            }
            else if (value > currentHealth)
                Event.OnHealed.Invoke(this);



            HealthText.color = value < MaxHealth ? Color.red : creature.Health < MaxHealth ? Color.green : Color.white;

            if (value > MaxHealth) value = MaxHealth;

            currentHealth = value;
            if (value <= 0) Die();
            UpdateStats();
        }
    }

    public void Die()
    {
        //Debug.Log("Killing " + this);

        Event.OnDeath.Invoke(this);
        ChangeLocation(Location, Deck.Zone.Graveyard);
        CardAnimation.Dissolve();// () => ChangeLocation(Location, Deck.Zone.Graveyard));
    }

    private int attack;
    public int Attack
    {
        get => attack; set
        {
            attack = value;
            AttackText.color = creature.Attack < value ? Color.green : Color.white;
            UpdateStats();
        }
    }
    public Deck.Zone Location;

    public void ChangeLocation(Deck.Zone to)
    {
        ChangeLocation(Location, to);
    }
    public void ChangeLocation(Deck.Zone from, Deck.Zone to)
    {
        //Debug.Log($"Moving {this} from {from} to {to}. PLAYER: {InDeck.PlayerDeck}");

        if (InDeck == null)
        {
            Debug.LogError("Not able to move card not in a deck");
            return;
        }

        Flip(to != Deck.Zone.Library & !(!InDeck.PlayerDeck && to == Deck.Zone.Hand));

        if (!InDeck.CreaturesInZone(from).Contains(this))
            Debug.LogWarning($"{Creature} not in correct zone: {from}");
        else InDeck.CreaturesInZone(from).Remove(this);

        InDeck.CreaturesInZone(to).Add(this);

        Location = to;

        //TODO: handle from BattleUI class
        //transform.SetParent( BattleUI.GetZoneHolder(to,!InDeck.PlayerDeck),false);

        //if(from != Deck.Zone.Library && to == Deck.Zone.Library)
        //    BattleUI.MoveCardToLibrary(this);
        BattleUI.Move(this, to, InDeck.PlayerDeck);
    }

    public void UpdateCreature(Creature creature)
    {
        if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

        CardImage.sprite = creature.Image;
        NameText.text = creature.name;
        name = creature?.name;

        MaxHealth = creature.Health;
        CurrentHealth = creature.Health;
        Attack = creature.Attack;

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

        if (creature.SpecialAbility)
        {
            creature.SpecialAbility.SetupListeners(this);
            DescriptionText.text += $"{creature.SpecialAbility.Description(this)}\n";
        }

        foreach (var a in creature.Traits)
        {
            var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
            instance.gameObject.SetActive(true);
            instance.sprite = a.Icon;

            DescriptionText.text += $"<b>{a.name}</b>\n";

            InstantiatedObjects.Add(instance.gameObject);
        }
    }

    private void UpdateStats()
    {
        AttackText.text = Attack.ToString("N0");
        HealthText.text = CurrentHealth.ToString("N0");
    }

    internal bool Defender() =>
        Creature.Traits.Any(a => a.name == "Defender");
    internal bool Ranged() =>
        Creature.Traits.Any(a => a.name == "Ranged");

    public bool Alive() => CurrentHealth > 0;


    internal void StatModifier(int amount)
    {
        if (amount > 0)
            CardAnimation.StatPlusAnimation.Show(amount);
        else if (amount < 0)
            CardAnimation.StatMinusAnimation.Show(amount);

        MaxHealth += amount;
        CurrentHealth += amount;
        Attack += amount;
    }

    private void Flip(bool upsideUp)
    {
        CardBackHolder.SetActive(!upsideUp);
        FrontHolder.SetActive(upsideUp);
    }
    private void PlayCard()
    {
        if (CombatManager.PlayerActionsLeft <= 0)
        {
            Debug.Log("No player actions left");
            return;
        }

        ChangeLocation(Deck.Zone.Hand, Deck.Zone.Battlefield);

        Event.OnPlay.Invoke(this);

        CombatManager.PlayerActionsLeft--;
    }

    public void Withdraw()
    {
        //TODO: replace with Waiting ON player Input
        if (CombatManager.PlayerActionsLeft <= 0)
        {
            Debug.Log("No player actions left");
            return;
        }

        ChangeLocation(Deck.Zone.Battlefield, Deck.Zone.Library);

        Event.OnDraw.Invoke(this);

    }

    internal void Resurrect(int amount)
    {
        if (Location != Deck.Zone.Graveyard || Alive())
            Debug.LogWarning("Resurrectting alive character; "+ name);

        CurrentHealth = amount;
        ChangeLocation(Deck.Zone.Graveyard, Deck.Zone.Battlefield);

        //TODO: change race to UNDEAD?
    }

    internal void Charm(Deck moveToDeck)
    {
        //TODO: Demon + Undead not charmable? or is that fun?

        if(moveToDeck == InDeck)
        {
            Debug.Log($"cannot charm to own dekc; {this}");
        }

        InDeck.Remove(this);

        InDeck = moveToDeck;

        moveToDeck.Add(this);

        ChangeLocation(Deck.Zone.Battlefield);
    }

    #region Input Handling
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked card " + this);

        if (Location == Deck.Zone.Hand)
        {
            PlayCard();
        }
        else if (Location == Deck.Zone.Battlefield)
        {
            Withdraw();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardHighlight.Hide();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardHighlight.Show(this);

    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    #endregion
}
