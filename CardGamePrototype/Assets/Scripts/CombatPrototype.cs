using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;

public class CombatPrototype : Singleton<CombatPrototype>
{
    public Button NextCombatButton;
    private Deck PlayerDeck;
    private Deck EnemyDeck;
    public Creature TestCreature;
    public Race[] AllRaces;
    public Creature[] AllCreatures;

    public int CombatDifficultyIncrease, CombatRarityIncrease;

    void Start()
    {
        if (AllCreatures.Length == 0)
            AllCreatures = Resources.FindObjectsOfTypeAll<Creature>();
        if (AllRaces.Length == 0)
            AllRaces = Resources.FindObjectsOfTypeAll<Race>();

        PlayerDeck = GenerateDeck(true);

        NextCombatButton.onClick.AddListener(NextCombat);
        Event.OnBattleFinished.AddListener(() => NextCombatButton.gameObject.SetActive(true));
    }

    private void NextCombat()
    {
        if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = GenerateDeck();

        GameSettings.Instance.EnemyDeckSize += CombatDifficultyIncrease;
        GameSettings.Instance.MaxRareEnemiesPrCombat += CombatRarityIncrease;

        Event.OnCombatSetup.Invoke(PlayerDeck, EnemyDeck);

        NextCombatButton.gameObject.SetActive(false);
    }

    public static void SetPlayerDeck(Deck deck)
    {
        Instance.PlayerDeck = deck;
    }

    public void GetNewMinions()
    {
        AddMinionScreen.SetupMinionScreen(PlayerDeck);
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
        }

        var rares = creatures.Where(c => c.Rarity == Creature.RarityType.Rare || c.Rarity == Creature.RarityType.Unique).ToList();
        var notRares = creatures.Except(rares).ToList();
        if (!notRares.Any())
            notRares = rares;

        for (int i = 0; i < (GameSettings.DeckSize(player)); i++)
        {
            if (rares.Any())
            {
                for (; i < GameSettings.Instance.MaxRareEnemiesPrCombat; i++)
                {
                    var rare = new Card(rares[Random.Range(0, rares.Count())]);

                    library.Add(rare);
                }
            }


            var card = new Card(notRares[Random.Range(0, notRares.Count())]);

            library.Add(card);
        }

        var deck = new Deck(library, player);


        return deck;
    }
}
