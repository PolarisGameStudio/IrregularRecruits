using System.Collections.Generic;
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

        public void NextCombat(List<Creature> addCreatures = null)
        {
            CurrentCombatDifficulty += CombatDifficultyIncrease;

            //if (EnemyDeck == null || EnemyDeck.Alive() == 0)

            if (addCreatures.Count == 0) addCreatures = null;

            EnemyDeck = DeckGeneration.GenerateDeck(CurrentCombatDifficulty,null,addCreatures);

            if (PlayerDeck == null) PlayerDeck = Battle.PlayerDeck;

            if(GameSettings.Instance.EnemyDeckSize < MaxEnemyDeckSize)
                GameSettings.Instance.EnemyDeckSize = MaxEnemyDeckSize;

            new Battle(PlayerDeck, EnemyDeck);
        }
    }
}