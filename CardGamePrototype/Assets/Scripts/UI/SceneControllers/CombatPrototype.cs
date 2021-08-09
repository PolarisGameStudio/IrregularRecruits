using GameLogic;
using MapLogic;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

public class CombatPrototype : Singleton<CombatPrototype>
{
    public Button NextCombatButton;
    private PrototypeGameControl GC;

    public bool ShopBetweenBattles;

    public Creature TestCreature;
    public HeroObject TestHero;

    public List<Creature> EnemyDeck;


    void Start()
    {
        GC = new PrototypeGameControl(TestCreature, TestHero);

        NextCombatButton.onClick.AddListener(()=> GC.NextCombat(EnemyDeck));
        NextCombatButton.onClick.AddListener(() => NextCombatButton.gameObject.SetActive(false));

        NextCombatButton.gameObject.SetActive(true);

        Event.OnGameBegin.AddListener(NextCombatButton.onClick.Invoke);

        BattleUI.OnBattleFinished.AddListener(AfterBattle);

        BattleSummary.Instance.CloseButton.onClick.AddListener(SetupShop);

    }

    private void AfterBattle()
    {

        NextCombatButton.gameObject.SetActive(true);
    }

    private void SetupShop()
    {
        if (ShopBetweenBattles)
        {
            var races = CreatureLibrary.Instance.AllRaces;
            new Shop(races[Random.Range(0, races.Length)]);
        }
    }

    internal float GetCombatDifficultyIncrease()
    {
        return GC.CombatDifficultyIncrease;
    }

    internal void SetCombatDifficultyIncrease(int i)
    {
        GC.CombatDifficultyIncrease = i;
    }

    internal static void SetPlayerHero(HeroObject heroObject)
    {
        Hero hero = new Hero(heroObject);
        Instance.GC.PlayerDeck.Hero = hero;
        hero.InDeck = Instance.GC.PlayerDeck;
    }
}
