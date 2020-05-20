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

        public static UnityEvent ActionUsed = new UnityEvent() ;
        public static UnityEvent ActionsRefreshed = new UnityEvent();

        private void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (Initialized) return;

            Initialized = true;

            ActionUsed.RemoveAllListeners();
            ActionsRefreshed.RemoveAllListeners();
            ActionUsed.AddListener(OnActionUsed);
            ActionsRefreshed.AddListener(RefreshActions);

            //TODO: does not take into account if amount of actions are changed. Move to on next turn and check there
            for (int i = 0; i < GameSettings.Instance.PlaysPrTurn; i++)
            {
                ActionIcons.Add(Instantiate(ActionIconExample, ActionIconExample.transform.parent));
            }
            Destroy(ActionIconExample.gameObject);

        }

        private void OnActionUsed()
        {
            if (ActionIcons.Any(a => a.Active))
                ActionIcons.First(a => a.Active).Active = false;
        }

        private void RefreshActions()
        {
            ActionIcons.ForEach(a => a.Active = true);
        }

    }
}