using GameLogic;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Responsible for showing the player the result of a combat
    /// </summary>
    public class BattleSummary : Singleton<BattleSummary>
    {
        [Header("Battle summary")]
        //TODO: move to battlesummary class
        public GameObject BattleSummaryHolder;
        public UnitIcon BattleSummaryLostIcon;
        public UnitIcon BattleSummaryKilledIcon;
        public UnitIcon BattleSummaryGainedIcon;
        public Button HeroPortrait;
        public Hero Hero;
        public Button CloseButton;
        public UnityEvent OnClose = new UnityEvent();

        public XpBar XpBar;

        public TextMeshProUGUI GoldGainedText;

        private List<UnitIcon> InstantiatedObjects = new List<UnitIcon>();

        private void Awake()
        {
            BattleSummaryHolder.SetActive(false);

            HeroPortrait.onClick.AddListener(HeroClick);

            CloseButton.onClick.AddListener(OnClose.Invoke);
        }

        private void HeroClick()
        {
            if(Hero!=null)
                HeroView.Open(Hero);
        }

        public static void ShowSummary(List<Card> initialPlayerDeck, List<Card> initialEnemyDeck, List<Card> finalPlayerDeck, List<Card> finalEnemyDeck,int startXp, int endXp,Hero hero, int goldAtStartOfBattle,int endGold)
        {
            Instance.ShowBattleSummary(initialPlayerDeck, initialEnemyDeck, finalPlayerDeck, finalEnemyDeck,hero);

            var gained = endGold - goldAtStartOfBattle;

            Instance.GoldGainedText.text = gained + " gold looted";

            Instance.XpBar.ShowXpGain(startXp, endXp);
        }

        private void ShowBattleSummary(List<Card> initialPlayerDeck, List<Card> initialEnemyDeck, List<Card> finalPlayerDeck, List<Card> finalEnemyDeck,Hero hero)
        {
            foreach (var i in InstantiatedObjects)
                Destroy(i.gameObject);
            InstantiatedObjects.Clear();

            if (hero != null)
            {
                Hero = hero;
                HeroPortrait.image.sprite = hero.HeroObject.Portrait;
            }
            var killed = initialEnemyDeck.Where(c => !finalEnemyDeck.Contains(c) & !finalPlayerDeck.Contains(c));
            var lost = initialPlayerDeck.Where(c => !finalPlayerDeck.Contains(c));
            var gained = finalPlayerDeck.Where(c => !initialPlayerDeck.Contains(c));
            SetupIcons(killed, BattleSummaryKilledIcon);
            SetupIcons(lost, BattleSummaryLostIcon);
            SetupIcons(gained, BattleSummaryGainedIcon);


            BattleSummaryHolder.SetActive(true);
        }

        private void SetupIcons(IEnumerable<Card> killed, UnitIcon iconPrefab)
        {
            foreach (var c in killed)
            {
                var icon = Instantiate(iconPrefab, iconPrefab.transform.parent);

                icon.Setup(c);
                icon.gameObject.SetActive(true);

                InstantiatedObjects.Add(icon);
            }
        }
    }
}