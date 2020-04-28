using System.Collections.Generic;

public class DeckManager : Singleton<DeckManager>
{
    public List<DeckObject> Decks;

    public static List<DeckObject> GetDecks()
    {
#if UNITY_EDITOR
        if (Instance.Decks == null || Instance.Decks.Count == 0)
            Instance.Decks = AssetManager.GetAssetsOfType<DeckObject>();
#endif

        return Instance.Decks;

    }
}
