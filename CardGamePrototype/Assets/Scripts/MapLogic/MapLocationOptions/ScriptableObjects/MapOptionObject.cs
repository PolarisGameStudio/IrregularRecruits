using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{

    public class MapOptionObject : ScriptableObject
    {
        public string Name { get; }
        public string PopUpDescription { get; set; }
        public bool ClosesLocationOnSelection = true;
        public List<Race> OnlyForHeroRaces = new List<Race>();
        //E.g. I use my fire magic to kill the ...
        public List<AbilityWithEffect> OnlyForAbility = new List<AbilityWithEffect>();
        [SerializeField]
        internal string OptionDescription;


        [Header("End game options")]
        public bool EndsGame;
        public bool Win;

        //TODO: make custom inspector, that turns on/off relevant sections
        [Header("Combat options")]
        public bool StartsCombat;
        public List<Creature> SpawnCreatures = new List<Creature>();
        public Race MainRace;
        public List<Race> AdditionalRaces = new List<Race>();
        public int CRValue;
        public bool UniquesAllowed;

        [Header("Gains/loses gold/xp")]
        public int XpAmount;
        public int GoldAmount;

        [Header("Gain/Lose units")]
        public bool LoseUnit;
        public List<Creature> GainUnits;


        //maybe remove
        public virtual bool IsApplicable()
        {
            return
                (OnlyForHeroRaces.Count == 0 || OnlyForHeroRaces.Contains(Battle.PlayerDeck?.Hero?.GetRace()))
                &&
                (OnlyForAbility.Count == 0 || Battle.PlayerDeck?.Hero != null && Battle.PlayerDeck.Hero.Abilities.Any(a => OnlyForAbility.Contains(a)));
        }

        public MapOption InstantiateMapOption()
        {
            var options = new List<MapOption>();

            if (StartsCombat)
                options.Add(new CombatOption(this));

            if (EndsGame)
                options.Add(new EndGameOption(this));

            if (XpAmount > 0)
                options.Add(new GainXpOption(this));

            if (XpAmount < 0)
                options.Add(new LoseXPOption(this));

            if (GoldAmount > 0)
                options.Add(new GainGoldOption(this));

            if (GoldAmount < 0)
                options.Add(new LoseGoldOption(this));

            if (LoseUnit)
                options.Add(new LoseUnitOption(this));

            if (GainUnits.Any())
                options.Add(new GainUnitOption(this));

            if (!options.Any())
                return new NoEffectOption(this);

            if (options.Count() == 1) return options.First();

            return new MapOptionComposite(this, options);
        }

        //Maybe?
        //public abstract MapOption GetMapOption();

    }
}