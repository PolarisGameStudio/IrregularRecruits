using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{

    public class EndStatScreen : UIWindow
    {
        public Image HeroImage;
        public TextMeshProUGUI StatTextEntry;
        public LegacyUIEntry UnlockEntry;
        public TextMeshProUGUI LossDescriptionText;
        public Button RestartButton;


        //Stats
        public Deck LostTo;


        private void OnEnable()
        {
            Event.OnStatScreen.AddListener(Open);
            Event.OnBattleFinished.AddListener(HandleBattleStats);
            RestartButton.onClick.AddListener(Restart);
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void HandleBattleStats(Deck winner, Deck loser)
        {
            if (winner == BattleManager.Instance.PlayerDeck)
            {

            }
            else
                LostTo = winner;
        }

        public override void Open()
        {
            base.Open();
            HeroObject heroObject = BattleManager.Instance.PlayerDeck.Hero.HeroObject;
            HeroImage.sprite = heroObject.Portrait;

            UnlockEntry.gameObject.SetActive(false);

            LossDescriptionText.text = heroObject.name + "\n was killed by a group of " + LostTo.Races.First().name;

            var killed = BattleSummary.TotalKilled;

            StartCoroutine( ShowUnlocksRoutine());
        }

        private IEnumerator ShowUnlocksRoutine()
        {
            var unlocksImproved = LegacySystem.Instance.UnlockProgresses.Where(uc => !uc.UnlockedAtStart && uc.StartedAt < uc.Count);

            //TODO: filter so we only show the easiest unlockable of each unlocktype

            foreach (var uc in unlocksImproved)
            {
                var inst = Instantiate(UnlockEntry, UnlockEntry.transform.parent);

                inst.gameObject.SetActive(true);

                inst.Open(uc);

                yield return inst.AnimateProgress(uc);
            }

        }
    }
}