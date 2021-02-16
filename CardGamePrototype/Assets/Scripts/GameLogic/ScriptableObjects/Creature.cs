using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Creature")]
    public class Creature : ScriptableObject, ICharacter
    {
        public new string name;
        public RarityType Rarity;
        public bool Locked;
        public Sprite Image;
        public Sprite IconImage;
        public int Attack = 1;
        public int Health = 1;
        public List<Trait> Traits = new List<Trait>();
        public SpecialAbility SpecialAbility;
        public int CR;

        public List<DeckStrategy> Enabling;
        public List<DeckStrategy> Payoff;


        public Race Race;

        public float Performance;

        public enum RarityType { Common, Rare, Unique }

        public bool IsSummon()
        {
            return Traits.Any(a => a.name == "Summoned");
        }

        public string GetName()
        {
            return name;
        }

        public Race GetRace()
        {
            return Race;
        }

    }
}