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
        public CreatureSound SoundType;
        public AudioClip[] Sounds;
    }

    public enum CreatureSound
    {
        Grunt,
        Attack,
        Death,
        Hitting,
        Hurt,
        Resurrection,
        Withdraw
            
    }

    [System.Serializable]
    public struct SoundSet
    {
        public SoundSetType Type;
        public List<SetSound> SoundSetSounds;

        internal AudioClip[] GetSoundType(CreatureSound soundType)
        {
            if (!SoundSetSounds.Any(s => s.SoundType == soundType))
                return null;

            return SoundSetSounds.First(s => s.SoundType == soundType).Sounds;

        }
    }

}