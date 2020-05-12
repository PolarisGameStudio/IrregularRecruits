﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameLogic;
using System;
using Event = GameLogic.Event;
using System.Linq;

namespace UI
{
    //Handles the GameLogic events and creates UI Event. Which the Animations and Sound managers listens to
    //Responsible for the timely and orderly execution of UI. with a wait between each ui event execution, determined by the event type
    public class UIFlowController : Singleton<UIFlowController>
    {
        private Queue<IEnumerator> ActionQueue = new Queue<IEnumerator>();
        private Coroutine ControlRoutine;

        public void Start()
        {
            var bm = BattleManager.Instance;

            ////Debugs -----------------------

            //Event.OnAttack.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Attacking"));
            //Event.OnBeingAttacked.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is Attacked"));
            //Event.OnDamaged.AddListener((c, i) => Debug.Log("Event: " + c.Creature.name + ": Is Damaged for " + i));
            //Event.OnDeath.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is dead"));
            //Event.OnHealed.AddListener((c, i) => Debug.Log("Event: " + c.Creature.name + ": Is healed for " + i));
            //Event.OnKill.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Killed a minion"));
            //Event.OnWithdraw.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Withdrew"));
            //Event.OnAbilityTrigger.AddListener((a, c, ts) => Debug.Log("Event: " + c.Creature.name + ": AbilityTriggered"));
            //Event.OnBattleFinished.AddListener(() => Debug.Log("Event: Combat Finished"));
            //Event.OnCombatResolveFinished.AddListener(() => Debug.Log("Event: Combat Round Finished"));
            //Event.OnCombatSetup.AddListener((p, c) => Debug.Log("Event: Combat started between: " + p + " and " + c));
            //Event.OnGameOver.AddListener(() => Debug.Log("Event: Game Over"));


            //SETUP listeners
            //    Move->deckmanager(Card, Zone) Handles death/ etb / withdraw / resurrection / draw animation
            // if an action moves a card a zone from different locations, the cards current location is used
            Event.OnChangeLocation.AddListener(AddMoveEvent);
           // Event.OnDeath.AddListener(card=> AddMoveEvent(card,Deck.Zone.Graveyard,card.Location));
           //Event.OnEtb.AddListener(card=> AddMoveEvent(card, Deck.Zone.Battlefield, card.Location));
           //Event.OnWithdraw.AddListener(card=> AddMoveEvent(card, Deck.Zone.Library, Deck.Zone.Battlefield));
           //Event.OnRessurrect.AddListener(card=> AddMoveEvent(card, Deck.Zone.Battlefield, Deck.Zone.Graveyard));
           //Event.OnDraw.AddListener(card=> AddMoveEvent(card, Deck.Zone.Hand, Deck.Zone.Library));

            //OnAttack-> (Card) Attack animation
            Event.OnAttack.AddListener(card => AddCardEvent(BattleUI.SetAttacker(card.Guid)));
            //On Being Attacked->
            Event.OnBeingAttacked.AddListener(card => AddCardEvent(BattleUI.SetAttackTarget(card.Guid)));

            //On Damage-> (Card, amount) & new health?
            //TODO: maybe the damaged state used is not the correct one
            Event.OnHealthLoss.AddListener((card, val) => AddCardEvent(BattleUI.CardHealthChange(card.Guid, -val,card.CurrentHealth,card.MaxHealth)));
            //On healed
            Event.OnHealed.AddListener((card, val) => AddCardEvent(BattleUI.CardHealthChange(card.Guid, val, card.CurrentHealth, card.MaxHealth)));

            //On Stat Change-> (Card, amount)
            Event.OnStatMod.AddListener((card, val) => AddCardEvent(BattleUI.CardStatsModified(card.Guid, val, card.CurrentHealth, card.Attack,card.Damaged())));

            Event.OnBattleFinished.AddListener(() => AddCardEvent(BattleUI.CleanUpUI()));

            //On Ability trigger->All the current Ability animation param
            Event.OnAbilityTrigger.AddListener((a,c,ts) => AddCardEvent(BattleUI.AbilityTriggered(a,c.Guid,ts.Select(t=>t.Guid))));

        }

        private void AddMoveEvent(Card card, Deck.Zone from, Deck.Zone to)
        {
            var playerdeck = card.InDeck.PlayerDeck;

            AddCardEvent( BattleUI.Move(card.Guid, to, from, playerdeck));
        }


        //TODO: replace with coroutines
        private void AddCardEvent(IEnumerator uIEvent)
        {
            ActionQueue.Enqueue(uIEvent);

            if (ControlRoutine == null)
                ControlRoutine = StartCoroutine(UIRoutine());
        }

        private IEnumerator UIRoutine()
        {
            while(!EmptyQueue())
            {
                yield return ActionQueue.Dequeue();
            }

            ControlRoutine = null;
        }

        public bool EmptyQueue() => ActionQueue.Count == 0;

    }

}