using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck 
{
    public enum Zone { Library, Battlefield, Graveyard, Hand, COUNT }
    public bool PlayerDeck;
    public AI AI;

    private Dictionary<Zone, List<Card>> Creatures = new Dictionary<Zone, List<Card>>();

    public Deck(DeckObject deckObject,bool playerDeck)
        : this(deckObject.Creatures.Select(c => new Card(c)).ToList(), playerDeck)
    {
    }

    public Deck(List<Card> initialLibrary,bool playerDeck)
    {
        for (int i = (int)Zone.Library; i < (int)Zone.COUNT; i++)
        {
            Creatures[(Zone)i] = new List<Card>();
        }

        PlayerDeck = playerDeck;


        Creatures[Zone.Library] = initialLibrary;

        foreach (var card in initialLibrary)
        {
            card.InDeck = this;
            card.Location = Zone.Library;
        }

        if (!playerDeck)
            AI = new AI(this);

    }
    
    public List<Card> CreaturesInZone(Zone z)
        => Creatures[z];

    internal void DrawInitialHand(bool enemy = false)
    {
        ShuffleLibrary();

        //Move AVANTGARDE cards to the front
        var lib = Creatures[Zone.Library];

        if (enemy)
        {
            for (int i = 0; i < GameSettings.Instance.EnemyBattlefieldSize; i++)
                MoveTopCardToBattleField();
        }
        
        Draw(GameSettings.StartingHandSize(enemy));
    }

    internal void PackUp()
    {
        //Debug.Log("Packing deck");

        //removing dead creatures
        while (Creatures[Zone.Graveyard].Any())
            Remove(Creatures[Zone.Graveyard].First());        

        while(Creatures.Any(z => z.Key != Zone.Library && z.Value.Any()))
        {
            var c = Creatures.First(z => z.Key != Zone.Library && z.Value.Any()).Value[0];

            c.ChangeLocation(Zone.Library);
            c.CurrentHealth = c.MaxHealth;
        }
        
    }

    public void ShuffleLibrary()
    {
        Debug.Log("shuffling deck");
        Creatures[Zone.Library] = Creatures[Zone.Library].OrderBy(x => Random.value).ToList();
    }



    public void Draw(int amount)
    {
        if (Creatures[Zone.Library].Count() == 0) return;
        if (Creatures[Zone.Library].Count() < amount) amount = Creatures[Zone.Library].Count();

        var draws = Creatures[Zone.Library].Take(amount);

        foreach (var card in draws)
        {
            FlowController.AddEvent(() => Event.OnDraw.Invoke(card));
        }
    }

    internal List<Card> AllCreatures()
    {
        return Creatures.SelectMany(x => x.Value).ToList();
    }

    public void MoveTopCardToBattleField()
    {
        if (Creatures[Zone.Library].Count() == 0) return;

        Creatures[Zone.Library][0].ChangeLocation(Zone.Library, Zone.Battlefield);
    }

    //returns count of all creatures not in Graveyard
    public int Alive() => Creatures.Sum(a => a.Key == Zone.Graveyard ? 0 : a.Value.Count);

    internal Card GetAttackTarget()
    {
        //empty list check?
        if (CreaturesInZone(Zone.Battlefield).Count == 0) return null;

        var defenders = CreaturesInZone(Zone.Battlefield).Where(c => c.Defender()).ToList();

        if (defenders.Any())
            return defenders[Random.Range(0, defenders.Count())];

        return CreaturesInZone(Zone.Battlefield)[Random.Range(0, CreaturesInZone(Zone.Battlefield).Count())];
    }

    internal void Remove(Card card)
    {
        Creatures[card.Location].Remove(card);
        card.InDeck = null;
    }

    internal void Add(Card card)
    {
        Creatures[card.Location].Add(card);
    }
}
