using GameLogic;
using MapLogic;
using MapUI;
using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Events;
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

            MapUI.MapUI.Instance.OnLineDraw.AddListener(() => PlayUISound(SoundLibrary.UiSound.MapLineDraw,0.2f));
            MapUI.MapUI.Instance.OnNodeDraw.AddListener(() => PlayUISound(SoundLibrary.UiSound.MapNodeDraw,0.4f));

            Event.OnGameBegin.AddListener(() => ChangeMusic(SoundLibrary.Music.Explore));
            Event.OnGameBegin.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameStart));

            Event.OnGameOpen.AddListener(() => ChangeMusic(SoundLibrary.Music.Menu));
            Event.OnGameOpen.AddListener(() => PlayStinger(SoundLibrary.Stinger.MenuOpen));

            Event.OnAchievement.AddListener(u => PlayStinger(SoundLibrary.Stinger.AchievementUnlocked));
            //LegacyUIEntry.OnUnlock.AddListener(() => PlayStinger(SoundLibrary.Stinger.AchievementUnlocked));

            Event.OnGameOver.AddListener(() => ChangeMusic(SoundLibrary.Music.NoMusic));
            Event.OnGameOver.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameLoss));
            Event.OnGameWin.AddListener(() => PlayStinger(SoundLibrary.Stinger.GameWin));

            AnimationSystem.Instance.OnEtb.AddListener(AnimationSystem.Instance. OnCreatureExclamation.Invoke);

            Shop.OnShopOpen.AddListener(s => ChangeMusic(SoundLibrary.Music.Shop, s.VillageType));
            Shop.OnShopReroll.AddListener(i => PlayUISound(SoundLibrary.UiSound.ShopReroll));
            Shop.OnShopPurchase.AddListener(i => PlayUISound(SoundLibrary.UiSound.PurchaseUnit));

            BattleUI.OnBattleBegin.AddListener(r => ChangeMusic(SoundLibrary.Music.Battle,r));

            BattleUI.OnBattleFinished.AddListener(() => ChangeMusic(SoundLibrary.Music.NoMusic));
            BattleUI.OnBattleFinished.AddListener(() => PlayStinger(SoundLibrary.Stinger.BattleWon));
            BattleUI.OnAbilitySelect.AddListener(a => PlayAbilityHit(a.ResultingAction.ActionType));
            BattleUI.OnLevelAnimation.AddListener(() => PlayCardSound(SoundLibrary.CardSound.LevelUp));
            BattleSummary.Instance.OnClose.AddListener(() => ChangeMusic(SoundLibrary.Music.Explore));
            Event.OnShopClose.AddListener(() => ChangeMusic(SoundLibrary.Music.Explore));

            PlayerGoldUpdater.Instance.OnGoldGained.AddListener(() => PlayUISound(SoundLibrary.UiSound.GoldCoin));

            CardUI.OnDragStarted.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardPickUp));
            CardUI.OnDragEnd.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardLetGo));
            CardLayoutGroup.OnSwitchingPlace.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardSwitchPlace));
            CardHoverInfo.Instance.OnCardHighlight.AddListener(() => PlayUISound(SoundLibrary.UiSound.CardHighlight));

            AnimationSystem.Instance.OnDraw.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Draw));
            AnimationSystem.Instance.OnWithdraw.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Withdraw));
            AnimationSystem.Instance.OnEtb.AddListener((c,b) => PlayCardSound(SoundLibrary.CardSound.ETB));
            AnimationSystem.Instance.OnDamaged.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Hit));
            AnimationSystem.Instance.OnHeal.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Heal));
            AnimationSystem.Instance.OnWardTrigger.AddListener(() => PlayCardSound(SoundLibrary.CardSound.Ward));

            ActionsLeftUI.OnActionGained.AddListener(() => PlayUISound(SoundLibrary.UiSound.ActionsRefreshed));

            AnimationSystem.Instance.OnCreatureExclamation.AddListener((c,bark) => PlayCardSound(bark,c.Creature.SoundSetType));

            AnimationSystem.Instance.OnAbilityTrigger.AddListener(PlayAbilityTrigger);
            AnimationSystem.Instance.OnAbilityTargetHit.AddListener(PlayAbilityHit);

            LegacyUIEntry.OnBarTick.AddListener(() => PlayUISound(SoundLibrary.UiSound.ProgressBarTick));

            AbilityButton.OnHolding.AddListener(button => PlayUISound(SoundLibrary.UiSound.ChargingAbility,()=> !button.Held));
            AbilityButton.OnFizzle.AddListener(() => PlayUISound(SoundLibrary.UiSound.FizzleAbility));
        }

        private void PlayCardSound(CreatureBark soundType, GameLogic.SoundSetType soundSetType)
        {
            AudioClip clip = SoundLibrary.GetSound(soundType, soundSetType);

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
        public static void PlayUISound(SoundLibrary.UiSound type,float volume = 1f)
        {
            Instance.UiAudioSource.PlayOneShot(SoundLibrary.GetSound(type),volume);

        }
        
        public static void PlayUISound(SoundLibrary.UiSound type, Func<bool> stopCondition)
        {
            Instance.StartCoroutine(Instance.PlayUntillRoutine(Instance.UiAudioSource, SoundLibrary.GetSound(type), stopCondition));
        }

        //TODO: create fade
        public static void ChangeMusic(SoundLibrary.Music type,Race race = null)
        {
            if (type == SoundLibrary.Music.NoMusic)
            {
                Instance.MusicAudioSource.clip = null;
                Instance.MusicAudioSource.Stop();
                return;
            }

            var f = SoundLibrary.GetSound(type,race);

            if (Instance.MusicAudioSource.clip != f)
            {
                Instance.MusicAudioSource.clip = f;
                Instance.MusicAudioSource.Play();
            }
        }

        private IEnumerator PlayUntillRoutine(AudioSource audioSource, AudioClip clip, Func<bool> stopAction)
        {
            audioSource.clip = clip;

            audioSource.Play();

            yield return new WaitUntil(stopAction);

            audioSource.clip = null;

        }
    }
}