using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
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
                SetCreature(value);
            }
        }

        //public CardUI BattleRepresentation;

        public string Name;

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


        public void Die()
        {
            Event.OnDeath.Invoke(this);
        }

        internal bool Damaged()
        {
            return CurrentHealth < MaxHealth;
        }

        private int attack;
        public int Attack
        {
            get => attack;
            private set
            {
                attack = value;
            }
        }

        public bool FaceUp = true;

        public Deck.Zone Location;

        public void ChangeLocation(Deck.Zone to, float delay = 0f)
        {
            ChangeLocation(Location, to, delay);
        }

        public void ChangeLocation(Deck.Zone from, Deck.Zone to, float delay = 0f)
        {
            //Debug.Log($"Moving {this.Name} from {from} to {to}. PLAYER: {InDeck.PlayerDeck}");

            if (InDeck == null)
            {
                Debug.LogError("Not able to move card not in a deck");
                return;
            }

            if (!InDeck.CreaturesInZone(from).Contains(this))
            {
                Debug.LogWarning($"{Creature} not in correct zone: {from}");
                return;
            }
            
            InDeck.CreaturesInZone(from).Remove(this);

            Flip(to != Deck.Zone.Library & !(!InDeck.PlayerDeck && to == Deck.Zone.Hand));


            InDeck.CreaturesInZone(to).Add(this);

            Location = to;

            //TODO: this should be controlled by ui level
            //BattleUI.Move(this, to, InDeck.PlayerDeck, delay);
        }

        public void SetCreature(Creature newCreature)
        {
            if (String.IsNullOrEmpty(newCreature.name)) newCreature.name = newCreature.ToString();

            Name = newCreature?.name;// + " !" + Guid.NewGuid();

            //if (Creature && Creature.SpecialAbility)
            //    Creature.SpecialAbility.RemoveListeners(this);

            MaxHealth = newCreature.Health;
            CurrentHealth = newCreature.Health;
            Attack = newCreature.Attack;

            OnCreatureChange.Invoke(newCreature);

            if (creature?.SpecialAbility)
                creature.SpecialAbility.RemoveListeners();

            if (newCreature.SpecialAbility)
            {
                newCreature.SpecialAbility.SetupListeners(this);
            }

            this.creature = newCreature;
        }

        internal bool Defender() =>
            Creature.Traits.Any(a => a.name == "Defender");
        internal bool Ranged() =>
            Creature.Traits.Any(a => a.name == "Ranged");

        internal bool Avantgarde() =>
            Creature.Traits.Any(a => a.name == "Avantgarde");

        internal bool Ethereal() =>
            Creature.Traits.Any(a => a.name == "Ethereal");

        public bool Alive() => Location != Deck.Zone.Graveyard;

        public void StatModifier(int amount)
        {
            MaxHealth += amount;
            CurrentHealth += amount;
            Attack += amount;

            Event.OnStatMod.Invoke(this,amount);

        }

        private void Flip(bool upsideUp)
        {
            FaceUp = upsideUp;

        }
        public void PlayCard()
        {

            Debug.Log("Playing card: " + Name);

            Event.OnPlay.Invoke(this);

            Event.OnPlayerAction.Invoke(this.InDeck);
        }


        internal bool CanAttack()
        {
            return Attack > 0 & !Ethereal();
        }

        public void Withdraw()
        {
            //TODO: replace with Waiting ON player Input

            Event.OnWithdraw.Invoke(this);

        }

        //should this method be called from OnRessurrect or the other way around?
        internal void Resurrect(int amount)
        {
            if (Location != Deck.Zone.Graveyard || Alive())
                Debug.LogWarning("Resurrectting alive character; " + Name);

            Event.OnRessurrect.Invoke(this);

            CurrentHealth = amount;


            //TODO: change race to UNDEAD?
        }

        internal void Charm(Deck moveToDeck)
        {
            //TODO: Demon + Undead not charmable? or is that fun?

            if (moveToDeck == InDeck)
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


            CurrentHealth -= damage;  
            
            Event.OnDamaged.Invoke(this,damage);

        }

        public void Heal(int value)
        {

            if (value < 1) return;

            Event.OnHealed.Invoke(this,value);

            CurrentHealth += value;
        }


        public void ResetAfterBattle()
        {
            ChangeLocation(Deck.Zone.Library);

            //todo: find a way to safe permanent buffs or drain
            if (Attack != Creature.Attack) Attack = Creature.Attack;
            if (MaxHealth != Creature.Health) MaxHealth = Creature.Health;

            CurrentHealth = MaxHealth;
        }

        public void Click()
        {
            if (InDeck == null)
            {
                Debug.Log("Clicked card not in deck");
                return;
            }

            if (!InDeck.DeckController.ActionAvailable())
            {
                Debug.Log("No player actions left");
                return;
            }

            if (!InDeck.PlayerDeck | !FaceUp)
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
}