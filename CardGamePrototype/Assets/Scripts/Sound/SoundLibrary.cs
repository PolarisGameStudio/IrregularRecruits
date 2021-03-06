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
            PurchaseUnit,
            ActionsRefreshed,
            BloodCut,
            ProgressBarTick,
            ChargingAbility,
            FizzleAbility,
            MapLineDraw,
            MapNodeDraw
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
            Ward = 11,
        }



        [System.Serializable]
        public struct FXReference
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
            public Race RaceSpecific;
            public AudioClip[] Audio;
        }

        public FXReference[] FXReferences;
        public UiSoundReference[] UiSounds;
        public StingerSoundReference[] Stingers;
        public MusicRef[] Musics;
        public AbilitySound[] AbilitySounds;
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

        public static AudioClip GetSound(Music sound,Race race =null)
        {
            System.Func<MusicRef, bool> predicate = s => s.Type == sound;
            if (!Instance.Musics.Any(predicate))
            {
                Debug.LogWarning("No sound for " + sound);
                return null;
            }

            System.Func<MusicRef, bool> racePredicate = s => s.Type == sound && s.RaceSpecific == race;

            if (race && Instance.Musics.Any(racePredicate))
                return Rnd(Instance.Musics.First(racePredicate).Audio);

            return Rnd(Instance.Musics.First(predicate).Audio);
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

        internal static AudioClip GetSound(CreatureBark soundType, SoundSetType soundSetType)
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

            if (CreatureSoundSets.Any(anySoundSetOfType))
                return CreatureSoundSets.First(anySoundSetOfType);
            else
            {
                Debug.LogWarning("No sound set of type: " + soundSet);
                return GetStandardSoundSet();
            }
        }


    }

}