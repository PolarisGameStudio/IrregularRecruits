using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card 
{
    [SerializeProperty("Creature")]
    [SerializeField]
    private Creature creature;
    public Creature Creature
    {
        get => creature; set
        {
            creature = value;
            SetCreature(value);            
        }
    }

    public CardUI BattleRepresentation;

    public string Name;

    public UnityEvent OnStatChange = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();
    public class StatChangeEvent : UnityEvent<int> { }
    public StatChangeEvent OnDamage = new StatChangeEvent();
    public StatChangeEvent OnStatMod = new StatChangeEvent();
    public class CreatureChangeEvent : UnityEvent<Creature> { }
    public CreatureChangeEvent OnCreatureChange = new CreatureChangeEvent();
    public UnityEvent OnAbilityTrigger = new UnityEvent();

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
                OnDamage.Invoke(value- currentHealth);
                Event.OnDamaged.Invoke(this);
            }
            else if (value > currentHealth)
                Event.OnHealed.Invoke(this);

            if (value > MaxHealth) value = MaxHealth;

            currentHealth = value;
            if (value <= 0) Die();
            OnStatChange.Invoke();
        }
    }

    public Card(CardUI representation)
    {
        BattleRepresentation = representation;
    }

    public void Die()
    {
        //Debug.Log("Killing " + this);

        Event.OnDeath.Invoke(this);
        ChangeLocation(Location, Deck.Zone.Graveyard);
        OnDeath.Invoke();
    }

    private int attack;
    public int Attack
    {
        get => attack; set
        {
            attack = value;
            OnStatChange.Invoke();
        }
    }

    public bool FaceUp = true; 

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

    public void SetCreature(Creature creature)
    {
        if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

        Name = creature?.name;

        MaxHealth = creature.Health;
        CurrentHealth = creature.Health;
        Attack = creature.Attack;

        OnCreatureChange.Invoke(creature);

        //TODO: remove old listeners
        if (creature.SpecialAbility)
        {
            creature.SpecialAbility.SetupListeners(this);
        }
    }

    internal bool Defender() =>
        Creature.Traits.Any(a => a.name == "Defender");
    internal bool Ranged() =>
        Creature.Traits.Any(a => a.name == "Ranged");

    public bool Alive() => CurrentHealth > 0;


    internal void StatModifier(int amount)
    { 
        MaxHealth += amount;
        CurrentHealth += amount;
        Attack += amount;
    }

    private void Flip(bool upsideUp)
    {
        FaceUp = upsideUp;

        BattleRepresentation.Flip();
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
            Debug.LogWarning("Resurrectting alive character; "+ Name);

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

    public void Click()
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

}
