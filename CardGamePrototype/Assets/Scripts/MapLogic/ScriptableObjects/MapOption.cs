using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MapLogic
{
    public abstract class MapOption : ScriptableObject
    {
        public abstract void ExecuteOption(MapNode owner);

        public bool ClosesLocationOnSelection;
        public List<Race> OnlyForHeroRaces = new List<Race>();
        //E.g. I use my fire magic to kill the ...
        public List<Ability> OnlyForAbility = new List<Ability>();
        public string OptionDescription;
        public UnitCandidate AssociatedUnit;
        
        public enum UnitCandidate { NoUnit, Strong,Weak,Random, FriendlyRace,NonFriendlyRace}

        public virtual bool IsApplicable()
        {
            return
                (OnlyForHeroRaces.Count == 0 || OnlyForHeroRaces.Contains(BattleManager.Instance.PlayerDeck?.Hero?.GetRace()))
                &&
                (OnlyForAbility.Count == 0 ||BattleManager.Instance.PlayerDeck?.Hero != null && BattleManager.Instance.PlayerDeck.Hero.Abilities.Any(a=> OnlyForAbility.Contains(a)));



        }

    }
}