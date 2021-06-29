using GameLogic;
using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class LegacyUI : EntryPopulaterUI<LegacyUIEntry, UnlockCondition>, IUIWindow
    {
        public CanvasGroup FocusGroup;
        public static LegacyUI Instance;

        public LegacyAchievementUI AchievementUI;

        private void Awake()
        {
            Instance = this;
        }

        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder()
        {
            return Holder;
        }

        public int GetPriority()
            => 11;

        public void Open()
        {
            Open(LegacySystem.Instance.UnlockProgresses);
        }
    }
}