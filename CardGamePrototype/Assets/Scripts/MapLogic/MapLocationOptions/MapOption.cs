using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    public abstract class MapOption : IMapLocation
    {
        public bool ClosesLocationOnSelection = true;
        public List<Race> OnlyForHeroRaces = new List<Race>();
        //E.g. I use my fire magic to kill the ...
        public List<AbilityWithEffect> OnlyForAbility = new List<AbilityWithEffect>();
        internal string OptionDescription;
        public UnitCandidate AssociatedUnit;

        private readonly string FirstUnitEscapeString = "U0";
        private readonly string SecondUnitEscapeString = "U1";
        private readonly string ThirdUnitEscapeString = "U2";

        public string Name { set; get; }
        public string PopUpDescription { get; set; }

        public enum UnitCandidate { NoUnit, Strong, Weak, Random, FriendlyRace, NonFriendlyRace }

        public MapOption(MapOptionObject optionObject)
        {
            Name = optionObject.Name;
            PopUpDescription = optionObject.PopUpDescription;
            OnlyForAbility = optionObject.OnlyForAbility;
            OnlyForHeroRaces = optionObject.OnlyForHeroRaces;
            ClosesLocationOnSelection = optionObject.ClosesLocationOnSelection;
            OptionDescription = optionObject.OptionDescription;
        }

        public MapOption() { }

        public abstract void ExecuteOption(MapNode owner);


        public virtual bool IsApplicable()
        {
            return
                (OnlyForHeroRaces.Count == 0 || OnlyForHeroRaces.Contains(Battle.PlayerDeck?.Hero?.GetRace()))
                &&
                (OnlyForAbility.Count == 0 || Battle.PlayerDeck?.Hero != null && Battle.PlayerDeck.Hero.Abilities.Any(a => OnlyForAbility.Contains(a)));
        }

        public virtual string GetOptionDescription(MapNode owner, string overrideDescription = "")
        {
            var str = overrideDescription != "" ? overrideDescription : OptionDescription;

            //if we have not associated any units
            if (!owner.SelectedCards.ContainsKey(this))
                return str;

            if (str.Contains(FirstUnitEscapeString) && owner.SelectedCards[this].Count > 0)
                str = str.Replace(FirstUnitEscapeString, owner.SelectedCards[this][0].GetName());

            if (str.Contains(SecondUnitEscapeString) && owner.SelectedCards[this].Count > 1)
                str = str.Replace(SecondUnitEscapeString, owner.SelectedCards[this][1].GetName());

            if (str.Contains(ThirdUnitEscapeString) && owner.SelectedCards[this].Count > 2)
                str = str.Replace(ThirdUnitEscapeString, owner.SelectedCards[this][2].GetName());

            return str;
        }

        internal virtual void FindCandidate(MapNode mapNode)
        {
            Func<Card, bool> predicate;



            switch (AssociatedUnit)
            {
                case UnitCandidate.NoUnit:
                    return;
                case UnitCandidate.Strong:
                case UnitCandidate.Weak:
                case UnitCandidate.Random:
                case UnitCandidate.FriendlyRace:
                case UnitCandidate.NonFriendlyRace:
                default:
                    {
                        predicate = d => !mapNode.SelectedCards.Values.Any(v => v.Contains(d));

                        break;
                    }
            }

            var unit = Battle.PlayerDeck.AllCreatures().FirstOrDefault(predicate);

            if (unit == null)
                unit = Battle.PlayerDeck.AllCreatures().FirstOrDefault();

            if (unit != null) mapNode.AddAssociation(this, unit);
        }

        public MapOption[] GetLocationOptions()
        {
            return new MapOption[] { this };
        }

        public void Open(MapNode node)
        {
            ExecuteOption(node);

            if (ClosesLocationOnSelection)
                node.Close();
        }

        public bool IsStartNode()
        {
            return false;
        }

        public bool IsWinNode()
        {
            return false;
        }

        public bool IsUniqueNode()
        {
            return false;
        }

        public abstract float Difficulty();
        public virtual MapNodeType GetLocationType()
        {
            return MapNodeType.Event;
        }
    }
}