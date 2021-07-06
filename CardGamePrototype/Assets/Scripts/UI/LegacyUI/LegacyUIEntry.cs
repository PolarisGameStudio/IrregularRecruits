using GameLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{
    public class LegacyUIEntry : UIEntry<UnlockCondition>
    {
        public Sprite UnlockedFrame, LockedFrame;

        public UnlockCondition Data;

        public Image FrameImage;
        public Image PortraitImage;
        public Button Button;
        public Image LockIcon;

        public Image ProgressFill;

        public float ProgressFillDelta = 0.05f;

        public static UnityEvent OnBarTick = new UnityEvent();

        public override void Open(UnlockCondition data)
        {
            Data = data;

            bool unlocked = Data.Unlocked();
            SetUnlockStatus(unlocked);

            PortraitImage.sprite = Data.UnlocksHero.Portrait;

            Button?.onClick.AddListener(() => LegacyAchievementUI.Instance.Open(Data));


        }

        public void SetUnlockStatus(bool unlocked)
        {
            FrameImage.sprite = unlocked ? UnlockedFrame : LockedFrame;
            LockIcon.gameObject.SetActive(!unlocked);

            if (ProgressFill)
                ProgressFill.fillAmount =
                    unlocked ?
                    1f : Data.StartedAt / (float)Data.UnlocksAt;
        }

        public IEnumerator AnimateProgress()
        {
            var data = Data;

            if (Data == null||!ProgressFill || data.UnlockedAtStart)
            {
                yield break;
            }

            float x = data.StartedAt;

            while (x < data.Count && x < data.UnlocksAt)
            {
                ProgressFill.fillAmount += ProgressFillDelta;

                if (ProgressFill.fillAmount >= (x + 1) / data.UnlocksAt)
                    x++;

                OnBarTick.Invoke();

                yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
            }

            if(data.Unlocked())
            {
                ProgressFill.color = Color.white;

                SetUnlockStatus(true);

                //unlock animation

                AnimationSystem.PlayLevelupFX(PortraitImage.transform.position);
                Event.OnAchievement.Invoke(data);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}