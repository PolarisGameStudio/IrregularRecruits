using UnityEngine;
using Databox;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Data
{
    [CreateAssetMenu]
    public class DataHandler : SingletonScriptableObject<DataHandler>
    {
        public DataboxObject PersistantDataObject;
        public DataboxObject PlayerPrefsObject;

        public const string LegacyTableName = "Unlocks";
        private Dictionary<string, IntType> LegacyData = new Dictionary<string, IntType>();

        private void CreateLegacyData(string key, int value)
        {
            if (!LegacyData.ContainsKey(key))
            {
                if (!PersistantDataObject.databaseLoaded)
                {
                    Debug.LogError("Database not loaded");
                    return;
                }

                IntType data;

                if (!PersistantDataObject.TryGetData<IntType>(LegacyTableName, key, "Count", false, out data))
                {
                    data = new IntType(value);
                    PersistantDataObject.AddData(LegacyTableName, key, "Count", data);
                }

                data.Value = value;

                LegacyData[key] = data;

                PersistantDataObject.SaveDatabase();
            }
        }

        internal void InitializeDatabases()
        {
            Debug.Log("loading databases");

            PersistantDataObject.LoadDatabase();

            PlayerPrefsObject.LoadDatabase();
        }

        public IntType GetLegacy(string key)
        {
            if (!LegacyData.ContainsKey(key))
                CreateLegacyData(key, 0);

            return LegacyData[key];
        }

    }
}
