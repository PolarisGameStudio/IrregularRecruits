﻿using System;
using UnityEngine;

namespace GameLogic
{
    public class PlayerController : DeckController
    {

        public Hero PlayerHero;

        public PlayerController(Deck controlledDeck) : base(controlledDeck)
        {
        }

        public override void SetupDeckActions(Deck deck, Action onfinish)
        {
            ControlledDeck = deck;

            OnFinish = onfinish;

            deck.DrawInitialHand();
        }

        public override void YourTurn()
        {
            Debug.Log("player turn");


            ControlledDeck.Draw(GameSettings.Instance.DrawPrTurn);

            ResetActions();
        }

        public override void UsedAction(Deck deck)
        {
            if (deck != ControlledDeck)
                return;

            ActionsLeft--;

            if (!ActionAvailable() && GameSettings.Instance.AutoEndTurn)
            {
                FinishTurn();
            }
        }

        public void FinishTurn()
        {
            Debug.Log("finish player turn");

            ActionsLeft = 0;

            OnFinish?.Invoke();
        }
    }
}