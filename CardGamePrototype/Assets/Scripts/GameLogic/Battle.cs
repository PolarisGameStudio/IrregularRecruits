using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class Battle
    {
        public static Deck PlayerDeck { get; private set; }
        public static Deck EnemyDeck;

        public static int Turn = 0;
        private const int MaxTurns = 100;

        private int PlayerCr, EnemyCr;
        private List<Card> PlayerStartingDeck, EnmStartDeck;

        private CombatAutoResolver CombatAutoResolver;

        public Battle(Deck playerDeck, Deck enemyDeck)
        {
            if (playerDeck.DeckController == null)
                playerDeck.DeckController = GameSettings.Instance.AiControlledPlayer ? new AI(playerDeck) : (DeckController)new PlayerController(playerDeck);
            if (enemyDeck.DeckController == null)
                enemyDeck.DeckController = new AI(enemyDeck);

            PlayerStartingDeck = playerDeck.AllCreatures();
            EnmStartDeck = enemyDeck.AllCreatures();
            PlayerCr = playerDeck.CR;
            EnemyCr = enemyDeck.CR;

            //TODO: These are added too many times..
            Event.OnPlayerAction.AddListener(d => AbilityWithEffect.AbilityStackCount = 0);
            Event.OnBattleFinished.AddListener(PackUp);

            AbilityWithEffect.AbilityStackCount = 0;

            CombatAutoResolver = new CombatAutoResolver(NextTurn);

            CombatAutoResolver.StartCombat(playerDeck, enemyDeck);

            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;

            EnemyDeck.DeckController.SetupDeckActions(EnemyDeck, PlayerDeck.DeckController.YourTurn);
            PlayerDeck.DeckController.SetupDeckActions(PlayerDeck, Event.OnCombatResolveStart.Invoke);

            //Debug.Log("starting battle. Enemies: " + enemyDeck.AllCreatures().Count + ", CR: " + enemyDeck.CR);
            Turn = 0;

            NextTurn();
        }


        internal static void CheckBattleOver()
        {
            if (BattleOver())
                HandleBattleOver();
        }

        public static bool BattleOver()
        {
            return EnemyDeck?.Alive() == 0 || PlayerDeck?.Alive() == 0 || Turn >= MaxTurns;
        }

        public static void HandleBattleOver()
        {
            if (PlayerDeck.Alive() > 0)
                Event.OnBattleFinished.Invoke(PlayerDeck, EnemyDeck);
            else
                Event.OnBattleFinished.Invoke(EnemyDeck, EnemyDeck);
        }

        public static void SetPlayerHero(Hero hero)
        {
            if(PlayerDeck!=null)
                hero.InDeck = PlayerDeck;

            Event.OnHeroSelect.Invoke(hero);
        }

        public static void SetPlayerDeck(DeckObject deckObject)
        {
            PlayerDeck = new Deck(deckObject);

            Event.OnDeckSizeChange.Invoke(PlayerDeck);
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

        public static Deck GetEnemyDeck(Deck myDeck)
        {
            if (myDeck == PlayerDeck) return EnemyDeck;
            else return PlayerDeck;
        }

        //TODO: this should be done by the player controller
        private void NextTurn()
        {
            Turn++;

            Event.OnTurnBegin.Invoke();

            //Could control which player goes  first
            if (EnemyDeck != null)
                EnemyDeck.DeckController.YourTurn();
        }


        public static List<Card> GetCardsInZone(Deck.Zone zone)
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