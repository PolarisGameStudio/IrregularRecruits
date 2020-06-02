using System;
using System.Collections.Generic;

namespace GameLogic
{
    //responsible for keeping track of decks currently in combat and setting up actions 
    public class BattleManager
    {
        public Deck PlayerDeck;
        public Deck EnemyDeck;
        public int Turn = 0;

        private CombatAutoResolver CombatAutoResolver;

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
            //throw new Exception("test exeeptino");

            Event.OnCombatSetup.AddListener(SetupCombat);

            Event.OnTurnBegin.AddListener(NextTurn);

            Event.OnBattleFinished.AddListener(PackUp);

            Event.OnPlayerAction.AddListener(d => Ability.AbilityStackCount = 0);

            CombatAutoResolver = new CombatAutoResolver();

        }

        public void PackUp(Deck d)
        {
            if(EnemyDeck!=null)
                PlayerDeck?.Hero?.AwardXp(EnemyDeck.GetXpValue());

            PlayerDeck?.PackUp();
            EnemyDeck?.PackUp(true);

            PlayerDeck = null;
            EnemyDeck = null;
        }

        public Deck GetEnemyDeck(Deck myDeck)
        {
            if (myDeck == PlayerDeck) return EnemyDeck;
            else return PlayerDeck;
        }

        private void SetupCombat(Deck playerDeck, Deck enemyDeck)
        {
            if (playerDeck.DeckController == null )
                playerDeck.DeckController = GameSettings.Instance.AiControlledPlayer ?(DeckController) new AI(playerDeck): (DeckController)new PlayerController(playerDeck);
            if (enemyDeck.DeckController == null)
                enemyDeck.DeckController = new AI(enemyDeck);

            CombatAutoResolver.StartCombat(playerDeck, enemyDeck);

            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;

            EnemyDeck.DeckController.SetupDeckActions(EnemyDeck, PlayerDeck.DeckController.YourTurn);
            PlayerDeck.DeckController.SetupDeckActions(PlayerDeck, Event.OnCombatResolveStart.Invoke);

            Event.OnTurnBegin.Invoke();
        }

        //TODO: this should be done by the player controller
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