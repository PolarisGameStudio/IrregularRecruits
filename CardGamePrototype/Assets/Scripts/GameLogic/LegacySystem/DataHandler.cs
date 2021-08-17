using UnityEngine;
using Databox;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;

namespace Data
{
    [CreateAssetMenu]
    public class DataHandler : SingletonScriptableObject<DataHandler>
    {
        public DataboxObject PersistantDataObject;
        //public DataboxObject PlayerPrefsObject;

        public const string LegacyTableName = "Unlocks";
        private Dictionary<string, IntType> LegacyData = new Dictionary<string, IntType>();

        private void GetPersistantData(string key)
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
                    data = new IntType(0);
                    PersistantDataObject.AddData(LegacyTableName, key, "Count", data);

                    Debug.Log("No data named " + key + ", Creating entry");
                }

                LegacyData[key] = data;

                PersistantDataObject.SaveDatabase();
            }
        }

        internal void InitializeDatabases()
        {
            var path = System.IO.Path.Combine(Application.persistentDataPath, "Data.json");

            if (!File.Exists(path))
            {
                var jsonString = Resources.Load<TextAsset>("Data");
                File.WriteAllText(path, jsonString.text);
            }
            PersistantDataObject.LoadDatabase();

            //PlayerPrefsObject.LoadDatabase();
        }

        public IntType GetData(string key)
        {
            if (!LegacyData.ContainsKey(key))
                GetPersistantData(key);

            return LegacyData[key];
        }

        internal void Save()
        {
            PersistantDataObject.SaveDatabase();
        }
    }
}
