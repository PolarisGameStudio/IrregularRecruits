using GameLogic;
using System;
using System.Collections.Generic;

public class DeckLibrary : Singleton<DeckLibrary>
{
    public List<DeckObject> Decks;
    public List<HeroObject> Heroes;

    public static List<DeckObject> GetDecks()
    {
//#if UNITY_EDITOR
//        if (Instance.Decks == null || Instance.Decks.Count == 0)
//            Instance.Decks = AssetManager.GetAssetsOfType<DeckObject>();
//#endif

        return Instance.Decks;

    }

    internal static List<HeroObject> GetHeroes()
    {
        return Instance.Heroes ;
    }
}
