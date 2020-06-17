using GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DeckLibrary : SingletonScriptableObject<DeckLibrary>
{
    public List<DeckObject> Decks;
    public List<HeroObject> Heroes;

    public static List<DeckObject> GetDecks()
    {
        return Instance.Decks;

    }

    internal static List<HeroObject> GetHeroes()
    {
        return Instance.Heroes ;
    }
}
