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

        private Dictionary<string, DataboxType> LoadedData = new Dictionary<string, DataboxType>();

        public T GetData<T>(string key, string table, string standardValue) where T : DataboxType
        {
            if (!LoadedData.ContainsKey(table + key))
                GetPersistantData<T>(key, table, standardValue);

            return (T)LoadedData[table + key];
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

        public void Save()
        {
            PersistantDataObject.SaveDatabase();
        }

        private void GetPersistantData<T>(string key, string table, string stdValue) where T: DataboxType
        {
            if (!LoadedData.ContainsKey(table + key))
            {
                if (!PersistantDataObject.databaseLoaded)
                {
                    Debug.LogError("Database not loaded");
                    return;
                }

                T data;

                if (!PersistantDataObject.TryGetData<T>(table, key, "Count", false, out data))
                {
                    data = (Activator.CreateInstance(typeof(T)) as T);

                    data.Convert(stdValue);

                    PersistantDataObject.AddData(table, key, "Count", data);

                    //Debug.Log("No data named " + key +" in table "+ table+", Creating entry");
                }

                LoadedData[table + key] = data;

                PersistantDataObject.SaveDatabase();
            }
        }

    }
}
