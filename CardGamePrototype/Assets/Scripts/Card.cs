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
    private static bool ListenersCreated;

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
        private set
        {
            if (value > MaxHealth) value = MaxHealth;

            currentHealth = value;
            if (value <= 0) Die();
            OnStatChange.Invoke();
        }
    }

    public Card(Creature c)
    {
        Creature = c;

        if (!ListenersCreated) SetupListeners();
    }

    public List<Trait> GetTraits()
    {
        return Creature.Traits;
    }

    //Should only be called once
    private static void SetupListeners()
    {
        if (ListenersCreated) return;

        ListenersCreated = true;

        Event.OnDeath.AddListener(c => c.BattleRepresentation?.CardAnimation.Dissolve());
        Event.OnRessurrect.AddListener(c => c.BattleRepresentation?.CardAnimation.UnDissolve());

        //TODO: this delay should be handled by the flow controller
        Event.OnDeath.AddListener(c => c.ChangeLocation(Deck.Zone.Graveyard, 2f));
        Event.OnRessurrect.AddListener(c => c.ChangeLocation(Deck.Zone.Battlefield, 2f));

        Event.OnPlay.AddListener(c => c.ChangeLocation(Deck.Zone.Hand, Deck.Zone.Battlefield));

        Event.OnWithdraw.AddListener(c => c.ChangeLocation(Deck.Zone.Battlefield, Deck.Zone.Library));

        Event.OnDraw.AddListener(c => c.ChangeLocation(Deck.Zone.Library, Deck.Zone.Hand));
    }

    public Ability Ability()
    {
        return Creature.SpecialAbility;
    }

    internal bool Avantgarde()
    {
        return Creature.Traits.Contains(CombatManager.Instance.AvantgardeTrait);
    }


    public void Die()
    {
        //Debug.Log("Killing " + this);

        FlowController.AddEvent(()=>
            Event.OnDeath.Invoke(this));
    }
    

    private int attack;
    public int Attack
    {
        get => attack; 
        private set
        {
            attack = value;
            OnStatChange.Invoke();
        }
    }

    public bool FaceUp = true; 

    public Deck.Zone Location;

    public void ChangeLocation(Deck.Zone to,float delay = 0f)
    {
        ChangeLocation(Location, to,delay);
    }

    public void ChangeLocation(Deck.Zone from, Deck.Zone to, float delay = 0f)
    {
        //Debug.Log($"Moving {this.Name} from {from} to {to}. PLAYER: {InDeck.PlayerDeck}");

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

        BattleUI.Move(this, to, InDeck.PlayerDeck,delay);
    }

    public void SetCreature(Creature creature)
    {
        if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

        Name = creature?.name;// + " !" + Guid.NewGuid();

        //if (Creature && Creature.SpecialAbility)
        //    Creature.SpecialAbility.RemoveListeners(this);

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

    public bool Alive() => Location != Deck.Zone.Graveyard;

    public void StatModifier(int amount)
    {
        MaxHealth += amount;
        CurrentHealth += amount;
        Attack += amount;

        OnStatMod.Invoke(amount);

    }

    private void Flip(bool upsideUp)
    {
        FaceUp = upsideUp;

        BattleRepresentation?.Flip();
    }
    private void PlayCard()
    {
        if(BattleRepresentation)
            BattleRepresentation.Interactable = false;

        Debug.Log("Playing card");

        FlowController.AddEvent(() =>    Event.OnPlay.Invoke(this));
    }

    internal bool Ethereal()
    {
        return Creature.Traits.Contains(CombatManager.Instance.EtherealTrait);
    }

    internal bool CanAttack()
    {
        return Attack > 0 & !Ethereal();
    }

    public void Withdraw()
    {
        //TODO: replace with Waiting ON player Input

        FlowController.AddEvent(() =>
            Event.OnWithdraw.Invoke(this));

    }

    //should this method be called from OnRessurrect or the other way around?
    internal void Resurrect(int amount)
    {
        if (Location != Deck.Zone.Graveyard || Alive())
            Debug.LogWarning("Resurrectting alive character; "+ Name);

        FlowController.AddEvent(() =>
            Event.OnRessurrect.Invoke(this));

        CurrentHealth = amount;

        
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

    public void Damage(int damage)
    {
        if (damage < 1) return;

        OnDamage.Invoke(damage);
        FlowController.AddEvent(() => Event.OnDamaged.Invoke(this));

        CurrentHealth -= damage;
    }

    public void Heal(int value)
    {

        if (value < 1) return;

        FlowController.AddEvent(() =>
                Event.OnHealed.Invoke(this));

        CurrentHealth += value;
    }


    public void ResetAfterBattle()
    {
        ChangeLocation(Deck.Zone.Library);
        
        //todo: find a way to safe permanent buffs or drain
        if (Attack != Creature.Attack) Attack = Creature.Attack;
        if (MaxHealth != Creature.Health) MaxHealth= Creature.Health;

        CurrentHealth = MaxHealth;
    }

    public void Click()
    {
        if(InDeck == null)
        {
            Debug.Log("Clicked card not in deck");
            return;
        }

        if (CombatManager.PlayerActionsLeft <= 0)
        {
            Debug.Log("No player actions left");
            return;
        }

        if (!InDeck.PlayerDeck |! FaceUp )
            //|| (BattleRepresentation &!  BattleRepresentation.Interactable)) 
            return;

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
