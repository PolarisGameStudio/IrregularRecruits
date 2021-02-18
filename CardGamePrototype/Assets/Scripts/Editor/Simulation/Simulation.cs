using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;
using UnityEngine.Events;
using System.Linq;
using MapLogic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Simulation
{
    public class Simulation : Singleton<Simulation>
    {
#if UNITY_EDITOR

        public List<DeckObject> DecksToSimulate;
        public int BattlesToSimulate;
        public List<MatchUpResult> MatchUpResults;
        public List<GameRunResult> GameWins;
        public Creature[] Creatures;
        public Race[] AllRaces;
        private readonly int MaxRun = 50;

        [System.Serializable]
        public struct MatchUpResult
        {
            public string FirstDeck;
            public string SecondDeck;
            public int FirstDeckWins;
            public int SecondDeckWins;

            public MatchUpResult(string firstDeck, string secondDeck)
            {
                FirstDeck = firstDeck;
                SecondDeck = secondDeck;
                FirstDeckWins = 0;
                SecondDeckWins = 0;
            }

            public void AddWin(Deck d)
            {
                if (d.DeckObject.name == FirstDeck)
                    FirstDeckWins++;
                else if (d.DeckObject.name == SecondDeck)
                    SecondDeckWins++;
            }
        }

        [System.Serializable]
        public struct GameRunResult
        {
            public string Deck;
            public List<int> Runs;
            public float AverageRun;
            public int HighestRun;
            public int LowestRun;

            public GameRunResult(string deck) : this()
            {
                Deck = deck;
                Runs = new List<int>();
                AverageRun = 0;
                HighestRun = int.MinValue;
                LowestRun = int.MaxValue;
            }

            public void AddRun(int wins)
            {
                Runs.Add(wins);
                if (wins < LowestRun) LowestRun = wins;
                if (wins > HighestRun) HighestRun = wins;

                AverageRun = (float)Runs.Average();
            }
        }

        [MenuItem("Simulation/Combat against eachother simulation")]
        public static void DeckVSSimulation()
        {
            if (Instance.DecksToSimulate.Count < 2)
                Debug.LogError("no decks to simulate");

            var x = BattleManager.Instance;

            GameSettings.Instance.AiControlledPlayer = true;

            for (int i = 0; i < Instance.DecksToSimulate.Count; i++)
            {
                for (int j = i + 1; j < Instance.DecksToSimulate.Count; j++)
                {
                    if (i == j) continue;

                    var d = Instance.DecksToSimulate[i];
                    var d2 = Instance.DecksToSimulate[j];

                    Instance.MatchUpResults.Add(Instance.Simulate(d, d2, Instance.BattlesToSimulate));

                }
            }
        }
        [MenuItem("Simulation/Combat decks against the game")]
        public static void DeckGameRunSimulation()
        {
            if (Instance.DecksToSimulate.Count < 1)
                Debug.LogError("no decks to simulate. Are you using the correct scene?");

            var x = BattleManager.Instance;

            GameSettings.Instance.AiControlledPlayer = true;

            for (int i = 0; i < Instance.DecksToSimulate.Count; i++)
            {
                var d = Instance.DecksToSimulate[i];

                var result = new GameRunResult(d.name);

                for (int j = 0; j < Instance.BattlesToSimulate; j++)
                {
                    result.AddRun(Instance.SimulateRun(d));
                }
                Instance.GameWins.Add(result);
            }
        }

        [MenuItem("Simulation/Combat against game with shopping")]
        public static void DeckGameRunShopSimulation()
        {
            if (Instance.DecksToSimulate.Count < 1)
                Debug.LogError("no decks to simulate. Are you using the correct scene?");

            var x = BattleManager.Instance;

            GameSettings.Instance.AiControlledPlayer = true;

            for (int i = 0; i < Instance.DecksToSimulate.Count; i++)
            {
                var d = Instance.DecksToSimulate[i];

                var result = new GameRunResult(d.name);

                for (int j = 0; j < Instance.BattlesToSimulate; j++)
                {
                    result.AddRun(Instance.SimulateRunWithShop(d));
                }

                Instance.GameWins.Add(result);
            }
        }

        private int SimulateRun(DeckObject playerDeck)
        {
            //Event.ResetEvents();

            var gc = new PrototypeGameControl(null);
            gc.PlayerDeck = new Deck(playerDeck);

            var wins = -1;

            var winner = gc.PlayerDeck;

            Event.OnBattleFinished.AddListener(d => winner = d);

            while (wins <= MaxRun && winner == gc.PlayerDeck && gc.PlayerDeck.Alive() > 0)
            {
                wins++;

                gc.NextCombat();
            }

            Debug.Log($"final enemy Races:{gc.EnemyDeck.Races.Aggregate("", (current, next) => current.ToString() + " " + next.ToString())} CR: {gc.EnemyDeck.CR} ");

            gc.PlayerDeck.AllCreatures().ForEach(c => c.CleanListeners());

            gc.PlayerDeck.PackUp(true);


            return wins;
        }

        private int SimulateRunWithShop(DeckObject playerDeck)
        {
            //Event.ResetEvents();

            var settings = MapSettings.Instance;

            int CurrentDifficulty = settings.StartDifficulty;


            var gc = new PrototypeGameControl(null,null,settings.StepDifficultyIncrease,CurrentDifficulty);
            gc.PlayerDeck = new Deck(playerDeck);

            var wins = -1;

            var winner = gc.PlayerDeck;

            Event.OnBattleFinished.AddListener(d => winner = d);

            while (wins <= MaxRun && winner == gc.PlayerDeck && gc.PlayerDeck.Alive() > 0)
            {
                wins++;

                GoShopping(new Shop(null),gc.PlayerDeck);

                gc.NextCombat();
            }

            Debug.Log($"final enemy Races:{gc.EnemyDeck.Races.Aggregate("", (current, next) => current.ToString() + " " + next.ToString())} CR: {gc.EnemyDeck.CR} ");

            gc.PlayerDeck.AllCreatures().ForEach(c => c.CleanListeners());

            gc.PlayerDeck.PackUp(true);


            return wins;
        }

        private MatchUpResult Simulate(DeckObject first, DeckObject second, int times)
        {
            var result = new MatchUpResult(first.name, second.name);

            UnityAction<Deck> winnerhandler = (d) => result.AddWin(d);

            Event.OnBattleFinished.AddListener(winnerhandler);

            for (int i = 0; i < times; i++)
            {
                var pDeck = new Deck(first);
                var enmDeck = new Deck(second);

                Event.OnCombatSetup.Invoke(pDeck, enmDeck);
            }

            Debug.Log($"match up result: {result.FirstDeck} wins {result.FirstDeckWins}!  {result.SecondDeck} wins {result.SecondDeckWins}!");

            return result;
        }

        private static void GoShopping(Shop shop,Deck deck)
        {

            System.Tuple<ShopChoice, Creature> recommendation = ShopRecommendation.GetRecommendation(shop, deck, MapController.Instance.PlayerGold);

            while ( recommendation.Item1 != ShopChoice.NoAction)
            {
                if (recommendation.Item1 == ShopChoice.Reroll)
                    shop.Reroll();
                else 
                    shop.Buy(recommendation.Item2);

                recommendation = ShopRecommendation.GetRecommendation(shop, deck, MapController.Instance.PlayerGold);
            }
        }

#endif
    }
}