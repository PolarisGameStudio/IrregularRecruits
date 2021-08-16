using System.Collections.Generic;
using UnityEngine.Events;

namespace GameLogic
{

    public class Event
    {

        //CARD EVENTS
        //Any move event happens after the event is called.
        //Takes the location zone, so that triggers can still happen, even if location has changed after
        public class CardEvent : UnityEvent<Card,Deck.Zone> { }
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
        public static CardEvent OnWardTriggered = new CardEvent();

        public static CardEvent OnDamaged = new CardEvent();

        public class IntEvent : UnityEvent<int> { }
        public static IntEvent OnPlayerGoldAdd = new IntEvent();
        public static IntEvent PlayerActionPointsChanged = new IntEvent();

        public class CardValueEvent : UnityEvent<Card,int,Deck.Zone> { }
        public static CardValueEvent OnHealthChange = new CardValueEvent();
        public static CardValueEvent OnHealed = new CardValueEvent();
        public static CardValueEvent OnStatMod = new CardValueEvent();



        //Ability,Owner,Targets 
        public class AbilityEvent : UnityEvent<SpecialAbility, AbilityHolder, List<Card>> { }
        public static AbilityEvent OnAbilityExecution = new AbilityEvent();

        //takes the trigger type, the ability owner and the result action to allow doublers to double the effect
        public class AbilityTriggerEvent : UnityEvent<TriggerType, EffectType, AbilityHolder, UnityAction> { }
        public static AbilityTriggerEvent OnAbilityTriggered = new AbilityTriggerEvent();


        //COMBAT EVENTS
        public class CombatEvent : UnityEvent<Deck, Deck> { }

        //Inputs winner deck and loser deck, in that order
        public static CombatEvent OnBattleFinished = new CombatEvent();

        public class AddUnitEvent : UnityEvent<Creature[]> { }
        public static AddUnitEvent OnHireUnits = new AddUnitEvent();


        //TODO: replace with deck/card action and parse the relevant Controller/deck/card
        public class DeckEvent : UnityEvent<Deck> { }

        public static DeckEvent OnPlayerAction = new DeckEvent();
        public static DeckEvent OnDeckSizeChange = new DeckEvent();

        public class UnlockEvent : UnityEvent<UnlockCondition> { }
        public static UnlockEvent OnAchievement = new UnlockEvent();

        //Inputs player deck and enemy deck, in that order
        public static UnityEvent OnCombatStart = new UnityEvent();

        //TODO: should be handled by combat Resolver ;
        public static UnityEvent OnCombatResolveStart = new UnityEvent();
        public static UnityEvent OnCombatResolveFinished = new UnityEvent();

        public static UnityEvent OnTurnBegin = new UnityEvent();

        //GAME EVENTS
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGameOpen = new UnityEvent();
        public static UnityEvent OnGameWin = new UnityEvent();
        public static UnityEvent OnGameBegin = new UnityEvent();
        public static UnityEvent OnStatScreen = new UnityEvent();

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
            OnWardTriggered.RemoveAllListeners();

            //public static CardEvent OnDamaged = new CardEvent();
            OnDamaged.RemoveAllListeners();

            //public static CardValueEvent OnHealthChange = new CardValueEvent();
            OnHealthChange.RemoveAllListeners();

            //public static CardValueEvent OnHealed = new CardValueEvent();
            OnHealed.RemoveAllListeners();
            //public static CardValueEvent OnStatMod = new CardValueEvent();
            OnStatMod.RemoveAllListeners();

            //public static AbilityEvent OnAbilityExecution = new AbilityEvent();
            OnAbilityExecution = new AbilityEvent();

            OnAbilityTriggered = new AbilityTriggerEvent();

            //public static CombatEvent OnCombatSetup = new CombatEvent();
            OnCombatStart.RemoveAllListeners();
            //public static DeckEvent OnBattleFinished = new DeckEvent();
            OnBattleFinished.RemoveAllListeners();

            //public static DeckEvent OnPlayerAction = new DeckEvent();
            OnPlayerAction.RemoveAllListeners();

            OnDeckSizeChange.RemoveAllListeners();

            OnAchievement.RemoveAllListeners();

            //public static UnityEvent OnCombatResolveStart = new UnityEvent();
            OnCombatResolveStart.RemoveAllListeners();
            //public static UnityEvent OnCombatResolveFinished = new UnityEvent();
            OnCombatResolveFinished.RemoveAllListeners();


            OnTurnBegin.RemoveAllListeners();
            //public static UnityEvent OnGameOver = new UnityEvent();
            OnGameOver.RemoveAllListeners();
            OnGameWin.RemoveAllListeners();
            //public static UnityEvent OnGameOpen = new UnityEvent();
            OnGameOpen.RemoveAllListeners();
            //public static UnityEvent OnGameBegin = new UnityEvent();
            OnGameBegin.RemoveAllListeners();

            OnStatScreen.RemoveAllListeners();

            //public static LocationChangeEvent OnChangeLocation = new LocationChangeEvent();
            OnChangeLocation.RemoveAllListeners();

            //public static HeroEvent OnLevelUpSelection = new HeroEvent();
            OnLevelUpSelection.RemoveAllListeners();
            //public static HeroEvent OnLevelUp = new HeroEvent();
            OnLevelUp.RemoveAllListeners();

            OnPlayerGoldAdd.RemoveAllListeners();

            PlayerActionPointsChanged.RemoveAllListeners();
        }
    }
}