using GameLogic;
using System.Linq;
using UnityEngine;
using Event = GameLogic.Event;

public class MapTest : MonoBehaviour
{
    private void Awake()
    {
        var deckObject = DeckLibrary.GetDecks().First();

        var heroObject = DeckLibrary.GetHeroes().First();

        var deck = new Deck(deckObject);

        Hero hero = new Hero(heroObject);
        deck.Hero = hero;
        hero.InDeck = deck;

        BattleManager.Instance.PlayerDeck = deck;

    }

    private void Start()
    {
        Event.OnGameBegin.Invoke();
    }
}
