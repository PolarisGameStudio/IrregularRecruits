using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;
using UnityEngine.Events;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Simulation : Singleton<Simulation>
{
#if UNITY_EDITOR

    public List<DeckObject> DecksToSimulate;
    public int BattlesToSimulate;
    public List<MatchUpResult> MatchUpResults;
    public List<GameRunResult> GameWins;
    public List<Creature> Creatures;

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
        public int HighestRun ;
        public int LowestRun ;

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

            AverageRun = (float) Runs.Average();
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
            for (int j = i+1; j < Instance.DecksToSimulate.Count; j++)
            {
                if (i == j) continue;

                var d = Instance.DecksToSimulate[i];
                var d2 = Instance.DecksToSimulate[j];

                Instance.MatchUpResults.Add(Instance.Simulate(d,d2, Instance.BattlesToSimulate));

            }
        }
    }
    [MenuItem("Simulation/Combat decks against the game")]
    public static void DeckGameRunSimulation()
    {
        if (Instance.DecksToSimulate.Count < 1)
            Debug.LogError("no decks to simulate");

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

    private int SimulateRun(DeckObject playerDeck)
    {
        var gc = new GameControl(null, null, null);
        gc.PlayerDeck = new Deck(playerDeck);

        var wins = -1;
        
        var winner = gc.PlayerDeck;
        
        Event.OnBattleFinished.AddListener(d => winner = d);

        while (winner == gc.PlayerDeck && gc.PlayerDeck.Alive() > 0)
        {
            wins++;

            gc.NextCombat();
        }

        return wins;
    }

    private MatchUpResult Simulate(DeckObject first,DeckObject second,int times)
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

#endif
}
