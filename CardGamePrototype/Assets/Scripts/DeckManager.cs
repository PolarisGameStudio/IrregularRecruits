using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : Singleton<DeckManager>
{
    public List<DeckObject> Decks;

    public static List<DeckObject> GetDecks()
    {
#if UNITY_EDITOR
        if (Instance.Decks == null ||Instance.Decks.Count == 0)
            Instance.Decks = CardGeneration.GetAssetsOfType<DeckObject>();
#endif

        return Instance.Decks;

    }
}
