using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSummary : Singleton<BattleSummary>
{
    [Header("Battle summary")]
    //TODO: move to battlesummary class
    public GameObject BattleSummaryHolder;
    public UnitIcon BattleSummaryLostIcon;
    public UnitIcon BattleSummaryKilledIcon;
    public UnitIcon BattleSummaryGainedIcon;
    private List<UnitIcon> InstantiatedObjects = new List<UnitIcon>();

    private void Start()
    {
        BattleSummaryHolder.SetActive(false);
    }

    public static void ShowSummary(List<Card> initialPlayerDeck, List<Card> initialEnemyDeck, List<Card> finalPlayerDeck, List<Card> finalEnemyDeck)
    {
        Instance.ShowBattleSummary(initialPlayerDeck, initialEnemyDeck, finalPlayerDeck, finalEnemyDeck);
    }

    private void ShowBattleSummary(List<Card> initialPlayerDeck, List<Card> initialEnemyDeck, List<Card> finalPlayerDeck, List<Card> finalEnemyDeck)
    {
        foreach (var i in InstantiatedObjects)
            Destroy(i.gameObject);
        InstantiatedObjects.Clear();

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
