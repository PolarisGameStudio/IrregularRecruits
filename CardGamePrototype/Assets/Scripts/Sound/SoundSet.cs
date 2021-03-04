using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public struct SetSound
    {
        public CreatureBark SoundType;
        public AudioClip[] Sounds;
    }


    [System.Serializable]
    public struct SoundSet
    {
        public SoundSetType Type;
        public List<SetSound> SoundSetSounds;

        internal AudioClip[] GetSoundType(CreatureBark soundType)
        {
            if (!SoundSetSounds.Any(s => s.SoundType == soundType))
                return null;

            return SoundSetSounds.First(s => s.SoundType == soundType).Sounds;

        }
    }

}