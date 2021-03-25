using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{


    public class BattleManager
    {
        public Deck PlayerDeck { get; private set; }
        public Deck EnemyDeck;
        public int Turn = 0;
        private int PlayerCr, EnemyCr;
        private List<Card> PlayerStartingDeck, EnmStartDeck;


        private CombatAutoResolver CombatAutoResolver;


        private static BattleManager instance = null;

        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new BattleManager();
                return instance;
            }
        }

        private BattleManager()
        {
            //Event.ResetEvents();

            Event.OnCombatSetup.AddListener(SetupCombat);

            Event.OnTurnBegin.AddListener(NextTurn);

            Event.OnBattleFinished.AddListener(PackUp);

            Event.OnPlayerAction.AddListener(d => AbilityWithEffect.AbilityStackCount = 0);

            AbilityWithEffect.AbilityStackCount = 0;

            CombatAutoResolver = new CombatAutoResolver();

        }

        internal void CheckBattleOver()
        {
            if (CombatAutoResolver.BattleOver())
                CombatAutoResolver.HandleBattleOver();
        }

        public static void Init()
        {
            instance = new BattleManager();
        }

        public static void SetPlayerHero(HeroObject heroObject)
        {
            SetPlayerHero(new Hero(heroObject));
        }
        public static void SetPlayerHero(Hero hero)
        {
            Instance.PlayerDeck.Hero = hero;
            hero.InDeck = Instance.PlayerDeck;

            Event.OnHeroSelect.Invoke(hero);
        }

        public static void SetPlayerDeck(DeckObject deckObject)
        {
            Instance.PlayerDeck = new Deck(deckObject);

            Event.OnDeckSizeChange.Invoke(Instance.PlayerDeck);
        }

        public void PackUp(Deck winner,Deck loser)
        {
            bool playerWon = winner == PlayerDeck;


#if UNITY_EDITOR
            if (PlayerCr != 0 && EnemyCr != 0)
            {
                ReevaluateCardPerformance(PlayerStartingDeck, PlayerCr, EnemyCr, playerWon);
                ReevaluateCardPerformance(EnmStartDeck, EnemyCr, PlayerCr, !playerWon);
            }
#endif

            if (EnemyDeck != null && playerWon)
            {
                PlayerDeck?.Hero?.AwardXp(EnemyDeck.GetXpValue());
                Event.OnPlayerGoldAdd.Invoke((int)(EnemyDeck.GetXpValue() * Random.Range(2.5f, 4f)));
            }

            PlayerDeck?.PackUp();
            EnemyDeck?.PackUp(true);

            EnemyDeck = null;

        }

        public Deck GetEnemyDeck(Deck myDeck)
        {
            if (myDeck == PlayerDeck) return EnemyDeck;
            else return PlayerDeck;
        }

        private void SetupCombat(Deck playerDeck, Deck enemyDeck)
        {
            if (playerDeck.DeckController == null)
                playerDeck.DeckController = GameSettings.Instance.AiControlledPlayer ? new AI(playerDeck) : (DeckController)new PlayerController(playerDeck);
            if (enemyDeck.DeckController == null)
                enemyDeck.DeckController = new AI(enemyDeck);

            PlayerStartingDeck = playerDeck.AllCreatures();
            EnmStartDeck = enemyDeck.AllCreatures();
            PlayerCr = playerDeck.CR;
            EnemyCr = enemyDeck.CR;

            CombatAutoResolver.StartCombat(playerDeck, enemyDeck);

            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;

            EnemyDeck.DeckController.SetupDeckActions(EnemyDeck, PlayerDeck.DeckController.YourTurn);
            PlayerDeck.DeckController.SetupDeckActions(PlayerDeck, Event.OnCombatResolveStart.Invoke);

            //Debug.Log("starting battle. Enemies: " + enemyDeck.AllCreatures().Count + ", CR: " + enemyDeck.CR);

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

#if UNITY_EDITOR
        private void ReevaluateCardPerformance(List<Card> cards, float deckCR, float enmCr,bool won)
        {
            float value = 0f;

            if(won)
            {
                var relation = enmCr / deckCR;

                if (relation < 0.75f)
                    return;

                value = relation;
            }
            else
            {
                var relation = deckCR / enmCr;


                if (relation < 0.75f)
                    return;

                value = -relation;
            }
            foreach (var c in cards)
                c.Creature.Performance += value;

        }
#endif
    }
}