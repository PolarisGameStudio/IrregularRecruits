using MapLogic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace Sound
{
    public class SoundController : Singleton<SoundController>
    {
        public AudioSource UiAudioSource;
        public AudioSource StingerAudioSource;
        public AudioSource MusicAudioSource;
        public AudioSource SfxAudioSource;

        private void Awake()
        {
            var buttons = Resources.FindObjectsOfTypeAll<Button>();

            foreach (var b in buttons)
                b.onClick.AddListener(PlayButtonClick);

            Event.OnGameBegin.AddListener(() => ChangeMusic(SoundBank.Music.Explore));
            Event.OnGameBegin.AddListener(() => PlayStinger(SoundBank.Stinger.GameStart));

            Event.OnGameOpen.AddListener(() => ChangeMusic(SoundBank.Music.Menu));
            Event.OnGameOpen.AddListener(() => PlayStinger(SoundBank.Stinger.MenuOpen));

            Event.OnGameOver.AddListener(() => ChangeMusic(SoundBank.Music.NoMusic));
            Event.OnGameOver.AddListener(() => PlayStinger(SoundBank.Stinger.GameLoss));
            Event.OnGameWin.AddListener(() => PlayStinger(SoundBank.Stinger.GameWin));
            Shop.OnShopOpen.AddListener(s => ChangeMusic(SoundBank.Music.Shop));

            BattleUI.OnBattleBegin.AddListener(() => ChangeMusic(SoundBank.Music.Battle));

            BattleUI.OnBattleFinished.AddListener(() => ChangeMusic(SoundBank.Music.NoMusic));
            BattleUI.OnBattleFinished.AddListener(() => PlayStinger(SoundBank.Stinger.BattleWon));
            BattleUI.OnAbilitySelect.AddListener(() => PlayCardSound(SoundBank.CardSound.AbilitySelection));
            BattleUI.OnLevelUp.AddListener(() => PlayCardSound(SoundBank.CardSound.LevelUp));
            BattleSummary.Instance.OnClose.AddListener(() => ChangeMusic(SoundBank.Music.Explore));

            PlayerGoldUpdater.Instance.OnGoldGained.AddListener(() => PlayCardSound(SoundBank.CardSound.GoldCoin));


            AnimationSystem.OnDraw.AddListener(() => PlayCardSound(SoundBank.CardSound.Draw));
            AnimationSystem.OnWithdraw.AddListener(() => PlayCardSound(SoundBank.CardSound.Withdraw));
            AnimationSystem.OnEtb.AddListener(() => PlayCardSound(SoundBank.CardSound.ETB));
            AnimationSystem.OnDamaged.AddListener(() => PlayCardSound(SoundBank.CardSound.Hit));
            AnimationSystem.OnHeal.AddListener(() => PlayCardSound(SoundBank.CardSound.Heal));
            AnimationSystem.OnDeath.AddListener(() => PlayCardSound(SoundBank.CardSound.Death));
            AnimationSystem.OnResurrect.AddListener(() => PlayCardSound(SoundBank.CardSound.Resurrect));

            AnimationSystem.OnAbilityTrigger.AddListener(PlayAbilityTrigger);
            AnimationSystem.OnAbilityTargetHit.AddListener(PlayAbilityHit);
        }

        public void PlayButtonClick()
        {
            UiAudioSource.PlayOneShot(SoundBank.GetSound(SoundBank.UiSound.ButtonClick));

        }

        public static void PlayAbilityTrigger(GameLogic.EffectType type)
        {
            Instance.StingerAudioSource.PlayOneShot(SoundBank.GetAbilityTrigger(type));
        }

        public static void PlayAbilityHit(GameLogic.EffectType type)
        {
            Instance.StingerAudioSource.PlayOneShot(SoundBank.GetAbilityHit(type));
        }

        public static void PlayStinger(SoundBank.Stinger type)
        {
            Instance.StingerAudioSource.clip = SoundBank.GetSound(type);

            Instance.StingerAudioSource.Play();

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
    }
}