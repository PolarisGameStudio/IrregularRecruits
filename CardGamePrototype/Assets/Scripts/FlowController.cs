using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Responsible for controlling the timed invoke of events, by a readyforaction boolean and adding the event to an action queue.
//TODO: replace event fields with an enum a create event in a dictionary, to better control 
public class FlowController : Singleton<FlowController>
{
    private static Queue<Action> ActionQueue = new Queue<Action>();
    public static bool ReadyForNextAction;

    public FlowSpeedFactor[] FlowSpeedFactors;

    [Serializable]
    public struct FlowSpeedFactor
    {
        public FlowEvent FlowEvent;
        public float SpeedFactor;
    }

    //Replace these with more descriptive Events
    public enum FlowEvent
    {
        Standard,
        Fast,
    }

    void Start()
    {
        //Debugs texts
        Event.OnAttack.AddListener(c => Debug.Log("Event: " + c + ": Attacking"));
        Event.OnBeingAttacked.AddListener(c => Debug.Log("Event: " + c + ": Is Attacked"));
        Event.OnDamaged.AddListener(c => Debug.Log("Event: " + c + ": Is Damaged"));
        Event.OnDeath.AddListener(c => Debug.Log("Event: " + c + ": Is dead"));
        Event.OnHealed.AddListener(c => Debug.Log("Event: " + c + ": Is healed"));
        Event.OnKill.AddListener(c => Debug.Log("Event: " + c + ": Killed a minion"));
        Event.OnWithdraw.AddListener(c => Debug.Log("Event: " + c + ": Withdrew"));
        Event.OnAbilityTrigger.AddListener((a,c,ts) => Debug.Log("Event: " + c + ": AbilityTriggered"));
        Event.OnCombatFinished.AddListener(() => Debug.Log("Event: Combat Finished"));
        Event.OnCombatRoundFinished.AddListener(() => Debug.Log("Event: Combat Round Finished"));
        Event.OnCombatStart.AddListener(() => Debug.Log("Event: Combat started"));
        Event.OnGameOver.AddListener(() => Debug.Log("Event: Game Over"));

        StartCoroutine(EventControlLoop());
    }

    //Every COMBATSPEED seconds it will trigger the next action in the action queue
    //If no actions to invoke, it will set ReadyForAction = true;
    private IEnumerator EventControlLoop()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();

            if (ActionQueue.Count > 0)
            {
                ActionQueue.Dequeue().Invoke();
                yield return new WaitForSeconds(GameSettings.Instance.CombatSpeed);
            }
            else
                ReadyForNextAction = true;
        }
    }

    //TODO: maybe use a enum or class for different event types
    internal static void AddEvent(Action p)
    {
        ReadyForNextAction = false;

        ActionQueue.Enqueue(p);
    }
}
