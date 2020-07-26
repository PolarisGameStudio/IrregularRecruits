using GameLogic;
using UI;
using UnityEngine.UI;
using Event = GameLogic.Event;

public class CombatPrototype : Singleton<CombatPrototype>
{
    public Button NextCombatButton;
    private PrototypeGameControl GC;

    public Creature TestCreature;
    public HeroObject TestHero;

    void Start()
    {
        GC = new PrototypeGameControl(TestCreature,TestHero);

        NextCombatButton.onClick.AddListener(GC.NextCombat);
        NextCombatButton.onClick.AddListener(() => NextCombatButton.gameObject.SetActive(false));

        NextCombatButton.gameObject.SetActive(true);

        Event.OnGameBegin.AddListener(NextCombatButton.onClick.Invoke);

        BattleUI.OnBattleFinished.AddListener(() => NextCombatButton.gameObject.SetActive(true));
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

    internal static void SetPlayerHero(HeroObject heroObject)
    {
        Hero hero = new Hero(heroObject);
        Instance.GC.PlayerDeck.Hero = hero;
        hero.InDeck = Instance.GC.PlayerDeck;
    }
}
