using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace GameLogic
{
    public class Hero : AbilityHolder
    {
        private HeroObject heroObject;
        public HeroObject HeroObject
        {
            get => heroObject; set
            {
                SetHeroObject(value);
            }
        }

        //public CardUI BattleRepresentation;

        public List<Ability> Abilities = new List<Ability>();

        public bool CanActivateAbility(Ability ability)
        {
            return InDeck.DeckController.ActionAvailable();
        }

        public int CurrentLevel = GetLevel(0);

        public static int[] LevelCaps = {  10, 20, 30, 50, 80, 130, 210, 340, 550, 890, 20000 };
        public static int GetLevel(int xp)
        {
            int level = 0;

            while (LevelCaps[level++] <= xp) { }

            return level - 1;
        }


        public int LevelUpPoints { get; private set; }

        private int xp = 0;

        public int Xp
        {
            get
            {
                return xp;
            }
            set
            {
                if (value == xp)
                    return;
                xp = value;
                var lvl = GetLevel((int)value);
                //TODO: should check for extra level 
                while (lvl > CurrentLevel)
                { 
                    LevelUpPoints++;
                    CurrentLevel++;
                    Event.OnLevelUp.Invoke(this);
                }
            }
        }
        
        public Hero(HeroObject h)
        {
            HeroObject = h;
        }

        public void AwardXp(int amount)
        {
            Xp += amount;
        }

        public void RemoveXp(int amount)
        {
            Xp -= amount;
        }

        public void SelectLevelUpAbility(Ability ability)
        {
            if (LevelUpPoints <= 0)
                return;

            if (!GetLevelUpOptions().Contains(ability))
                return;

            LevelUpPoints--;
            AddAbility(ability);
            Event.OnLevelUpSelection.Invoke(this);
        }

        public List<Ability> GetLevelUpOptions()
        {
            List<Ability> abilities = new List<Ability>();

            if(heroObject.RaceOption)
                abilities.AddRange(heroObject.RaceOption.Options.Take(GetLevel(Xp)));
            if (heroObject.Class)
                abilities.AddRange(heroObject.Class.Options.Take(GetLevel(Xp)));

            abilities.RemoveAll(Abilities.Contains);

            return abilities;

        }

        public void SetHeroObject(HeroObject hero)
        {
            if (String.IsNullOrEmpty(hero.name)) hero.name = hero.ToString();

            Name = hero.name;

            AddAbility(hero.StartingAbility);

            this.heroObject = hero;
        }

        public void AddAbility(Ability ability)
        {
            if (ability as PassiveAbility)
                (ability as PassiveAbility).SetupListeners(this);

            Abilities.Add(ability);

        }

        public override Race GetRace()
        {
            return heroObject.Race;
        }

    }
}