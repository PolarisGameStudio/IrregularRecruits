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

        //TODO: replace with deck/card action and parse the relevant Controller/deck/card
        public class DeckEvent : UnityEvent<Deck> { }

        public static DeckEvent OnPlayerAction = new DeckEvent();

        //TODO: should be handled by combat Resolver ;
        public static UnityEvent OnCombatResolveStart = new UnityEvent();
        public static UnityEvent OnCombatResolveFinished = new UnityEvent();

        public static UnityEvent OnTurnBegin = new UnityEvent();

        //GAME EVENTS
        public static UnityEvent OnGameOver = new UnityEvent();
        public static UnityEvent OnGameOpen = new UnityEvent();
        public static UnityEvent OnGameBegin = new UnityEvent();

        //moved Card. From. To
        //should only be used by ui to determine card locations
        public class LocationChangeEvent : UnityEvent<Card, Deck.Zone, Deck.Zone> { }
        public static LocationChangeEvent OnChangeLocation = new LocationChangeEvent();


        public class HeroEvent : UnityEvent<Hero> { }

        public static HeroEvent OnLevelUpSelection = new HeroEvent();
        public static HeroEvent OnLevelUp = new HeroEvent();
    }
}