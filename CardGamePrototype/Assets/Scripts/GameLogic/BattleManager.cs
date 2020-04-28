namespace GameLogic
{
    //responsible for keeping track of deck currently in combat and setting up actions 
    public class BattleManager
    {
        public Deck PlayerDeck;
        public Deck EnemyDeck;

        public BattleManager()
        {
            Event.OnCombatSetup.AddListener(SetupCombat);

            Event.OnTurnBegin.AddListener(NextTurn);
        }

        private void SetupCombat(Deck playerDeck, Deck enemyDeck)
        {
            if (playerDeck.DeckController == null)
                playerDeck.DeckController = new AI();
            if (enemyDeck.DeckController == null)
                enemyDeck.DeckController = new AI();

            CombatAutoResolver combatAutoResolver = new CombatAutoResolver();

            combatAutoResolver.StartCombat(playerDeck, enemyDeck);

            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;

            EnemyDeck.DeckController.SetupDeckActions(EnemyDeck, PlayerDeck.DeckController.YourTurn);
            PlayerDeck.DeckController.SetupDeckActions(PlayerDeck, Event.OnCombatResolveStart.Invoke);

            Event.OnTurnBegin.Invoke();
        }

        private void NextTurn()
        {
            //Could control which player goes  first
            if (EnemyDeck != null)
                EnemyDeck.DeckController.YourTurn();
        }
    }
}