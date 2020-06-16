using GameLogic;
using System.Collections.Generic;
using UnityEngine;


namespace MapLogic
{
    public abstract class MapOption : ScriptableObject
    {
        public abstract void ExecuteOption(MapLocation owner);

        public bool ClosesLocationOnSelection;
        public List<Race> OnlyForHeroRaces;
        //E.g. I use my fire magic to kill the ...
        public List<Ability> OnlyForAbility;
        public string OptionDescription;
      
    }
}