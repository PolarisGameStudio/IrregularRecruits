using Data;
using UnityEngine;

namespace GameLogic
{
    //should be called first of everything to make sure everything is reset and initialised correctly
    public class GameControl : MonoBehaviour
    {
        private void Awake()
        {
            DeckGeneration.UniquesGenerated.Clear();
            Event.ResetEvents();

            Load();

        }


        public void Load()
        {
            DataHandler.Instance.PersistantDataObject.OnDatabaseLoaded -= ImportData;

            DataHandler.Instance.PersistantDataObject.OnDatabaseLoaded += ImportData;

            DataHandler.Instance.InitializeDatabases();

            //if (UnlockProgresses.Select(u => u.UnlocksHero.name).Distinct().Count() != UnlockProgresses.Count())
            //    Debug.LogError("Several unlock conditions for the same hero!");

        }


        private void ImportData()
        {
            StartCoroutine(LegacySystem.Instance.ImportRoutine());
        }
    }
}