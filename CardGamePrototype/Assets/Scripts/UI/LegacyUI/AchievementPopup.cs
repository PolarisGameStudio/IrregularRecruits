using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameLogic;
using Event = GameLogic.Event;

namespace UI
{
    public class AchievementPopup : Singleton<AchievementPopup>
    {
        public GameObject Holder;
        public TextMeshProUGUI TitleText;
        public LegacyUIEntry PortraitUI;

        private void Start()
        {
            Event.OnAchievement.AddListener(Popup);

            Holder.SetActive(false);
        }

        private void Popup(UnlockCondition unlock)
        {
            TitleText.text = unlock.UnlocksHero.name;

            PortraitUI.Open(unlock);

            Holder.SetActive(true);

            Holder.transform.localScale = Vector3.zero;

            Holder.transform.LeanScale(Vector3.one, 1.6f).setEaseInOutExpo();

            LeanTween.delayedCall(4f, ()=> Holder.SetActive(false));
        }
    }
}