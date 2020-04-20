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
    public DeckObject DeckObject;

    private Dictionary<Zone, List<Card>> Creatures = new Dictionary<Zone, List<Card>>();

    public Deck(DeckObject deckObject,bool playerDeck)
        : this(deckObject.Creatures.Select(c => new Card(c)).ToList(), playerDeck)
    {
        DeckObject = deckObject;
    }

    public Deck(List<Card> initialLibrary,bool playerDeck)
    {
        for (int i = (int)Zone.Library; i < (int)Zone.COUNT; i++)
        {
            Creatures[(Zone)i] = new List<Card>();
        }

        PlayerDeck = playerDeck;


        foreach (var card in initialLibrary)
        {
            AddCreature(card);
        }

        if (!playerDeck)
            AI = new AI(this);

    }

    public void AddCreature(Card card)
    {
        Debug.Log("adding card to deck: " + card.Name);

        card.InDeck = this;
        card.Location = Zone.Library;

        Creatures[Zone.Library].Add(card);
    }

    public List<Card> CreaturesInZone(Zone z)
        => Creatures[z];

    internal void DrawInitialHand(bool enemy = false)
    {
        ShuffleLibrary();

        var amountToDraw = GameSettings.StartingHandSize(enemy);

        //Move AVANTGARDE cards to the front and shuffle the rest
        foreach(var c in CreaturesInZone(Zone.Library).Where(c=>c.Avantgarde()))
        {
            Draw(c);
            amountToDraw--;
        }

        if (enemy)
        {
            for (int i = 0; i < GameSettings.Instance.EnemyBattlefieldSize; i++)
                MoveTopCardToBattleField();
        }
        
        Draw(amountToDraw);
    }


    internal void PackUp()
    {
        //Debug.Log("Packing deck");

        //removing dead creatures
        while (Creatures[Zone.Graveyard].Any())
            Remove(Creatures[Zone.Graveyard].First());    
        
        foreach(var c in AllCreatures())
        {
            c.ResetAfterBattle();
        }
    }

    public void ShuffleLibrary()
    {
        Debug.Log("shuffling deck");
        Creatures[Zone.Library] = Creatures[Zone.Library].OrderBy(x => Random.value).ToList();
    }
       
    public void Draw(int amount)
    {
        if (Creatures[Zone.Library].Count() == 0 || amount < 0) return;
        if (Creatures[Zone.Library].Count() < amount) amount = Creatures[Zone.Library].Count();

        var draws = Creatures[Zone.Library].Take(amount);

        foreach (var card in draws)
        {
            Draw(card);
        }
    }

    public void Draw(Card card)
    {
        FlowController.AddEvent(() => Event.OnDraw.Invoke(card));
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
        if (CreaturesInZone(Zone.Battlefield).Count > 0)
        {
            List<Card> battlefield = CreaturesInZone(Zone.Battlefield).Where(c=> !c.Ethereal()).ToList();
            var defenders = battlefield.Where(c => c.Defender()).ToList();

            if (defenders.Any())
                return defenders[Random.Range(0, defenders.Count())];

            if (!battlefield.Any()) //meaning only ethereals
                return CreaturesInZone(Zone.Battlefield)[Random.Range(0, CreaturesInZone(Zone.Battlefield).Count)];

            return battlefield[Random.Range(0, battlefield.Count())];
        }
        else if (CreaturesInZone(Zone.Hand).Count > 0)
        {
            return CreaturesInZone(Zone.Hand)[Random.Range(0, CreaturesInZone(Zone.Hand).Count())];
        }
        else if (CreaturesInZone(Zone.Library).Count > 0)
        {
            return CreaturesInZone(Zone.Library)[Random.Range(0, CreaturesInZone(Zone.Library).Count())];
        }
        else return null;
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
