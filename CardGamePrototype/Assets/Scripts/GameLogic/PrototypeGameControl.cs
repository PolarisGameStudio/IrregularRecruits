using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    public partial class PrototypeGameControl : DeckGeneration
    {
        public Creature TestCreature;
        public HeroObject TestHero;
        public int CombatDifficultyIncrease = 50;
        public int CurrentCombatDifficulty;
        public int MaxEnemyDeckSize = 10;

        public Deck PlayerDeck;
        public Deck EnemyDeck;

        public PrototypeGameControl(Creature testCreature, HeroObject testHero = null)
        {
            TestCreature = testCreature;
            TestHero = testHero;

            if(TestCreature)
                PlayerDeck = GenerateDeck(0,testCreature,testHero);

        }

        public void NextCombat()
        {
            CurrentCombatDifficulty += CombatDifficultyIncrease;

            //if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = GenerateDeck(CurrentCombatDifficulty);

            if (PlayerDeck == null) PlayerDeck = BattleManager.Instance.PlayerDeck;

            if(GameSettings.Instance.EnemyDeckSize < MaxEnemyDeckSize)
                GameSettings.Instance.EnemyDeckSize ++;

            Event.OnCombatSetup.Invoke(PlayerDeck, EnemyDeck);
        }
    }
}