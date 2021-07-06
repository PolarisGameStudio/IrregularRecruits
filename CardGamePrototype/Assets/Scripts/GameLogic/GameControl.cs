using UnityEngine;

namespace GameLogic
{
    //should be called first of everything to make sure everything is reset and initialised correctly
    public class GameControl : MonoBehaviour
    {
        private void Awake()
        {
            Event.ResetEvents();
            LegacySystem.Instance.Load();
        }
    }
}