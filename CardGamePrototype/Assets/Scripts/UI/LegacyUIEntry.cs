using GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class LegacyUIEntry : UIEntry<UnlockCondition>
{
    public Sprite UnlockedFrame, LockedFrame;
    

    public Image FrameImage;
    public Image PortraitImage;
    public Button Button;
    public Image LockIcon;

    public override void Open(UnlockCondition data)
    {
        FrameImage.sprite = data.Unlocked() ? UnlockedFrame : LockedFrame;
        LockIcon.gameObject.SetActive(!data.Unlocked());

        PortraitImage.sprite = data.UnlocksHero.Portrait;
    }
}
