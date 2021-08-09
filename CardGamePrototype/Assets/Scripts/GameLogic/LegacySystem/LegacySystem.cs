using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    //[InitializeOnLoad]
    public class LegacySystem : SingletonScriptableObject<LegacySystem>
    {
        public List<UnlockCondition> UnlockProgresses;
        public UnityEvent OnLegaciesLoaded = new UnityEvent();

        private void SetStartValues()
        {
            foreach (var u in UnlockProgresses)
                u.StartRun();
        }

        public IEnumerator ImportRoutine()
        {
            Event.OnBattleFinished.AddListener(HandleBattleFinished);

            Debug.Log("Started importing data");

            yield return new WaitUntil(()=> DataHandler.Instance.PersistantDataObject.databaseLoaded);

            foreach (var unlock in UnlockProgresses)
            {
                var value = DataHandler.Instance.GetData(unlock.UnlocksHero.name);

                unlock.Count = value.Value;

                unlock.OnCountUp.AddListener(i => ChangeIntTypeValue(i, value, unlock.UnlocksHero.name));

                Debug.Log("synced: " + unlock.Description());

            }
            SetStartValues();

            OnLegaciesLoaded.Invoke();
        }

        internal bool IsUnlocked(HeroObject arg)
        {
            return !UnlockProgresses.Any(con => con.UnlocksHero== arg && !con.Unlocked());
        }

        private void ChangeIntTypeValue(int i, IntType data,string name = "")
        {
            Debug.Log($"data {name}, changed from {data.Value} to {i}");

            data.Value = i;

            DataHandler.Instance.Save();

            //TODO: Create a test that changes the value and gets and

            Debug.Log("actual value in Databox:" + DataHandler.Instance.GetData(name).Value);
        }

        private void HandleBattleFinished(Deck winner, Deck loser)
        {
            if (winner != Battle.PlayerDeck)
                return;

            foreach(var condition in UnlockProgresses)
            {
                if (condition.Condition != UnlockConditionType.WinBattle && condition.Condition != UnlockConditionType.WinHardBattle)
                    continue;

                if (condition.Against & !loser.Races.Contains(condition.Against))
                    continue;

                condition.CountUp();

                //Debug.Log("Increasing: " + condition.Description());
            }
        }


    }
}