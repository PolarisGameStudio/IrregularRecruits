using GameLogic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

public class MapTestScene : MonoBehaviour
{
    private void Awake()
    {
        var deckObject = DeckLibrary.GetDecks().First();

        var heroObject = DeckLibrary.GetHeroes().First();
        Battle.SetPlayerDeck(deckObject);
        Battle.SetPlayerHero(new Hero(heroObject));

    }

    private void Start()
    {
        Event.OnGameBegin.Invoke();
    }
}
