using Data;
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
        public int BattlesWon;
        public int GoldGained;

        //Stats
        public Deck LostTo;


        private void OnEnable()
        {
            GoldGained = BattlesWon = 0;

            Event.OnStatScreen.AddListener(Open);
            Event.OnBattleFinished.AddListener(HandleBattleStats);
            RestartButton.onClick.AddListener(Restart);
            Event.OnPlayerGoldAdd.AddListener(i => GoldGained += Mathf.Max(0, i));
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void HandleBattleStats(Deck winner, Deck loser)
        {
            if (winner == Battle.PlayerDeck)
            {
                BattlesWon++;
            }
            else
                LostTo = winner;
        }

        public override void Open()
        {
            base.Open();
            var hero = Battle.PlayerDeck.Hero;
            HeroImage.sprite = hero.HeroObject.Portrait;

            UnlockEntry.gameObject.SetActive(false);

            LossDescriptionText.text = hero.GetName() + "\n was killed by a group of " + LostTo.Races.First().name;

            var killed = BattleSummary.TotalKilled;
            var xp = hero.Xp;

            var text = "Battles won";
            var score = BattlesWon;

            SetupStatEntry(text, score);
            SetupStatEntry("Units killed", killed);
            SetupStatEntry("Gold gained", GoldGained);
            SetupStatEntry("Xp gained", xp);

            StatTextEntry.gameObject.SetActive(false);

            StartCoroutine(ShowUnlocksRoutine());
        }

        private void SetupStatEntry(string text, int score)
        {
            var savedScore = DataHandler.Instance.GetData<IntType>($"highscore.{text}","Highscores","0");

            var newHighscore = savedScore.Value < score;

            if (newHighscore) savedScore.Value = score;

            var inst = Instantiate(StatTextEntry, StatTextEntry.transform.parent);

            inst.text = text + ": " + score + " "+ 
                (
                newHighscore ? "*NEW HIGH SCORE*"
                : $"(High score: {savedScore.Value})"
                ) ;



        }

        private IEnumerator ShowUnlocksRoutine()
        {

            var unlocksImproved = LegacySystem.Instance.UnlockProgresses.Where(uc => !uc.UnlockedAtStart && uc.StartedAt < uc.Count);

            //TODO: filter so we only show the easiest unlockable of each unlocktype

            var instances = new List<LegacyUIEntry>();

            foreach (var uc in unlocksImproved)
            {
                var inst = Instantiate(UnlockEntry, UnlockEntry.transform.parent);

                inst.gameObject.SetActive(true);

                inst.Open(uc);
                inst.SetUnlockStatus(uc.UnlockedAtStart);

                instances.Add(inst);

            }

            yield return new WaitForSeconds(1.2f);

            foreach (var inst in instances)
                yield return inst.AnimateProgress();

        }
    }
}