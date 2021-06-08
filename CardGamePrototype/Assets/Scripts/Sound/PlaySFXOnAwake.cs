using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class PlaySFXOnAwake : MonoBehaviour
    {
        public SoundLibrary.UiSound Sound;

        void OnEnable()
        {
            SoundController.PlayUISound(Sound);
        }
    }
}