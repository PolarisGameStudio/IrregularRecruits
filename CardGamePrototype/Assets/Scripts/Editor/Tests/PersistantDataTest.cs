using Data;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PersistantDataTest : TestFixture
    {
        [UnityTest]
        public IEnumerator PersistantDataIsLoaded()
        {
            DataHandler.Instance.PersistantDataObject.LoadDatabase();

            yield return null;

            Assert.IsTrue(DataHandler.Instance.PersistantDataObject.databaseLoaded);

        }
        
        [UnityTest]
        public IEnumerator PlayerPrefsAreLoaded()
        {
            DataHandler.Instance.PlayerPrefsObject.LoadDatabase();

            yield return null;

            Assert.IsTrue(DataHandler.Instance.PlayerPrefsObject.databaseLoaded);

        }

        [UnityTest]
        public IEnumerator PersistantDataContainsData()
        {
            Databox.DataboxObject persistantDataObject = DataHandler.Instance.PersistantDataObject;

            persistantDataObject.LoadDatabase();

            yield return null;

            Assert.IsTrue(persistantDataObject.databaseLoaded);

            const string tableName = DataHandler.LegacyTableName;

            const string Key = "TestHero";

            IntType data;

            Assert.IsTrue(persistantDataObject.TryGetData<IntType>(tableName, Key, "Count", false, out data));

            Assert.NotNull(data);
        }


        [Test]
        public void LegacyChangeIsStored()
        {

            const string Key = "TestHero";
            var referenceValue = DataHandler.Instance.GetLegacy(Key);

            Assert.NotNull(referenceValue);

            int originalValue = referenceValue.Value;
            int change = 15;

            referenceValue.Value += change;

            var reference2 = DataHandler.Instance.GetLegacy(Key);

            Assert.AreEqual(originalValue + change, reference2.Value);

        }

    }
}
