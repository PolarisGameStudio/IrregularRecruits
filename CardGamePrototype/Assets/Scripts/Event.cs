using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event 
{
    //CARD EVENTS
    public class CardEvent : UnityEvent<Card> { }
    public static CardEvent OnDraw = new CardEvent();
    public static CardEvent OnPlay = new CardEvent();
    public static CardEvent OnDeath = new CardEvent();
    public static CardEvent OnKill = new CardEvent();
    public static CardEvent OnAttack = new CardEvent();
    public static CardEvent OnBeingAttacked = new CardEvent();
    public static CardEvent OnDamaged = new CardEvent();
    public static CardEvent OnHealed = new CardEvent();
    public static CardEvent OnWithdraw = new CardEvent();

    //Ability,Owner,Targets . TODO: use holder class
    public class AbilityEvent : UnityEvent<Ability,Card,List<Card>> { }
    public static AbilityEvent OnAbilityTrigger = new AbilityEvent();

    //COMBAT EVENTS
    public static UnityEvent OnCombatFinished = new UnityEvent();
    public static UnityEvent OnPlayerAction = new UnityEvent();
    public static UnityEvent OnCombatStart = new UnityEvent();
    public static UnityEvent OnCombatRoundFinished = new UnityEvent();
    public static UnityEvent OnTurnBegin = new UnityEvent();

    //GAME EVENTS
    public static UnityEvent OnGameOver = new UnityEvent();



}
