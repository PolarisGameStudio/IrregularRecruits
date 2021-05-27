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
        BattleManager.SetPlayerDeck(deckObject);
        BattleManager.SetPlayerHero(heroObject);

    }

    private void Start()
    {
        Event.OnGameBegin.Invoke();
    }
}
