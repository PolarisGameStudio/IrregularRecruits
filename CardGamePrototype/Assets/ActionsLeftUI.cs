using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionsLeftUI : MonoBehaviour
{
    public ActionIcon ActionIconExample;
    private List<ActionIcon> ActionIcons = new List<ActionIcon>();

    void Awake()
    {
        //TODO: does not take into account if amount of actions are changed. Move to on next turn and check there
        for(int i = 0; i < GameSettings.Instance.PlayerPlaysPrTurn;i++)
        {
            ActionIcons.Add(Instantiate(ActionIconExample, ActionIconExample.transform.parent));
        }
        Destroy(ActionIconExample.gameObject);

        Event.OnPlayerAction.AddListener(OnActionUsed);
        Event.OnTurnBegin.AddListener(OnNextTurn);
    }

    private void OnActionUsed()
    {
        if(ActionIcons.Any(a=> a.Active))
            ActionIcons.First(a => a.Active).Active = false;
    }

    private void OnNextTurn()
    {
        ActionIcons.ForEach(a => a.Active = true);
    }

}