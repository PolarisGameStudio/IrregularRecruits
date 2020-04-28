using System.Collections.Generic;
using UnityEngine.Events;

namespace GameLogic
{
    public class Event
    {
        //CARD EVENTS
        public class CardEvent : UnityEvent<Card> { }
        public static CardEvent OnDraw = new CardEvent();
        public static CardEvent OnPlay = new CardEvent();
        public static CardEvent OnDeath = new CardEvent();
        public static CardEvent OnRessurrect = new CardEvent();
        public static CardEvent OnKill = new CardEvent();
        public static CardEvent OnAttack = new CardEvent();
        public static CardEvent OnBeingAttacked = new CardEvent();
        public static CardEvent OnDamaged = new CardEvent();
        public static CardEvent OnHealed = new CardEvent();
        public static CardEvent OnWithdraw = new CardEvent();

        //Ability,Owner,Targets . TODO: use holder class
        public class AbilityEvent : UnityEvent<Ability, Card, List<Card>> { }
        public static AbilityEvent OnAbilityTrigger = new AbilityEvent();

        //COMBAT EVENTS
        public class CombatEvent : UnityEvent<Deck, Deck> { }
        //Inputs player deck and enemy deck, in that order
        public static CombatEvent OnCombatSetup = new CombatEvent();
        public static UnityEvent OnBattleFinished = new UnityEvent();

        //TODO: replace with deck/card action and parse the relevant Controller/deck/card
        public static UnityEvent OnPlayerAction = new UnityEvent();

        //TODO: should be handled by combat Resolver ;
        public static UnityEvent OnCombatResolveStart = new UnityEvent();
        public static UnityEvent OnCombatResolveFinished = new UnityEvent();

        public static UnityEvent OnTurnBegin = new UnityEvent();

        //GAME EVENTS
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGameOpen = new UnityEvent();
        public static UnityEvent OnGameBegin = new UnityEvent();

        //public static void ResetListeners()
        //{
        //    OnDraw.RemoveAllListeners();
        //    OnPlay.RemoveAllListeners();
        //    OnDeath.RemoveAllListeners();
        //    OnRessurrect.RemoveAllListeners();
        //    OnKill.RemoveAllListeners();
        //    OnAttack.RemoveAllListeners();
        //    OnBeingAttacked.RemoveAllListeners();
        //    OnDamaged.RemoveAllListeners();
        //    OnHealed.RemoveAllListeners();
        //    OnWithdraw.RemoveAllListeners();

        //    OnAbilityTrigger.RemoveAllListeners();

        //    OnBattleFinished.RemoveAllListeners();
        //    OnPlayerAction.RemoveAllListeners();
        //    OnCombatSetup.RemoveAllListeners();
        //    OnCombatResolveFinished.RemoveAllListeners();
        //    OnTurnBegin.RemoveAllListeners();

        //    OnGameBegin.RemoveAllListeners();
        //    OnGameOpen.RemoveAllListeners();
        //    OnGameOver.RemoveAllListeners();
        //}
    }
}