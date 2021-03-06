using Data;
using MapLogic;
using UnityEngine;

namespace GameLogic
{
    //should be called first of everything to make sure everything is reset and initialised correctly
    public class GameControl : MonoBehaviour
    {
        private void Awake()
        {
            LeanTween.init(3200);

            DeckGeneration.UniquesGenerated.Clear();
            Event.ResetEvents();
            MapNode.ResetListiners();

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
            try
            {
                StartCoroutine(LegacySystem.Instance.ImportRoutine());
                StartCoroutine(GameSettings.Instance.ImportRoutine());
            }
            catch (System.Exception)
            {
            }
        }
    }
}