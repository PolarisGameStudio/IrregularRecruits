using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{

    public class Card : AbilityHolder
    {
        #region Fields

        private Creature creature;
        public Creature Creature
        {
            get => creature;             
            private set
            {
                SetCreature(value);
            }
        }

        public int MaxHealth;
        private int currentHealth;
        public int CurrentHealth
        {
            get => currentHealth;
            private set
            {
                if (value > MaxHealth) value = MaxHealth;

                currentHealth = value;
            }
        }

        internal int XpValue()
        {
            return Mathf.RoundToInt(creature.CR / 10f);
            //switch (creature.Rarity)
            //{
            //    case Creature.RarityType.Common:
            //        return 1;
            //    case Creature.RarityType.Rare:
            //        return 3;
            //    case Creature.RarityType.Unique:
            //        return 10;
            //    default:
            //        return 1;
            //}
        }

        public int Attack { get; private set; }

        public Deck.Zone Location { get; private set; }

        public bool Warded = false;




        #endregion

        //temporary if the card is only meant to display in deck selection or similar and not in deck to battle
        public Card(Creature c)
        {
            Creature = c;

            //this means that they are warded outisde of combat as well? intentional
            Warded = Ward();
        }

        public List<Trait> GetTraits()
        {
            return Creature.Traits;
        }

        public SpecialAbility Ability()
        {
            return Creature.SpecialAbility;
        }


        public void Die()
        {
            ChangeLocation(Deck.Zone.Graveyard);
        }

        public bool Damaged()
        {
            return CurrentHealth < MaxHealth;
        }

        public void ChangeLocation(Deck.Zone to)
        {
            ChangeLocation(Location, to);
        }

        public void ChangeLocation(Deck.Zone from, Deck.Zone to, bool noAbilityTrigger = false, int position = 0)
        {
            //Debug.Log($"Moving {this.Name} from {from} to {to}. indeck: {InDeck}");

            if (InDeck == null)
            {
                return;
            }

            if (!InDeck.CreaturesInZone(from).Contains(this))
            {
                return;
            }


            InDeck.CreaturesInZone(from).Remove(this);

            InDeck.CreaturesInZone(to).Add(this);

            Location = to;

            if (IsSummon() && to != Deck.Zone.Battlefield)
                Unsummon();

            //TODO: this null prop should not be nessecary!!? but some nullrefs occured
            InDeck?.SetPosition(this, to, position);

            if (noAbilityTrigger)
                return;

            Event.OnChangeLocation.Invoke(this, from, to);

            if (from == to)
                return;

            switch (to)
            {
                case Deck.Zone.Library:
                    //if(from == Deck.Zone.Battlefield)
                    Event.OnWithdraw.Invoke(this,Location);
                    break;
                case Deck.Zone.Battlefield:
                    Event.OnEtb.Invoke(this,Location);
                    break;
                case Deck.Zone.Graveyard:
                    this.ResetStats();
                    if(from == Deck.Zone.Battlefield)
                        Event.OnDeath.Invoke(this,Location);
                    break;
                case Deck.Zone.Hand:
                    Event.OnDraw.Invoke(this,Location);
                    break;
            }

        }

        public void PositionChanged(int position)
        {
            InDeck.SetPosition(this, InDeck.GetZone(this), position);
        }

        public void AttackCard(Card target)
        {
            if (!Alive() || !target.Alive())
                return;

            var returnDamage = !this.Ranged() && (target.Location == Deck.Zone.Battlefield);

            Event.OnReadyToAttack.Invoke(this, Location);

            Event.OnAttack.Invoke(this,Location);

            Event.OnBeingAttacked.Invoke(target,target.Location);

            if (Location != Deck.Zone.Battlefield || !target.Alive())
                return;

            int damageGiven = Mathf.Max(0, Attack);

            int damageTaken = Mathf.Max(0, target.Attack);

            var nextTo = target.Neighbours();

            target.HealthChange(-damageGiven);
            
            if(Carnage())
            {
                foreach (var neighbour in nextTo)
                    neighbour.HealthChange(-damageGiven);
            }

            if (returnDamage )
            {
                this.HealthChange(-damageTaken);

            }

            if (Lifedrain())
            {
                HealthChange(damageGiven);
            }

            if (returnDamage && target.Lifedrain())
                target.HealthChange(damageTaken);

            if (!Alive() && target.Alive()) Event.OnKill.Invoke(target,Location);
            else if (Alive() & !target.Alive()) Event.OnKill.Invoke(this,Location);

            Event.OnAttackFinished.Invoke(this, Location);
        }

        private List<Card> Neighbours()
        {
            var neighbours = new List<Card>();

            if (InDeck ==null) return neighbours;

            var zone = InDeck.CreaturesInZone(Location);

            var pos = zone.IndexOf(this);

            if (pos > 0)
                neighbours.Add(zone[pos - 1]);            
            
            if (pos < zone.Count-1)
                neighbours.Add(zone[pos + 1]);

            return neighbours;
        }

        private void Unsummon()
        {
            if (!IsSummon())
                return;

            //TODO: remove all listeners

            InDeck.Remove(this);

            Event.OnUnSummon.Invoke(this,Location);
        }

        public void SetCreature(Creature newCreature)
        {
            if (String.IsNullOrEmpty(newCreature.name)) newCreature.name = newCreature.ToString();

            Name = newCreature.name;// + " !" + Guid.NewGuid();

            MaxHealth = newCreature.Health;
            CurrentHealth = newCreature.Health;
            Attack = newCreature.Attack;

            if (creature && creature.SpecialAbility)
                creature.SpecialAbility.RemoveListeners(this);

            if (newCreature.SpecialAbility )
            {
                newCreature.SpecialAbility.SetupListeners(this);
            }

            this.creature = newCreature;
        }

        public bool Defender() =>
            Creature.Traits.Any(a => a.name == "Defender");
        public bool Ranged() =>
            Creature.Traits.Any(a => a.name == "Ranged");
        public bool Avantgarde() =>
            Creature.Traits.Any(a => a.name == "Avantgarde");
        public bool Ethereal() =>
            Creature.Traits.Any(a => a.name == "Ethereal");
        public bool Deathless() =>
            Creature.Traits.Any(a => a.name == "Deathless");
        public bool Assassin() =>
            Creature.Traits.Any(a => a.name == "Assassin");
        public bool Carnage() =>
            Creature.Traits.Any(a => a.name == "Carnage");
        public bool Ferocity() =>
            Creature.Traits.Any(a => a.name == "Ferocity");
        public bool Lifedrain() =>
            Alive () && Creature.Traits.Any(a => a.name == "Lifedrain");
        public bool Shapeshifter() =>
            Creature.Traits.Any(a => a.name == "Shapeshifter");
        public bool Ward() => Creature && Creature.Traits != null &&
            Creature.Traits.Any(a => a.name == "Ward");
        public bool IsSummon() =>
            Creature.IsSummon();

        public bool Alive() => Location != Deck.Zone.Graveyard;

        public void StatModifier(int amount)
        {
            MaxHealth += amount;
            CurrentHealth += amount;
            Attack += amount;

            Event.OnStatMod.Invoke(this, amount,Location);

            if (CurrentHealth <= 0)
                Die();

        }

        public void PlayCard(int position = 0)
        {
            ChangeLocation(Deck.Zone.Hand, Deck.Zone.Battlefield,false,position);

            Event.OnPlayerAction.Invoke(this.InDeck);

        }

        internal void Rally()
        {
            ChangeLocation(Deck.Zone.Library, Deck.Zone.Battlefield);
        }

        public void CleanListeners()
        {
            if (Creature && Creature.SpecialAbility )
                Creature.SpecialAbility.RemoveListeners(this);
        }

        internal bool CanAttack()
        {
            return Attack > 0 & !Ethereal();
        }

        public void Withdraw(bool playerAction)
        {
            ChangeLocation(Deck.Zone.Battlefield, Deck.Zone.Library);

            //otherwise it was caused by an ability and should not cost an action
            if(playerAction)
                Event.OnPlayerAction.Invoke(this.InDeck);
        }

        //should this method be called from OnRessurrect or the other way around?
        internal void Resurrect(int amount)
        {
            ChangeLocation(Deck.Zone.Battlefield);
            Event.OnRessurrect.Invoke(this,Location);

            var healthChange = amount - CurrentHealth;

            CurrentHealth = amount;

            Event.OnHealthChange.Invoke(this, healthChange,Location);
            //TODO: change race to UNDEAD?
        }

        internal void Charm(Deck moveToDeck)
        {
            //TODO: Demon + Undead not charmable? or is that fun?

            if (moveToDeck == InDeck)
            {
                return;
            }

            InDeck.Remove(this);

            InDeck = moveToDeck;

            moveToDeck.Add(this);

            ChangeLocation(Deck.Zone.Battlefield);
        }

        public void HealthChange(int change)
        {
            if (change == 0 || !Alive()) return;

            if(change < 0  && Warded && Location == Deck.Zone.Battlefield)
            {
                Warded = false;
                Event.OnWardTriggered.Invoke(this,Location);
                return;
            }

            CurrentHealth += change;

            Event.OnHealthChange.Invoke(this, change,Location);

            if (CurrentHealth <= 0) Die();

            if (change < 0)
                Event.OnDamaged.Invoke(this,Location);
            if (change > 0)
                Event.OnHealed.Invoke(this, change,Location);
        }



        public void ResetAfterBattle()
        {
            if (Location != Deck.Zone.Library)
                ChangeLocation(Location, Deck.Zone.Library, true);

            ResetStats();
        }

        private void ResetStats()
        {

            //todo: find a way to save permanent buffs or drain
            if (Attack != Creature.Attack) Attack = Creature.Attack;
            if (MaxHealth != Creature.Health) MaxHealth = Creature.Health;

            CurrentHealth = MaxHealth;
        }

        public void Activate(int position = 0)
        {
            if (InDeck == null || InDeck.DeckController == null)
            {
                return;
            }

            if (!InDeck.DeckController.ActionAvailable())
            {
                return;
            }

            if (Location == Deck.Zone.Hand)
            {
                PlayCard(position);
            }
            else if (Location == Deck.Zone.Battlefield)
            {
                Withdraw(true);
            }

        }

        public override Race GetRace()
        {
            return Creature.Race;
        }

        public bool IsRace(Race race) => Shapeshifter() || Creature.Race == race;

        internal override bool IsActive()
        {
            return Location == Deck.Zone.Battlefield;
        }
    }
}