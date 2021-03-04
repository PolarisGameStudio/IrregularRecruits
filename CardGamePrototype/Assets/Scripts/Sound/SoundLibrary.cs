using GameLogic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sound
{
    [CreateAssetMenu]
    public class SoundLibrary : SingletonScriptableObject<SoundLibrary>
    {
        public enum UiSound
        {
            TapClick,
            ButtonClick,
            CardClick,
            PopUp,
            Event,
            LevelUp,
            NextRound,
            GoldCoin,
            CardPickUp,
            CardSwitchPlace,
            CardHighlight,
            CardLetGo,
            HeroWalking,
            ShopReroll,
            PurchaseUnit
        }

        public enum Stinger
        {
            GameLoss,
            GameStart,
            GameWin,
            BattleWon,
            BattleLost,
            AchievementUnlocked,
            MenuOpen,
        }
        public enum Music
        {
            NoMusic,
            Explore,
            Battle,
            Menu,
            Shop,
            Event
        }

        public enum CardSound
        {
            Hit = 0,
            BigHit = 1,
            ETB = 2,
            //Death = 3,
            Withdraw = 4,
            Draw = 5,
            NoAbilityTarget = 6,
            Resurrect = 7,
            Heal = 8,
            AbilitySelection = 9,
            LevelUp = 10,
        }



        [System.Serializable]
        public struct FXReference
        {
            public CardSound Type;
            public AudioClip[] Audio;
        }


        [System.Serializable]
        public struct RaceSpecificSound
        {
            public CardSound Type;
            public AudioClip[] Audio;
        }

        [System.Serializable]
        public struct AbilitySound
        {
            public EffectType Type;
            public AudioClip[] TriggerAudio;
            public AudioClip[] TargetAudio;
        }

        [System.Serializable]
        public struct UiSoundReference
        {
            public UiSound Type;
            public AudioClip[] Audio;
        }
        [System.Serializable]
        public struct StingerSoundReference
        {
            public Stinger Type;
            public AudioClip[] Audio;
        }
        [System.Serializable]
        public struct MusicRef
        {
            public Music Type;
            public AudioClip[] Audio;
        }

        public FXReference[] FXReferences;
        public UiSoundReference[] UiSounds;
        public StingerSoundReference[] Stingers;
        public MusicRef[] Musics;
        public AbilitySound[] AbilitySounds;
        public RaceSpecificSound[] VillageSounds;
        public SoundSet[] CreatureSoundSets;

        internal static AudioClip GetSound(CardSound type)
        {
            if (!Instance.FXReferences.Any(s => s.Type == type))
            {
                Debug.LogWarning("No sound for " + type);
                return null;
            }

            return Rnd(Instance.FXReferences.First(s => s.Type == type).Audio);
        }

        internal static AudioClip GetSound(Stinger sound)
        {
            if (!Instance.Stingers.Any(s => s.Type == sound))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            return Rnd(Instance.Stingers.First(s => s.Type == sound).Audio);
        }

        public static AudioClip GetSound(UiSound sound)
        {
            if (!Instance.UiSounds.Any(s => s.Type == sound))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            return Rnd(Instance.UiSounds.First(s => s.Type == sound).Audio);
        }

        public static AudioClip GetSound(Music sound)
        {
            if (!Instance.Musics.Any(s => s.Type == sound))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            return Rnd(Instance.Musics.First(s => s.Type == sound).Audio);
        }


        public static AudioClip GetAbilityTrigger(EffectType sound)
        {
            if (!Instance.AbilitySounds.Any(s => s.Type == sound))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            return Rnd(Instance.AbilitySounds.First(s => s.Type == sound).TriggerAudio);
        }
        public static AudioClip GetAbilityHit(EffectType sound)
        {
            if (!Instance.AbilitySounds.Any(s => s.Type == sound))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            return Rnd(Instance.AbilitySounds.First(s => s.Type == sound).TargetAudio);
        }

        internal static AudioClip GetSound(CreatureSound soundType, SoundSetType soundSetType)
        {
            SoundSet set = Instance.GetSoundSetOrStandard(soundSetType);

            AudioClip[] audioClips = set.GetSoundType(soundType);

            if (audioClips != null && audioClips.Any())
                return Rnd(audioClips);

            return null;
        }



        private static AudioClip Rnd(AudioClip[] arr)
        {
            if (arr.Length == 0)
                return null;

            return arr[Random.Range(0, arr.Length)];
        }

        private SoundSet GetStandardSoundSet()
        {
            return CreatureSoundSets.First(s => s.Type == SoundSetType.Standard);
        }

        private SoundSet GetSoundSetOrStandard(SoundSetType soundSet)
        {
            System.Func<SoundSet, bool> anySoundSetOfType = s => s.Type == soundSet;

            SoundSet set;

            if (CreatureSoundSets.Any(anySoundSetOfType))
                set = CreatureSoundSets.First(anySoundSetOfType);
            else
                set = GetStandardSoundSet();
            return set;
        }


    }

}