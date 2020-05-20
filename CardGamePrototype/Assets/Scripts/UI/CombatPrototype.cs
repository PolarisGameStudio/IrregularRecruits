using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;

public class CombatPrototype : Singleton<CombatPrototype>
{
    public Button NextCombatButton;
    private GameControl GC;

    public Creature TestCreature;
    public HeroObject TestHero;
    public Race[] AllRaces;
    public Creature[] AllCreatures;
    public Creature[] AllHeroes;

    void Start()
    {
        GC = new GameControl(TestCreature, AllRaces, AllCreatures,TestHero);

        NextCombatButton.onClick.AddListener(GC.NextCombat);
        NextCombatButton.onClick.AddListener(() => NextCombatButton.gameObject.SetActive(false));
        BattleUI.OnBattleFinished.AddListener(() => NextCombatButton.gameObject.SetActive(true));
    }


    public static void SetPlayerDeck(Deck deck)
    {
        Instance.GC.PlayerDeck = deck;
    }

    public void GetNewMinions()
    {
        AddMinionScreen.SetupMinionScreen(GC.PlayerDeck);
    }

    internal float GetCombatDifficultyIncrease()
    {
        return GC.CombatDifficultyIncrease;
    }

    internal void SetCombatDifficultyIncrease(int i)
    {
        GC.CombatDifficultyIncrease = i;
    }
}
