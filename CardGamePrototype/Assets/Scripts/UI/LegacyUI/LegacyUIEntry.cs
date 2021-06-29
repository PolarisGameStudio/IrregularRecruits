using GameLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LegacyUIEntry : UIEntry<UnlockCondition>
    {
        public Sprite UnlockedFrame, LockedFrame;


        public Image FrameImage;
        public Image PortraitImage;
        public Button Button;
        public Image LockIcon;

        public Image ProgressFill;

        public override void Open(UnlockCondition data)
        {
            FrameImage.sprite = data.Unlocked() ? UnlockedFrame : LockedFrame;
            LockIcon.gameObject.SetActive(!data.Unlocked());

            PortraitImage.sprite = data.UnlocksHero.Portrait;

            Button?.onClick.AddListener(() => LegacyAchievementUI.Instance.Open(data));



        }

        public IEnumerator AnimateProgress(UnlockCondition data)
        {
            if (!ProgressFill || data.UnlockedAtStart)
            {
                yield break;
            }


            var x = data.StartedAt;

            while (x <= data.Count && x <= data.UnlocksAt)
            {
                ProgressFill.fillAmount = x++ / (float) data.UnlocksAt;

                yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
            }

            if(data.Unlocked())
            {
                ProgressFill.color = Color.white;

                //todo: play unlock animation
            }
        }
    }
}