using GameLogic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class XpBar : MonoBehaviour
    {
        public Image FillImage;
        public TextMeshProUGUI XpBarText;
        public int FillSpeed =1 ;

        public void ShowXpGain(int from, int gain)
        {
            StartCoroutine(XpGainAnimation(from, gain));
        }

        private IEnumerator XpGainAnimation(int from, int gain)
        {
            var currentLevel = Hero.GetLevel(from);
            var startLevel = currentLevel;
            var currentValue = from;
            var targetValue = from + gain;

            while (currentValue < targetValue)
            {
                currentValue += FillSpeed;

                XpBarText.text = $"{currentValue - from} XP gained!";

                currentLevel = Hero.GetLevel(currentValue);

                var lastXp = currentLevel > 0 ? Hero.LevelCaps[currentLevel - 1] : 0;

                int nextlevelXp = Hero.LevelCaps[currentLevel];

                FillImage.fillAmount = Mathf.Lerp(0f, 1f, (currentValue-lastXp)/(float)(nextlevelXp-lastXp) );

                yield return new WaitForSeconds(0.2f);
                if (startLevel < currentLevel)
                {
                    XpBarText.text = "LEVEL UP!!!";

                    //trigger levelup in ui

                    LeanTween.scale(XpBarText.gameObject, new Vector3(1.15f, 1.15f, 1.15f), 1f).setEaseInOutElastic().setOnComplete(() =>
                    {
                        LeanTween.scale(XpBarText.gameObject, new Vector3(1f, 1f, 1f), 0.1f);
                    });

                }
            }




        }
    }
}