using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class Creature : ScriptableObject
    {
        public new string name;
        public RarityType Rarity;
        public bool Locked;
        public Guid Guid = new Guid();
        public Sprite Image;
        public Sprite IconImage;
        public int Attack = 1;
        public int Health = 1;
        public List<Trait> Traits;
        public Ability SpecialAbility;

        public Race Race;


        public enum RarityType { Common, Rare, Unique }

        internal bool IsSummon()
        {
            return Traits.Any(a => a.name == "Summoned");
        }
    }
}