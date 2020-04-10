using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatTest : Singleton<CombatTest>
{
    public Button NextCombatButton;
    private Deck PlayerDeck;
    private Deck EnemyDeck;
    public Creature TestCreature;
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

    public static void SetPlayerDeck(Deck deck)
    {
        Instance.PlayerDeck = deck;
    }


    private Deck GenerateDeck(bool player = false)
    {
        var library = new List<Card>();

        List<Creature> creatures;

        //Generate a deck full of the test creature
        if (TestCreature && player)
        {
            creatures = new List<Creature>() { TestCreature }; 
        }
        else
        {
            Race[] possibleRaces = player ? AllRaces.Where(r => r.PlayerRace).ToArray() : AllRaces;
            var race = possibleRaces[Random.Range(0, possibleRaces.Length)];

             creatures = AllCreatures.Where(c => c.Race == race).ToList();
            //creatures.OrderBy(c => Random.value);
        }

        for (int i = 0; i < (GameSettings.DeckSize(player)); i++)
        {
            var card = new Card(creatures[Random.Range(0, creatures.Count())]);
            
            library.Add(card);
        }

        var deck = new Deck(library, player);


        return deck;
    }
}
