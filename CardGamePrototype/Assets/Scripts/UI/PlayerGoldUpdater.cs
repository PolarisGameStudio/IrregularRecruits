using MapLogic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerGoldUpdater : Singleton<PlayerGoldUpdater>
    {
        public float MinGoldCoinWait, MaxGoldCoinWait;

        private TextMeshProUGUI Text;
        private int MoneyValue;
        [Header("how much increase for each coin animation")]
        public int AnimationFactor = 3;
        public UnityEvent OnGoldGained = new UnityEvent();
        public ParticleSystem CoinParticlePrefab;
        public Transform GoldIconTransform;

        private Coroutine IncreaseRoutine;


        private void Start()
        {
            Text = GetComponent<TextMeshProUGUI>();

            MoneyValue = MapController.Instance.PlayerGold;

            Text.text = MoneyValue. ToString();

            MapController.Instance.OnPlayerGoldUpdate.AddListener(UpdateValue);
        }

        private void UpdateValue(int newamount)
        {

            if (IncreaseRoutine != null)
                StopCoroutine(IncreaseRoutine);

            if(newamount <= MoneyValue)
            {
                SetMoneyText(newamount);
            }
            else
            {
                IncreaseRoutine = StartCoroutine(IncreaseMoneyRoutine(MoneyValue, newamount));
            }

        }

        private void SetMoneyText(int newamount)
        {
            MoneyValue = newamount;
            Text.text = MoneyValue.ToString();
        }

        private IEnumerator IncreaseMoneyRoutine(int moneyValue, int newamount)
        {
            while(MoneyValue < newamount)
            {
                yield return new WaitForSeconds(Random.Range(MinGoldCoinWait, MaxGoldCoinWait));

                SetMoneyText(MoneyValue + 1);

                if (MoneyValue % AnimationFactor == 0)
                {
                    OnGoldGained.Invoke();

                    Instantiate(CoinParticlePrefab, (Vector2)GoldIconTransform.position, GoldIconTransform.rotation).transform.SetParent(GoldIconTransform);
                }
            }
        }
    }
}