using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{

    public class PrototypeGameControl 
    {
        public int CombatDifficultyIncrease = 50;
        public int CurrentCombatDifficulty;
        public int MaxEnemyDeckSize = 10;

        public Deck PlayerDeck;
        public Deck EnemyDeck;

        public PrototypeGameControl(Creature testCreature = null, HeroObject testHero = null, int combatDifficultyIncrease = 10, int startingDiffulty = 50)
        {
            if (testCreature)
                PlayerDeck = DeckGeneration.GenerateDeck(0, testCreature, testHero);

            CombatDifficultyIncrease = combatDifficultyIncrease;
            CurrentCombatDifficulty = startingDiffulty;
        }

        public void NextCombat()
        {
            CurrentCombatDifficulty += CombatDifficultyIncrease;

            //if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = DeckGeneration.GenerateDeck(CurrentCombatDifficulty);

            if (PlayerDeck == null) PlayerDeck = Battle.PlayerDeck;

            if(GameSettings.Instance.EnemyDeckSize < MaxEnemyDeckSize)
                GameSettings.Instance.EnemyDeckSize ++;

            new Battle(PlayerDeck, EnemyDeck);
        }
    }
}