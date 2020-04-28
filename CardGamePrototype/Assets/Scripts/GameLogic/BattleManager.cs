using System;
using System.Collections.Generic;

namespace GameLogic
{
    //responsible for keeping track of decks currently in combat and setting up actions 
    public class BattleManager
    {
        public Deck PlayerDeck;
        public Deck EnemyDeck;
        public int PlayerActionsLeft;
        public int Turn = 0;

        private static BattleManager instance;

        public static BattleManager Instance { get {
                if (instance == null)
                    instance = new BattleManager();
                return instance; 
            } 
            private set => instance = value; 
        }

        private BattleManager()
        {
            Event.OnCombatSetup.AddListener(SetupCombat);

            Event.OnTurnBegin.AddListener(NextTurn);

            Event.OnBattleFinished.AddListener(PackUp);

            Event.OnPlayerAction.AddListener(() => PlayerActionsLeft--);
        }

        private void PackUp()
        {
            PlayerDeck?.PackUp();
            EnemyDeck?.PackUp();
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


        public List<Card> GetCardsInZone(Deck.Zone zone)
        {
            List<Card> cardsInZone = new List<Card>();

            if (PlayerDeck != null)
                cardsInZone.AddRange(PlayerDeck.CreaturesInZone(zone));
            if (EnemyDeck != null)
                cardsInZone.AddRange(EnemyDeck.CreaturesInZone(zone));

            return cardsInZone;
        }
    }
}