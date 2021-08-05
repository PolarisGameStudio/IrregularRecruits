using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{

    public abstract class MapOptionObject : ScriptableObject
    {
        public bool ClosesLocationOnSelection = true;
        public List<Race> OnlyForHeroRaces = new List<Race>();
        //E.g. I use my fire magic to kill the ...
        public List<AbilityWithEffect> OnlyForAbility = new List<AbilityWithEffect>();
        [SerializeField]
        internal string OptionDescription;

        public abstract string Name { get; }
        public string PopUpDescription { get; set; }

        //maybe remove
        public virtual bool IsApplicable()
        {
            return
                (OnlyForHeroRaces.Count == 0 || OnlyForHeroRaces.Contains(Battle.PlayerDeck?.Hero?.GetRace()))
                &&
                (OnlyForAbility.Count == 0 ||Battle.PlayerDeck?.Hero != null && Battle.PlayerDeck.Hero.Abilities.Any(a=> OnlyForAbility.Contains(a)));
        }

        public abstract MapOption InstantiateMapOption();

        //Maybe?
        //public abstract MapOption GetMapOption();

    }
}