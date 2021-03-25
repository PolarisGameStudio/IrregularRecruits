using UnityEngine;
using Databox;
using System.Collections.Generic;

namespace Data
{
    [CreateAssetMenu]
    public class DataHandler : SingletonScriptableObject<DataHandler>
    {
        public DataboxObject PersistantDataObject;
        public DataboxObject PlayerPrefsObject;

        private const string LegacyTableName = "Unlocks";
        private Dictionary<string, IntType> LegacyData = new Dictionary<string, IntType>();

        private void CreateLegacyData(string key, int value)
        {
            if (!LegacyData.ContainsKey(key))
            {
                if (!PersistantDataObject.databaseLoaded)
                    //TODO: consider async if this takes too long
                    PersistantDataObject.LoadDatabase();

                IntType data;

                if (!PersistantDataObject.TryGetData<IntType>(LegacyTableName, key, "Count", true, out data))
                {
                    data = new IntType(value);
                    PersistantDataObject.AddData(LegacyTableName, key, "Count", data);
                }

                data.Value = value;

                LegacyData[key] = data;
            }
        }

        public IntType GetLegacy(string key)
        {
            if (!LegacyData.ContainsKey(key))
                CreateLegacyData(key, 0);

            return LegacyData[key];
        }
    }
}
