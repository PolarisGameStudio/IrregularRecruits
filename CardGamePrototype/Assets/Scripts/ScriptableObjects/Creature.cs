using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class Creature : ScriptableObject
{
    public new string name;
    public RarityType Rarity;
    public bool Locked;
    public Guid Guid = new Guid();
    public Sprite Image;
    public int Attack = 1;
    public int Health = 1;
    public Trait[] Traits;
    public Race[] Races;
    [SerializeProperty("Race")]
    [SerializeField]
    private Race race;
    public Ability SpecialAbility;

    public Race Race { get {
            if (!race && Races.Length > 0)
                race = Races[0];

            return race; 
        } 
        set => race = value; }

    public enum RarityType { Common, Rare, Unique }
}
