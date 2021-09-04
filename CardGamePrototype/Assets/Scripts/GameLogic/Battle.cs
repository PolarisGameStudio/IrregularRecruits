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
                playerDeck.DeckController = GameSettings.Instance.AiControlledPlayer.Value ? new DeckAI(playerDeck,false) : (DeckController)new PlayerController(playerDeck);
            if (enemyDeck.DeckController == null)
                enemyDeck.DeckController = new DeckAI(enemyDeck,true);

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
            
            Turn = 0;

            Event.OnCombatStart.Invoke();

            EnemyDeck.DeckController.SetupDeckActions(EnemyDeck, PlayersTurn);
            PlayerDeck.DeckController.SetupDeckActions(PlayerDeck, CombatAutoResolver.ResolveCombat);

            //Debug.Log("starting battle. Enemies: " + enemyDeck.AllCreatures().Count + ", CR: " + enemyDeck.CR);
            

            NextTurn();
        }

        //TODO: should aslo handle the things we don't want ai to handle. Reset actions and Draws
        private void PlayersTurn()
        {
            PlayerDeck.DeckController.YourTurn();

            Event.OnPlayersTurnBegin.Invoke();

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
            if (PlayerDeck != null)
            { 
                hero.InDeck = PlayerDeck;
                PlayerDeck.Hero = hero;
            }

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
                Event.OnPlayerGoldAdd.Invoke((int)(15 + EnemyDeck.GetXpValue() ));
            }

            PlayerDeck?.PackUp();
            EnemyDeck?.PackUp(true);

            //EnemyDeck = null;

        }

        public static Deck GetEnemyDeck(Deck myDeck)
        {
            if (myDeck == PlayerDeck) return EnemyDeck;
            else return PlayerDeck;
        }

        private void NextTurn()
        {
            Turn++;

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