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
            Hit,
            BigHit,
            ETB,
            Death,
            Withdraw,
            Draw,
            NoAbilityTarget,
            Resurrect,
            Heal,
            AbilitySelection,
            LevelUp,
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


        private static AudioClip Rnd(AudioClip[] arr)
        {
            if (arr.Length == 0)
                return null;

            return arr[Random.Range(0, arr.Length)];
        }

    }

}