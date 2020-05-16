using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = GameLogic.Event;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Simulation : Singleton<Simulation>
{
#if UNITY_EDITOR

    public List<DeckObject> DecksToSimulate;
    public int BattlesToSimulate;
    public List<MatchUpResult> MatchUpResults;

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

    private MatchUpResult Simulate(DeckObject first,DeckObject second,int times)
    {
        var result = new MatchUpResult(first.name, second.name);

        UnityAction<Deck> winnerhandler = (d) => result.AddWin(d);

        Event.OnBattleFinished.AddListener(winnerhandler);

        for (int i = 0; i < times; i++)
        {
            var pDeck = new Deck(first, false);
            var enmDeck = new Deck(second, false);

            Event.OnCombatSetup.Invoke(pDeck, enmDeck);
        }

        Debug.Log($"match up result: {result.FirstDeck} wins {result.FirstDeckWins}!  {result.SecondDeck} wins {result.SecondDeckWins}!");

        return result;
    }

#endif
}
