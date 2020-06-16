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

        public PrototypeGameControl(Creature testCreature, Race[] races, Creature[] enmCreatures,HeroObject testHero = null)
        {
            TestCreature = testCreature;
            TestHero = testHero;
            SpawnableRaces = races; 
            EnemyCreatures = enmCreatures;
            
            if (EnemyCreatures == null|| EnemyCreatures.Length == 0 )
                EnemyCreatures= Resources.FindObjectsOfTypeAll<Creature>();
            if (SpawnableRaces == null || SpawnableRaces.Length == 0)
                SpawnableRaces = Resources.FindObjectsOfTypeAll<Race>();

            if(TestCreature)
                PlayerDeck = GenerateDeck(0,true,testCreature,testHero);

        }

        public void NextCombat()
        {
            CurrentCombatDifficulty += CombatDifficultyIncrease;

            //if (EnemyDeck == null || EnemyDeck.Alive() == 0)
            EnemyDeck = GenerateDeck(CurrentCombatDifficulty);

            if(GameSettings.Instance.EnemyDeckSize < MaxEnemyDeckSize)
                GameSettings.Instance.EnemyDeckSize ++;

            Event.OnCombatSetup.Invoke(PlayerDeck, EnemyDeck);
        }
    }
}