using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Event = GameLogic.Event;

namespace UI
{

    public partial class ActionsLeftUI : MonoBehaviour
    {

        public ActionIcon ActionIconExample;
        private List<ActionIcon> ActionIcons = new List<ActionIcon>();
        private bool Initialized;

        public static Event.IntEvent ActionGained = new Event.IntEvent();

        private void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (Initialized) return;

            Initialized = true;

            ActionGained.RemoveAllListeners();
            ActionGained.AddListener(ActionsChanged);
            //ActionsRefreshed.AddListener(RefreshActions);

            //TODO: does not take into account if amount of actions are changed. Move to on next turn and check there
            for (int i = 0; i < GameSettings.Instance.PlaysPrTurn; i++)
            {
                CreateActionIcon();
            }
            ActionIconExample.gameObject.SetActive(false);

        }

        private void ActionsChanged(int arg0)
        {
            while(ActionIcons.Count(a => a.Active) != arg0)
            {
                if (ActionIcons.Count(a => a.Active) > arg0)
                    UseAction();
                else
                    GainAction();
            }
        
        }

        private void CreateActionIcon()
        {
            ActionIcon item = Instantiate(ActionIconExample, ActionIconExample.transform.parent);
            item.gameObject.SetActive(true);
            
            ActionIcons.Add(item);
        }

        private void UseAction()
        {
            if (ActionIcons.Any(a => a.Active))
                ActionIcons.First(a => a.Active).Active = false;

        }
        private void GainAction()
        {
            if (!ActionIcons.Any(a => !a.Active))
                CreateActionIcon();
            
            ActionIcons.Last(a => !a.Active).Active = true;

        }

        //private void RefreshActions()
        //{
        //    ActionIcons.ForEach(a => a.Active = true);
        //}

    }
}