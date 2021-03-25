using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameLogic
{
    [InitializeOnLoad]
    public class LegacySystem : SingletonScriptableObject<LegacySystem>
    {
        public List<UnlockCondition> UnlockProgresses;

        private void OnEnable()
        {
            if (UnlockProgresses.Select(u => u.UnlocksHero.name).Distinct().Count() != UnlockProgresses.Count())
                Debug.LogError("Several unlock conditions for the same hero!");

            foreach(var unlock in UnlockProgresses)
            {
                var value = DataHandler.Instance.GetLegacy(unlock.UnlocksHero.name);

                unlock.Count = value.Value;

                unlock.OnCountUp.AddListener(i => value.Value = i);

                Debug.Log("synced: " + unlock.Description());

            }

            Event.OnBattleFinished.AddListener(HandleBattleFinished);
        }

        internal bool IsUnlocked(HeroObject arg)
        {
            return !UnlockProgresses.Any(con => con.UnlocksHero== arg && !con.Unlocked());
        }

        private void HandleBattleFinished(Deck winner, Deck loser)
        {
            if (winner != BattleManager.Instance.PlayerDeck)
                return;

            foreach(var condition in UnlockProgresses)
            {
                if (condition.Condition != UnlockConditionType.WinBattle && condition.Condition != UnlockConditionType.WinHardBattle)
                    continue;

                if (condition.Against & !loser.Races.Contains(condition.Against))
                    continue;

                condition.CountUp();

                Debug.Log("Increasing: " + condition.Description());
            }
        }


    }
}