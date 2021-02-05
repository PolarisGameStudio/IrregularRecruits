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
            get => creature; set
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
            return Mathf.RoundToInt(creature.CR / 4f);
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

        public Deck.Zone Location;



        #endregion

        public Card(Creature c)
        {
            Creature = c;

        }

        public List<Trait> GetTraits()
        {
            return Creature.Traits;
        }

        public PassiveAbility Ability()
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
                    Event.OnWithdraw.Invoke(this);
                    break;
                case Deck.Zone.Battlefield:
                    Event.OnEtb.Invoke(this);
                    break;
                case Deck.Zone.Graveyard:
                    Event.OnDeath.Invoke(this);
                    break;
                case Deck.Zone.Hand:
                    Event.OnDraw.Invoke(this);
                    break;
            }

            //TODO: this should be controlled by ui level
            //BattleUI.Move(this, to, InDeck.PlayerDeck, delay);
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

            Event.OnAttack.Invoke(this);

            Event.OnBeingAttacked.Invoke(target);

            if (Location != Deck.Zone.Battlefield || !target.Alive())
                return;

            target.HealthChange(- Mathf.Max(0, Attack));

            if (returnDamage)
                this.HealthChange(-Mathf.Max(0, target.Attack));

            if (!Alive() && target.Alive()) Event.OnKill.Invoke(target);
            else if (Alive() & !target.Alive()) Event.OnKill.Invoke(this);

        }

        private void Unsummon()
        {
            if (!IsSummon())
                return;

            //TODO: remove all listeners

            InDeck.Remove(this);

            Event.OnUnSummon.Invoke(this);
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
        internal bool Deathless() =>
            Creature.Traits.Any(a => a.name == "Deathless");
        internal bool IsSummon() =>
            Creature.IsSummon();

        public bool Alive() => Location != Deck.Zone.Graveyard;

        public void StatModifier(int amount)
        {
            MaxHealth += amount;
            CurrentHealth += amount;
            Attack += amount;

            Event.OnStatMod.Invoke(this, amount);

            if (CurrentHealth <= 0)
                Die();

        }

        public void PlayCard(int position = 0)
        {
            ChangeLocation(Deck.Zone.Hand, Deck.Zone.Battlefield,false,position);
            

        }

        internal void Rally()
        {
            ChangeLocation(Deck.Zone.Library, Deck.Zone.Battlefield);
        }

        public void CleanListeners()
        {
            if (Creature && Creature.SpecialAbility)
                Creature.SpecialAbility.RemoveListeners(this);
        }

        internal bool CanAttack()
        {
            return Attack > 0 & !Ethereal();
        }

        public void Withdraw()
        {
            //TODO: replace with Waiting ON player Input

            ChangeLocation(Deck.Zone.Battlefield, Deck.Zone.Library);

        }

        //should this method be called from OnRessurrect or the other way around?
        internal void Resurrect(int amount)
        {
            ChangeLocation(Deck.Zone.Battlefield);
            Event.OnRessurrect.Invoke(this);

            var healthChange = amount - CurrentHealth;

            CurrentHealth = amount;

            Event.OnHealthChange.Invoke(this, healthChange);
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

            CurrentHealth += change;

            Event.OnHealthChange.Invoke(this, change);

            if (CurrentHealth <= 0) Die();

            if (change < 0)
                Event.OnDamaged.Invoke(this);
            if (change > 0)
                Event.OnHealed.Invoke(this, change);
        }



        public void ResetAfterBattle()
        {
            if (Location != Deck.Zone.Library)
                ChangeLocation(Location, Deck.Zone.Library, true);

            //todo: find a way to safe permanent buffs or drain
            if (Attack != Creature.Attack) Attack = Creature.Attack;
            if (MaxHealth != Creature.Health) MaxHealth = Creature.Health;

            CurrentHealth = MaxHealth;
        }

        public void Click(int position = 0)
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
                Withdraw();
            }

            Event.OnPlayerAction.Invoke(this.InDeck);
        }

        public override Race GetRace()
        {
            return Creature.Race;
        }
    }
}