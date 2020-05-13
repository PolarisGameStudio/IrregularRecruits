using UI;
using UnityEngine;
using Event = GameLogic.Event;

public class SoundController : Singleton<SoundController>
{
    public AudioSource UiAudioSource;
    public AudioSource BackgroundAudioSource;
    public AudioSource StingerAudioSource;
    public AudioSource MusicAudioSource;
    public AudioSource SfxAudioSource;

    private void Awake()
    {
        Event.OnGameBegin.AddListener(() => ChangeMusic(SoundBank.Music.Explore));
        Event.OnGameBegin.AddListener(() => PlayStinger(SoundBank.Stinger.GameStart));

        Event.OnGameOpen.AddListener(() => ChangeMusic(SoundBank.Music.Menu));
        Event.OnGameOpen.AddListener(() => PlayStinger(SoundBank.Stinger.MenuOpen));

        Event.OnGameOver.AddListener(() => ChangeMusic(SoundBank.Music.NoMusic));
        Event.OnGameOver.AddListener(() => PlayStinger(SoundBank.Stinger.GameLoss));

        BattleUI.OnBattleBegin.AddListener(() => ChangeMusic(SoundBank.Music.Battle));

        BattleUI.OnBattleFinished.AddListener(() => ChangeMusic(SoundBank.Music.Battle));
        BattleUI.OnBattleFinished.AddListener(() => PlayStinger(SoundBank.Stinger.BattleWon));

        AnimationSystem.OnDraw.AddListener(() => PlayCardSound(SoundBank.CardSound.Draw));
        AnimationSystem.OnWithdraw.AddListener(() => PlayCardSound(SoundBank.CardSound.Withdraw));
        AnimationSystem.OnEtb.AddListener(() => PlayCardSound(SoundBank.CardSound.ETB));
        AnimationSystem.OnDamaged.AddListener(() => PlayCardSound(SoundBank.CardSound.Hit));
        AnimationSystem.OnHeal.AddListener(() => PlayCardSound(SoundBank.CardSound.Heal));
        AnimationSystem.OnDeath.AddListener(() => PlayCardSound(SoundBank.CardSound.Death));
        AnimationSystem.OnResurrect.AddListener(() => PlayCardSound(SoundBank.CardSound.Resurrect));
    }

    public void PlayButtonClick()
    {
        UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.ButtonClick));

    }

    public static void PlayAbilityTrigger(GameLogic.Ability.ActionType type)
    {
        Instance.StingerAudioSource.PlayOneShot(SoundBank.GetAbilityTrigger(type));
    }

    public static void PlayAbilityHit(GameLogic.Ability.ActionType type)
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
