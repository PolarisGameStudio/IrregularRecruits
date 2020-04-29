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
    public class UIController : Singleton<UIController>
    {
        public Queue<ControlledUIEvent> ActionQueue = new Queue<ControlledUIEvent>();
        public bool ReadyForInput { get; private set; }
        public UnityEvent OnReadyForInput = new UnityEvent();

        public class UIEvent : UnityEvent<CardUI> { }

        public UIController()
        {
            //SETUP listeners
            //Event.OnDamaged.AddListener(c => AddCardEvent(c, 1f, );
        }

        private void AddCardEvent(Card c,float timeToExecute,UIEvent uIEvent)
        {
            ActionQueue.Enqueue(new ControlledUIEvent());
        }

        //TODO: remove this from Game Logic
        //Event.OnDeath.AddListener(c => c.BattleRepresentation?.CardAnimation.Dissolve());
        //        Event.OnRessurrect.AddListener(c => c.BattleRepresentation?.CardAnimation.UnDissolve());


        //BattleRepresentation?.Flip();
    }

    public struct ControlledUIEvent
    {
        public float TimeToExecute;
        public UIController.UIEvent InvokesEvent;
        public CardUI CardUI;
        public CardUI TargetCardUI;

        public ControlledUIEvent(float timeToExecute, UIController.UIEvent invokesEvent, CardUI cardUI, CardUI targetCardUI)
        {
            TimeToExecute = timeToExecute;
            InvokesEvent = invokesEvent;
            CardUI = cardUI;
            TargetCardUI = targetCardUI;
        }
    } 
}