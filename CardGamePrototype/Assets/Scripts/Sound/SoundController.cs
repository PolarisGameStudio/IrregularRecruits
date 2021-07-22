using MapLogic;
using MapUI;
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

            MapNodeIcon.OnMapButtonClick.AddListener(PlayButtonClick);

            MapNodeIcon.OnMapButtonClick.AddListener(() => PlayUISound(SoundLibrary.UiSound.HeroWalking));

            Event.OnGameBegin.AddListener(() => ChangeMusic(SoundLibrary.Music.Explore));
            Event.OnGameBegin.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameStart));

            Event.OnGameOpen.AddListener(() => ChangeMusic(SoundLibrary.Music.Menu));
            Event.OnGameOpen.AddListener(() => PlayStinger(SoundLibrary.Stinger.MenuOpen));

            Event.OnAchievement.AddListener(u => PlayStinger(SoundLibrary.Stinger.AchievementUnlocked));
            //LegacyUIEntry.OnUnlock.AddListener(() => PlayStinger(SoundLibrary.Stinger.AchievementUnlocked));


            Event.OnGameOver.AddListener(() => ChangeMusic(SoundLibrary.Music.NoMusic));
            Event.OnGameOver.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameLoss));
            Event.OnGameWin.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameWin));
            Shop.OnShopOpen.AddListener(s => ChangeMusic(SoundLibrary.Music.Shop));
            Shop.OnShopReroll.AddListener(i => PlayUISound(SoundLibrary.UiSound.ShopReroll));
            Shop.OnShopPurchase.AddListener(i => PlayUISound(SoundLibrary.UiSound.PurchaseUnit));

            BattleUI.OnBattleBegin.AddListener(() => ChangeMusic(SoundLibrary.Music.Battle));

            BattleUI.OnBattleFinished.AddListener(() => ChangeMusic(SoundLibrary.Music.NoMusic));
            BattleUI.OnBattleFinished.AddListener(() => PlayStinger(SoundLibrary.Stinger.BattleWon));
            BattleUI.OnAbilitySelect.AddListener(a => PlayAbilityHit(a.ResultingAction.ActionType));
            BattleUI.OnLevelAnimation.AddListener(() => PlayCardSound(SoundLibrary.CardSound.LevelUp));
            BattleSummary.Instance.OnClose.AddListener(() => ChangeMusic(SoundLibrary.Music.Explore));

            PlayerGoldUpdater.Instance.OnGoldGained.AddListener(() => PlayUISound(SoundLibrary.UiSound.GoldCoin));

            CardUI.OnDragStarted.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardPickUp));
            CardUI.OnDragEnd.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardLetGo));
            CardLayoutGroup.OnSwitchingPlace.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardSwitchPlace));
            CardHoverInfo.Instance.OnCardHighlight.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardHighlight));

            AnimationSystem.OnDraw.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Draw));
            AnimationSystem.OnWithdraw.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Withdraw));
            AnimationSystem.OnEtb.AddListener(() => PlayCardSound(SoundLibrary.CardSound.ETB));
            AnimationSystem.OnDamaged.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Hit));
            AnimationSystem.OnHeal.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Heal));
            AnimationSystem.OnWardTrigger.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Ward));

            ActionsLeftUI.OnActionGained.AddListener(() => PlayUISound(SoundLibrary.UiSound.ActionsRefreshed));

            AnimationSystem.OnCreatureExclamation.AddListener((c,bark) => PlayCardSound(bark,c.Creature.SoundSetType));

            AnimationSystem.OnAbilityTrigger.AddListener(PlayAbilityTrigger);
            AnimationSystem.OnAbilityTargetHit.AddListener(PlayAbilityHit);

            LegacyUIEntry.OnBarTick.AddListener(() => PlayUISound(SoundLibrary.UiSound.ProgressBarTick));
        }

        private void PlayCardSound(CreatureBark soundType, GameLogic.SoundSetType soundSetType)
        {
            AudioClip clip = SoundLibrary.GetSound(soundType, GameLogic.SoundSetType.Standard);

            if(clip)
                SfxAudioSource.PlayOneShot(clip);
        }

        public void PlayButtonClick()
        {
            UiAudioSource.PlayOneShot(SoundLibrary.GetSound(SoundLibrary.UiSound.ButtonClick));

        }

        public static void PlayAbilityTrigger(GameLogic.EffectType type)
        {
            Instance.StingerAudioSource.PlayOneShot(SoundLibrary.GetAbilityTrigger(type));
        }

        public static void PlayAbilityHit(GameLogic.EffectType type)
        {
            Instance.StingerAudioSource.PlayOneShot(SoundLibrary.GetAbilityHit(type));
        }

        public static void PlayStinger(SoundLibrary.Stinger type)
        {
            Instance.StingerAudioSource.clip = SoundLibrary.GetSound(type);

            Instance.StingerAudioSource.Play();

        }
        public static void PlayCardSound(SoundLibrary.CardSound type)
        {
            Instance.SfxAudioSource.PlayOneShot(SoundLibrary.GetSound(type));

        }
        public static void PlayUISound(SoundLibrary.UiSound type)
        {
            Instance.UiAudioSource.PlayOneShot(SoundLibrary.GetSound(type));

        }


        //TODO: create fade
        public static void ChangeMusic(SoundLibrary.Music type)
        {
            if (type == SoundLibrary.Music.NoMusic)
            {
                Instance.MusicAudioSource.clip = null;
                Instance.MusicAudioSource.Stop();
            }

            var f = SoundLibrary.GetSound(type);

            if (Instance.MusicAudioSource.clip != f)
            {
                Instance.MusicAudioSource.clip = f;
                Instance.MusicAudioSource.Play();
            }
        }
    }
}