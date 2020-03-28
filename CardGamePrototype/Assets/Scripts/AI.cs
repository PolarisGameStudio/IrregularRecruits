using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    private Deck ControlledDeck;

    public AI(Deck deck)
    {
        this.ControlledDeck = deck;
    }


    public void MakeMoves()
    {
        for (int i = 0; i < GameSettings.Instance.EnemyPlaysPrTurn; i++)
        {
            if (ControlledDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0)
                break;

            ControlledDeck.CreaturesInZone(Deck.Zone.Hand)[0].ChangeLocation(Deck.Zone.Battlefield);
        }
    }
}
