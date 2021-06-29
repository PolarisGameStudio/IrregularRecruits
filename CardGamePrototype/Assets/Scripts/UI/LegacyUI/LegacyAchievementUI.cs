using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{

    public class LegacyAchievementUI : Singleton<LegacyAchievementUI>,IPointerClickHandler
    {
        public GameObject Holder;
        public CanvasGroup FocusGroup;
        public TextMeshProUGUI UnlocksText;
        public TextMeshProUGUI ConditionText;
        public TextMeshProUGUI ProgressText;
        public LegacyUIEntry PortraitUI;
        public Color UnlockedColor;
        public Sprite UnlockedBackground, LockedBackground;
        public Image Background;

        public void OnPointerClick(PointerEventData eventData)
        {
            Close();
        }

        public void Open(UnlockCondition unlockCondition)
        {
            Holder.SetActive(true);

            PortraitUI.Open(unlockCondition);
            UnlocksText.text = //"Unlock " + 
                unlockCondition.UnlocksHero.name;

            ConditionText.text = $"Win {unlockCondition.UnlocksAt} battles" + (
                unlockCondition.Against ? $" against {unlockCondition.Against.name}" : "");

            ProgressText.text = unlockCondition.Unlocked() ? "Unlocked!" : $"{ unlockCondition.Count}/{unlockCondition.UnlocksAt}";

            ProgressText.color = unlockCondition.Unlocked() ? UnlockedColor : Color.white;

            Background.sprite = unlockCondition.Unlocked() ? UnlockedBackground : LockedBackground;
        }

        public void Close()
        {
            Holder.SetActive(false);
        
        }

    }
}