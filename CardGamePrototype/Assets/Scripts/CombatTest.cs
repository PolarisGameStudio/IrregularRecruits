using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatTest : MonoBehaviour
{
    public Button NextCombatButton;
    public CardUI CardPrefab;
    private Deck PlayerDeck;
    private Deck EnemyDeck;
    public Race[] AllRaces;
    public Creature[] AllCreatures;

    void Start()
    {
        if (AllCreatures.Length == 0)
            AllCreatures = Resources.FindObjectsOfTypeAll<Creature>();
        if (AllRaces.Length == 0)
            AllRaces = Resources.FindObjectsOfTypeAll<Race>();

        PlayerDeck = GenerateDeck(true);


        NextCombatButton.onClick.AddListener( NextCombat);
        Event.OnCombatFinished.AddListener(() => NextCombatButton.gameObject.SetActive(true));
    }

    private void NextCombat()
    {
        if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = GenerateDeck();

        CombatManager.StartCombat(PlayerDeck, EnemyDeck);

        NextCombatButton.gameObject.SetActive(false);
    }

    private Deck GenerateDeck(bool player = false)
    {
        var library = new List<Card>();

        var deck = new Deck(library,player);

        deck.PlayerDeck = player;



        Race[] possibleRaces = player ? AllRaces.Where(r => r.PlayerRace).ToArray() : AllRaces;
        var race = possibleRaces[Random.Range(0, possibleRaces.Length)];

        var creatures = AllCreatures.Where(c => c.Race == race).ToList();
        //creatures.OrderBy(c => Random.value);

        for (int i = 0; i < (GameSettings.DeckSize(player)); i++)
        {
            var ui = Instantiate<CardUI>(CardPrefab);

            var card = new Card(ui);

            ui.Card = card;

            card.InDeck = deck;

            card.Creature = creatures[Random.Range(0, creatures.Count())];

            library.Add(card);

            card.ChangeLocation(Deck.Zone.Library);

        }

        return deck;
    }
}
