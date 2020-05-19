using System;
using System.Collections.Generic;
using System.Linq;

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

        public string Name;
        private Guid Guid = System.Guid.NewGuid();

        public Deck InDeck;

        public List<Ability> Abilities;

        public int CurrentLevel = GetLevel(0);

        public static int[] LevelCaps = { 0, 10, 20, 30, 50, 80, 130, 210, 340, 550, 890, 20000 };
        public static int GetLevel(int xp)
        {
            int level = 0;

            while (LevelCaps[level++] <= xp) { }

            return level - 1;
        }

        public Hero(HeroObject h)
        {
            HeroObject = h;
        }


        public void SetHeroObject(HeroObject hero)
        {
            if (String.IsNullOrEmpty(hero.name)) hero.name = hero.ToString();

            Name = hero.name;

            AddAbility(hero.StartingAbility);

            this.heroObject = hero;
        }

        private void AddAbility(Ability ability)
        {
            if (ability as PassiveAbility)
                (ability as PassiveAbility).SetupListeners(this);

            Abilities.Add(ability);

        }

        public override Race GetRace()
        {
            return HeroObject.Race;
        }

        public override Deck GetDeck()
        {
            return InDeck;
        }

        public override Guid GetGuid() => Guid;
    }
}