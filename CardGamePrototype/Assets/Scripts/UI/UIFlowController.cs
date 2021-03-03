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
        public UnityEvent OnEmptyQueue = new UnityEvent();

        public void Start()
        {
            //todo: do we need this
            //var bm = BattleManager.Instance;


            //SETUP listeners
            //    Move->deckmanager(Card, Zone) Handles death/ etb / withdraw / resurrection / draw animation
            // if an action moves a card a zone from different locations, the cards current location is used
            Event.OnChangeLocation.AddListener(AddMoveEvent);

            Event.OnSummon.AddListener(card => AddCardEvent(BattleUI.Summon(card, card.InDeck == BattleUI.Instance.PlayerDeck)));

            Event.OnUnSummon.AddListener(card => AddCardEvent(BattleUI.UnSummon(card.Guid)));

            //OnAttack-> (Card) Attack animation
            Event.OnAttack.AddListener(card => AddCardEvent(BattleUI.SetAttacker(card.Guid)));
            //On Being Attacked->
            Event.OnBeingAttacked.AddListener(card => AddCardEvent(BattleUI.SetAttackTarget(card.Guid)));

            //On Damage-> (Card, amount) & new health?
            //On healed
            Event.OnHealthChange.AddListener((card, val) => AddCardEvent(BattleUI.CardHealthChange(card.Guid, val, card.CurrentHealth, card.MaxHealth)));

            Event.OnActionPointsChange.AddListener(i => AddCardEvent(ActionsChanged(i)));

            //On Stat Change-> (Card, amount)
            Event.OnStatMod.AddListener((card, val) => AddCardEvent(BattleUI.CardStatsModified(card.Guid, val, card.CurrentHealth, card.Attack, card.Damaged())));

            Event.OnBattleFinished.AddListener(d => AddCardEvent(BattleUI.CleanUpUI(d)));

            //On Ability trigger->All the current Ability animation param
            Event.OnAbilityExecution.AddListener((a, c, ts) => AddCardEvent(BattleUI.AbilityTriggered(a, c.Guid, ts.Select(t => t.Guid))));

            //TODO: is this actually used in a timely order
            Event.OnCombatSetup.AddListener((e, c) => PlayerTurnStart());

            Event.OnTurnBegin.AddListener(() => AddCardEvent(PlayerTurnStart()));
            Event.OnCombatResolveStart.AddListener(PlayerTurnDone);
        }

        private void AddMoveEvent(Card card, Deck.Zone from, Deck.Zone to)
        {
            var playerdeck = BattleUI.Instance.PlayerDeck == card.InDeck;

            AddCardEvent(BattleUI.Move(card.Guid, to, from, playerdeck));
        }

        private IEnumerator ActionsChanged(int amount)
        {
            ActionsLeftUI.ActionGained.Invoke(amount);


            yield return new WaitForSeconds(0.1f);

        }

        private IEnumerator PlayerTurnStart()
        {
            yield return null;

            if (BattleManager.Instance.PlayerDeck.DeckController is PlayerController)
            {
                BattleUI.Instance.UILocked = false;

                HeroUI.Instance?.UnlockAbilities();
            }
            else
                BattleUI.Instance.UILocked = true;

        }

        private void PlayerTurnDone()
        {
            Debug.Log("Combat started. Locking ui");

            BattleUI.Instance.UILocked = true;
            HeroUI.Instance?.LockAbilities();

        }



        private void AddCardEvent(IEnumerator uIEvent)
        {
            ActionQueue.Enqueue(uIEvent);

            if (ControlRoutine == null)
                ControlRoutine = StartCoroutine(UIRoutine());
        }

        private IEnumerator UIRoutine()
        {
            while (!EmptyQueue())
            {
                yield return ActionQueue.Dequeue();
            }

            ControlRoutine = null;
            OnEmptyQueue.Invoke();
        }

        public bool EmptyQueue() => ActionQueue.Count == 0;

    }

}