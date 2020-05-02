using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameLogic;
using System;
using Event = GameLogic.Event;

namespace UI
{
    //Handles the GameLogic events and creates UI Event. Which the Animations and Sound managers listens to
    //Responsible for the timely and orderly execution of UI. with a wait between each ui event execution, determined by the event type
    public class UIFlowController : Singleton<UIFlowController>
    {
        public Queue<ControlledUIEvent> ActionQueue = new Queue<ControlledUIEvent>();
        public Coroutine ControlRoutine;

        public void Start()
        {
            var bm = BattleManager.Instance;

            //SETUP listeners
            //    Move->deckmanager(Card, Zone) Handles death/ etb / withdraw / resurrection / draw animation
            // if an action moves a card a zone from different locations, the cards current location is used
           Event.OnDeath.AddListener(card=> AddCardEvent( ()=> BattleUI.Move(card,Deck.Zone.Graveyard,card.Location)));
           Event.OnPlay.AddListener(card=> AddCardEvent( ()=> BattleUI.Move(card,Deck.Zone.Battlefield,card.Location)));
           Event.OnWithdraw.AddListener(card=> AddCardEvent( ()=> BattleUI.Move(card,Deck.Zone.Library,Deck.Zone.Battlefield)));
           Event.OnRessurrect.AddListener(card=> AddCardEvent( ()=> BattleUI.Move(card,Deck.Zone.Battlefield,Deck.Zone.Graveyard)));
           Event.OnDraw.AddListener(card=> AddCardEvent( ()=> BattleUI.Move(card,Deck.Zone.Hand,Deck.Zone.Library ),0.2f));

            //OnAttack-> (Card) Attack animation
            Event.OnAttack.AddListener(card => AddCardEvent(() => BattleUI.SetAttacker(card),0.5f));
            //On Being Attacked->
            Event.OnBeingAttacked.AddListener(card => AddCardEvent(() => BattleUI.SetAttackTarget(card),0.5f));

            //On Damage-> (Card, amount) & new health?
            Event.OnDamaged.AddListener((card, val) => AddCardEvent(() => BattleUI.CardHealthChange(card, -val), 0.1f));
            //On healed
            Event.OnHealed.AddListener((card, val) => AddCardEvent(() => BattleUI.CardHealthChange(card, val), 0.1f));

            //On Stat Change-> (Card, amount)
            Event.OnStatMod.AddListener((card, val) => AddCardEvent(() => BattleUI.CardStatsModified(card, val), 0.5f));

            //On Ability trigger->All the current Ability animation param

        }

        private void AddCardEvent (Action action, float timeToExecute = 1f)
        {
            ActionQueue.Enqueue(new ControlledUIEvent(timeToExecute,action));

            if (ControlRoutine == null)
                ControlRoutine = StartCoroutine(UIRoutine());
        }

        private IEnumerator UIRoutine()
        {
            while(ActionQueue.Count > 0)
            {
                var nextEvent = ActionQueue.Dequeue();

                nextEvent.UIAction.Invoke();
                yield return new WaitForSeconds(nextEvent.TimeToExecute);
            }

            ControlRoutine = null;
        }

    }

    public struct ControlledUIEvent
    {
        public float TimeToExecute;
        public Action UIAction;

        public ControlledUIEvent(float timeToExecute, Action action)
        {
            TimeToExecute = timeToExecute;
            UIAction = action;
        }
    } 
}