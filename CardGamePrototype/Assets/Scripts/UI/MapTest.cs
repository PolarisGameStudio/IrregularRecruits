using GameLogic;
using System.Linq;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    private void Start()
    {
        var deckObject = DeckLibrary.GetDecks().First();

        var heroObject = DeckLibrary.GetHeroes().First();

        var deck = new Deck(deckObject);

        Hero hero = new Hero(heroObject);
        deck.Hero = hero;
        hero.InDeck = deck;

        BattleManager.Instance.PlayerDeck = deck;
    }
}
