using System.Collections.Generic;
using UnityEngine.Events;

namespace GameLogic
{
    public class Event
    {

        //CARD EVENTS
        //Any move event happens after the event is called
        public class CardEvent : UnityEvent<Card> { }
        public static CardEvent OnDraw = new CardEvent();
        public static CardEvent OnEtb = new CardEvent();
        public static CardEvent OnDeath = new CardEvent();
        public static CardEvent OnRessurrect = new CardEvent();
        public static CardEvent OnKill = new CardEvent();
        public static CardEvent OnAttack = new CardEvent();
        public static CardEvent OnBeingAttacked = new CardEvent();
        public static CardEvent OnWithdraw = new CardEvent();
        public static CardEvent OnSummon = new CardEvent();
        public static CardEvent OnUnSummon = new CardEvent();

        public static CardEvent OnDamaged = new CardEvent();

        public class IntEvent : UnityEvent<int> { }
        public static IntEvent OnPlayerGoldAdd = new IntEvent();
        public static IntEvent OnActionGained = new IntEvent();

        public class CardValueEvent : UnityEvent<Card,int> { }
        public static CardValueEvent OnHealthChange = new CardValueEvent();
        public static CardValueEvent OnHealed = new CardValueEvent();
        public static CardValueEvent OnStatMod = new CardValueEvent();

        //Ability,Owner,Targets . TODO: use holder class
        public class AbilityEvent : UnityEvent<Ability, AbilityHolder, List<Card>> { }
        public static AbilityEvent OnAbilityExecution = new AbilityEvent();

        //COMBAT EVENTS
        public class CombatEvent : UnityEvent<Deck, Deck> { }
        //Inputs player deck and enemy deck, in that order
        public static CombatEvent OnCombatSetup = new CombatEvent();
        public static DeckEvent OnBattleFinished = new DeckEvent();

        public class AddMinionEvent : UnityEvent<Creature[]> { }
        public static AddMinionEvent OnHireMinions = new AddMinionEvent();


        //TODO: replace with deck/card action and parse the relevant Controller/deck/card
        public class DeckEvent : UnityEvent<Deck> { }

        public static DeckEvent OnPlayerAction = new DeckEvent();
        public static DeckEvent OnDeckSizeChange = new DeckEvent();

        //TODO: should be handled by combat Resolver ;
        public static UnityEvent OnCombatResolveStart = new UnityEvent();
        public static UnityEvent OnCombatResolveFinished = new UnityEvent();

        public static UnityEvent OnTurnBegin = new UnityEvent();

        //GAME EVENTS
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGameOpen = new UnityEvent();
        public static UnityEvent OnGameWin = new UnityEvent();
        public static UnityEvent OnGameBegin = new UnityEvent();

        //moved Card. From. To
        //should only be used by ui to determine card locations
        public class LocationChangeEvent : UnityEvent<Card, Deck.Zone, Deck.Zone> { }
        public static LocationChangeEvent OnChangeLocation = new LocationChangeEvent();



        //Hero events
        public class HeroEvent : UnityEvent<Hero> { }

        public static HeroEvent OnHeroSelect = new HeroEvent();
        public static HeroEvent OnLevelUpSelection = new HeroEvent();
        public static HeroEvent OnLevelUp = new HeroEvent();

        //TODO: remove this and instead use a event manager with a lifetime. and remove card listeners correctly, when they are removed
        public static void ResetEvents()
        {
            //public static CardEvent OnDraw = new CardEvent();
            OnDraw.RemoveAllListeners();
            //public static CardEvent OnEtb = new CardEvent();
            OnEtb.RemoveAllListeners();
            //public static CardEvent OnDeath = new CardEvent();
            OnDeath.RemoveAllListeners();
            //public static CardEvent OnRessurrect = new CardEvent();
            OnRessurrect.RemoveAllListeners();
            //public static CardEvent OnKill = new CardEvent();
            OnKill.RemoveAllListeners();
            //public static CardEvent OnAttack = new CardEvent();
            OnAttack.RemoveAllListeners();
            //public static CardEvent OnBeingAttacked = new CardEvent();
            OnBeingAttacked.RemoveAllListeners();
            //public static CardEvent OnWithdraw = new CardEvent();
            OnWithdraw.RemoveAllListeners();
            //public static CardEvent OnSummon = new CardEvent();
            OnSummon.RemoveAllListeners();
            //public static CardEvent OnUnSummon = new CardEvent();
            OnUnSummon.RemoveAllListeners();

            //public static CardEvent OnDamaged = new CardEvent();
            OnDamaged.RemoveAllListeners();

            //public static CardValueEvent OnHealthChange = new CardValueEvent();
            OnHealthChange.RemoveAllListeners();
            //public static CardValueEvent OnHealed = new CardValueEvent();
            OnHealed.RemoveAllListeners();
            //public static CardValueEvent OnStatMod = new CardValueEvent();
            OnStatMod.RemoveAllListeners();

            //public static AbilityEvent OnAbilityExecution = new AbilityEvent();
            OnAbilityExecution.RemoveAllListeners();

            //public static CombatEvent OnCombatSetup = new CombatEvent();
            OnCombatSetup.RemoveAllListeners();
            //public static DeckEvent OnBattleFinished = new DeckEvent();
            OnBattleFinished.RemoveAllListeners();

            //public static DeckEvent OnPlayerAction = new DeckEvent();
            OnPlayerAction.RemoveAllListeners();

            //public static UnityEvent OnCombatResolveStart = new UnityEvent();
            OnCombatResolveStart.RemoveAllListeners();
            //public static UnityEvent OnCombatResolveFinished = new UnityEvent();
            OnCombatResolveFinished.RemoveAllListeners();

            //public static UnityEvent OnGameOver = new UnityEvent();
            OnGameOver.RemoveAllListeners();
            OnGameWin.RemoveAllListeners();
            //public static UnityEvent OnGameOpen = new UnityEvent();
            OnGameOpen.RemoveAllListeners();
            //public static UnityEvent OnGameBegin = new UnityEvent();
            OnGameBegin.RemoveAllListeners();

            //public static LocationChangeEvent OnChangeLocation = new LocationChangeEvent();
            OnChangeLocation.RemoveAllListeners();

            //public static HeroEvent OnLevelUpSelection = new HeroEvent();
            OnLevelUpSelection.RemoveAllListeners();
            //public static HeroEvent OnLevelUp = new HeroEvent();
            OnLevelUp.RemoveAllListeners();

            OnPlayerGoldAdd.RemoveAllListeners();

            OnActionGained.RemoveAllListeners();
        }
    }
}