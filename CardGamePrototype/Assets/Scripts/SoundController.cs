using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    public AudioSource UiAudioSource;
    public AudioSource BackgroundAudioSource;
    public AudioSource StingerAudioSource;
    public AudioSource MusicAudioSource;
    public AudioSource SfxAudioSource;

    private void Awake()
    {
        Event.OnGameBegin.AddListener(()=> ChangeMusic(SoundBank.Music.Explore));
        Event.OnGameBegin.AddListener(()=> PlayStinger(SoundBank.Stinger.GameStart));

        Event.OnGameOpen.AddListener(() => ChangeMusic(SoundBank.Music.Menu));
        Event.OnGameOpen.AddListener(()=> PlayStinger(SoundBank.Stinger.MenuOpen));

        Event.OnGameOver.AddListener(() => ChangeMusic(SoundBank.Music.NoMusic));
        Event.OnGameOver.AddListener(()=> PlayStinger(SoundBank.Stinger.GameLoss));
        
        Event.OnCombatStart.AddListener(() => ChangeMusic(SoundBank.Music.Battle));
        
        Event.OnCombatFinished.AddListener(() => ChangeMusic(SoundBank.Music.Battle));
        Event.OnCombatFinished.AddListener(() => PlayStinger(SoundBank.Stinger.BattleWon));

        Event.OnDraw.AddListener(c => PlayCardSound(SoundBank.CardSound.Draw));
        Event.OnWithdraw.AddListener(c => PlayCardSound(SoundBank.CardSound.Withdraw));
        Event.OnPlay.AddListener(c => PlayCardSound(SoundBank.CardSound.ETB));
        Event.OnDamaged.AddListener(c => PlayCardSound(SoundBank.CardSound.Hit));
        Event.OnDeath.AddListener(c => PlayCardSound(SoundBank.CardSound.Death));
    }

    public void PlayButtonClick()
    {
        UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.ButtonClick));

    }

    public static void PlayAbilityTrigger(Ability.ActionType type)
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetAbilityTrigger(type));
    }
    
    public static void PlayAbilityHit(Ability.ActionType type)
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetAbilityHit(type));
    }

        public static void PlayStinger(SoundBank.Stinger type)
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetSound(type));

    }
    public static void PlayCardSound(SoundBank.CardSound type)
    {
        Instance.SfxAudioSource.PlayOneShot(SoundBank.GetSound(type));

    }
    public static void PlayUISound(SoundBank.UiSound type)
    {
        Instance.UiAudioSource.PlayOneShot(SoundBank.GetSound(type));

    }

    
    //TODO: create fade
    public static void ChangeMusic(SoundBank.Music type)
    {
        if (type == SoundBank.Music.NoMusic)
        {
            Instance.MusicAudioSource.clip = null;
            Instance.MusicAudioSource.Stop();
        }

        var f = SoundBank.GetSound(type);
        
        if (Instance.MusicAudioSource.clip != f)
        {
            Instance.MusicAudioSource.clip = f;
            Instance.MusicAudioSource.Play();
        }
    }
    public static void ChangeBackground(SoundBank.Background type)
    {
        var f = SoundBank.GetSound(type);


        if (Instance.BackgroundAudioSource.clip != f)
        {
            Instance.BackgroundAudioSource.clip = f;
            Instance.BackgroundAudioSource.Play();
        }
    }
}
