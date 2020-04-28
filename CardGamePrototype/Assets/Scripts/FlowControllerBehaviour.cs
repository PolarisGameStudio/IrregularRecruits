using System.Collections;
using UnityEngine;

//Responsible for controlling the timed invoke of events, by a readyforaction boolean and adding the event to an action queue.
//TODO: replace event fields with an enum a create event in a dictionary, to better control 
public class FlowControllerBehaviour : Singleton<FlowControllerBehaviour>
{
    void Start()
    {
        //Debugs texts
        Event.OnAttack.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Attacking"));
        Event.OnBeingAttacked.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is Attacked"));
        Event.OnDamaged.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is Damaged"));
        Event.OnDeath.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is dead"));
        Event.OnHealed.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Is healed"));
        Event.OnKill.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Killed a minion"));
        Event.OnWithdraw.AddListener(c => Debug.Log("Event: " + c.Creature.name + ": Withdrew"));
        Event.OnAbilityTrigger.AddListener((a, c, ts) => Debug.Log("Event: " + c.Creature.name + ": AbilityTriggered"));
        Event.OnBattleFinished.AddListener(() => Debug.Log("Event: Combat Finished"));
        Event.OnCombatResolveFinished.AddListener(() => Debug.Log("Event: Combat Round Finished"));
        Event.OnCombatSetup.AddListener((p, c) => Debug.Log("Event: Combat started between: " + p + " and " + c));
        Event.OnGameOver.AddListener(() => Debug.Log("Event: Game Over"));

        StartCoroutine(EventControlLoop());
    }

    //Every COMBATSPEED seconds it will trigger the next action in the action queue
    //If no actions to invoke, it will set ReadyForAction = true;
    private IEnumerator EventControlLoop()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (FlowController.ActionQueue.Count > 0)
            {
                FlowController.TriggerNextAction();
                yield return new WaitForSeconds(GameSettings.Instance.CombatSpeed);
            }
        }
    }

}
