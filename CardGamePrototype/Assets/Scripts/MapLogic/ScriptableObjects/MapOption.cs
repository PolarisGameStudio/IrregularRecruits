using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    public abstract class MapOption : ScriptableObject
    {
        public abstract void ExecuteOption(MapNode owner);

        public bool ClosesLocationOnSelection = true;
        public List<Race> OnlyForHeroRaces = new List<Race>();
        //E.g. I use my fire magic to kill the ...
        public List<Ability> OnlyForAbility = new List<Ability>();
        private string OptionDescription;
        public UnitCandidate AssociatedUnit;

        private readonly string FirstUnitEscapeString = "U0";
        private readonly string SecondUnitEscapeString = "U1";
        private readonly string ThirdUnitEscapeString = "U2";
        
        public enum UnitCandidate { NoUnit, Strong,Weak,Random, FriendlyRace,NonFriendlyRace}

        public virtual bool IsApplicable()
        {
            return
                (OnlyForHeroRaces.Count == 0 || OnlyForHeroRaces.Contains(BattleManager.Instance.PlayerDeck?.Hero?.GetRace()))
                &&
                (OnlyForAbility.Count == 0 ||BattleManager.Instance.PlayerDeck?.Hero != null && BattleManager.Instance.PlayerDeck.Hero.Abilities.Any(a=> OnlyForAbility.Contains(a)));
        }

        public string GetOptionDescription(MapNode owner)
        {
            var str = OptionDescription;

            //if we have not associated any units
            if (!owner.SelectedCards.ContainsKey(this))
                return OptionDescription;

            if (OptionDescription.Contains(FirstUnitEscapeString) && owner.SelectedCards[this].Count > 0)
                str = str.Replace(FirstUnitEscapeString, owner.SelectedCards[this][0].GetName());

            if (OptionDescription.Contains(SecondUnitEscapeString) && owner.SelectedCards[this].Count > 1)
                str = str.Replace(SecondUnitEscapeString, owner.SelectedCards[this][1].GetName());

            if (OptionDescription.Contains(ThirdUnitEscapeString) && owner.SelectedCards[this].Count > 2)
                str = str.Replace(ThirdUnitEscapeString, owner.SelectedCards[this][2].GetName());

            return str;
        }
    }
}