using System.Collections;
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

            //SETUP listeners
            //    Move->deckmanager(Card, Zone) Handles death/ etb / withdraw / resurrection / draw animation
            // if an action moves a card a zone from different locations, the cards current location is used
            Event.OnChangeLocation.AddListener(AddMoveEvent);

            Event.OnSummon.AddListener(card => AddCardEvent(BattleUI.Summon(card, card.InDeck == BattleUI.Instance.PlayerDeck)));

            Event.OnUnSummon.AddListener(card => AddCardEvent(BattleUI.UnSummon(card.GetGuid())));

            //OnAttack-> (Card) Attack animation
            Event.OnAttack.AddListener(card => AddCardEvent(BattleUI.SetAttacker(card.GetGuid())));
            //On Being Attacked->
            Event.OnBeingAttacked.AddListener(card => AddCardEvent(BattleUI.SetAttackTarget(card.GetGuid())));

            //On Damage-> (Card, amount) & new health?
            //On healed
            Event.OnHealthChange.AddListener((card, val) => AddCardEvent(BattleUI.CardHealthChange(card.GetGuid(), val,card.CurrentHealth,card.MaxHealth)));
            
            //On Stat Change-> (Card, amount)
            Event.OnStatMod.AddListener((card, val) => AddCardEvent(BattleUI.CardStatsModified(card.GetGuid(), val, card.CurrentHealth, card.Attack,card.Damaged())));

            Event.OnBattleFinished.AddListener(d => AddCardEvent(BattleUI.CleanUpUI()));

            //On Ability trigger->All the current Ability animation param
            Event.OnAbilityExecution.AddListener((a,c,ts) => AddCardEvent(BattleUI.AbilityTriggered(a,c.GetGuid(),ts.Select(t=>t.GetGuid()))));

            Event.OnPlayerAction.AddListener(d => AddCardEvent(ActionUsed(d)));
            Event.OnTurnBegin.AddListener(() => AddCardEvent(RefreshActions()));
        }

        private void AddMoveEvent(Card card, Deck.Zone from, Deck.Zone to)
        {
            var playerdeck = BattleUI.Instance.PlayerDeck == card.InDeck;

            AddCardEvent( BattleUI.Move(card.GetGuid(), to, from, playerdeck));
        }

        private IEnumerator RefreshActions()
        {
            yield return null;

            ActionsLeftUI.ActionsRefreshed.Invoke();
        }
        private IEnumerator ActionUsed(Deck deck)
        {
            if (deck != BattleUI.Instance.PlayerDeck)
                yield break;

            ActionsLeftUI.ActionUsed.Invoke();
        }


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