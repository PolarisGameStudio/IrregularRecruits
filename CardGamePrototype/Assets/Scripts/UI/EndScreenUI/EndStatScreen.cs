using Data;
using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        public int BattlesWon;
        public int GoldGained;

        //Stats
        public Deck LostTo;

        public GameObject BloodAnimation;
        public GameObject Crown;

        private bool PlayerLost;

        private void OnEnable()
        {
            GoldGained = BattlesWon = 0;

            Event.OnStatScreen.AddListener(Open);
            Event.OnBattleFinished.AddListener(HandleBattleStats);
            Event.OnPlayerGoldAdd.AddListener(i => GoldGained += Mathf.Max(0, i));
        }

        private void HandleBattleStats(Deck winner, Deck loser)
        {
            if (winner == Battle.PlayerDeck)
            {
                BattlesWon++;
            }
            else
            {
                PlayerLost = true;
                if (LostTo != null)
                    LostTo = winner;
            }
        }

        public override void Open()
        {
            base.Open();
            var hero = Battle.PlayerDeck.Hero;
            HeroImage.sprite = hero.HeroObject.Portrait;

            UnlockEntry.gameObject.SetActive(false);

            if (PlayerLost)
            {
                if (LostTo != null)
                {
                    LossDescriptionText.text = hero.GetName() + "\n was killed by a group of " + LostTo.Races.First().name;
                }
                else
                    LossDescriptionText.text = hero.GetName() + "\n was broken by the Cinder Lands" ;

                BloodAnimation.SetActive(true);

            }
            else
            {
                LossDescriptionText.text = hero.GetName() + "\n vanquished the demon Konallath";

                Crown.SetActive(true);
                //TODO crown animation and sound
            }

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

            if (newHighscore)
            {
                savedScore.Value = score;
                DataHandler.Instance.Save();
            }

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