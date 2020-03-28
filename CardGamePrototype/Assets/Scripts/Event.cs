using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event : MonoBehaviour
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
    public static CardEvent OnAbilityTrigger = new CardEvent();

    //COMBAT EVENTS
    public static UnityEvent OnCombatFinished = new UnityEvent();
    public static UnityEvent OnCombatStart = new UnityEvent();
    public static UnityEvent OnCombatRoundFinished = new UnityEvent();

    //GAME EVENTS
    public static UnityEvent OnGameOver = new UnityEvent();


}
