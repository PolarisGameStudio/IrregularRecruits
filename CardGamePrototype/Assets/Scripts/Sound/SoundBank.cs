using GameLogic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SoundBank : Singleton<SoundBank>
{

    public enum UiSound
    {
        TapClick,
        ButtonClick,
        CardClick,
        PopUp,
        Event,
        LevelUp,
        NextRound
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
    public enum Background
    {
        Forest,
        Night
    }

    public enum Music
    {
        NoMusic,
        Explore,
        Battle,
        Menu
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
        LevelUp
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
        public Ability.ActionType Type;
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
    public struct BackgroundSoundref
    {
        public Background Type;
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
    public BackgroundSoundref[] Backgrounds;
    public MusicRef[] Musics;
    public AbilitySound[] AbilitySounds;


    internal static AudioClip GetSound(CardSound type)
    {
        if (!Instance.FXReferences.Any(s => s.Type == type))
        {
            Debug.LogWarning("No sound for " + type);
            return null;
        }

        return Rnd(Instance.FXReferences.First(s => s.Type == type).Audio);
    }

    internal static AudioClip GetSound(Background type)
    {
        if (!Instance.Backgrounds.Any(s => s.Type == type))
        {
            Debug.LogWarning("No sound for " + type);
            return null;
        }

        return Rnd(Instance.Backgrounds.First(s => s.Type == type).Audio);
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

    public static AudioClip GetAbilityTrigger(Ability.ActionType sound)
    {
        if (!Instance.AbilitySounds.Any(s => s.Type == sound))
        {
            Debug.LogWarning("No sound for " + sound);
            return null;
        }

        return Rnd(Instance.AbilitySounds.First(s => s.Type == sound).TriggerAudio);
    }
    public static AudioClip GetAbilityHit(Ability.ActionType sound)
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
