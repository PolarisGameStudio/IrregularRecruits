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

        public void ShowXpGain(int from, int to)
        {
            StartCoroutine(XpGainAnimation(from, to));
        }

        private IEnumerator XpGainAnimation(int from, int targetValue)
        {
            var currentLevel = Hero.GetLevel(from);
            var lastLevel = currentLevel;
            var currentValue = from;

            bool leveledUp = false;

            while (currentValue < targetValue)
            {
                currentValue += FillSpeed;

                if(!leveledUp)
                    XpBarText.text = $"{currentValue - from} XP gained!";

                currentLevel = Hero.GetLevel(currentValue);

                var lastXp = currentLevel > 0 ? Hero.LevelCaps[currentLevel - 1] : 0;

                int nextlevelXp = Hero.LevelCaps[currentLevel];

                FillImage.fillAmount = Mathf.Lerp(0f, 1f, (currentValue-lastXp)/(float)(nextlevelXp-lastXp) );

                yield return new WaitForSeconds(0.2f);
                if (lastLevel < currentLevel)
                {
                    lastLevel = currentLevel;
                    leveledUp = true;

                    XpBarText.text = "LEVEL UP!!!";
                    BattleUI.OnLevelUp.Invoke();
                    AnimationSystem.PlayAbilitySelection(XpBarText.transform.position);

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