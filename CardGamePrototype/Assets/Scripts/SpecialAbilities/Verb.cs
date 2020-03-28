using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Verb 
{
    public Verb() { }

    public abstract Ability.Verb VerbType { get; }
}
