using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using MapLogic;

public class InputManager : Singleton<InputManager>
{
#if UNITY_EDITOR
    public enum MappableAction
    {
        Pause,
        WinBattle,
        LoseBattle,
        GainXp,
        GainGold,
    }

    [Serializable]
    public struct ActionKeyMapping
    {
        public KeyCode Key;
        public MappableAction Action;
    }

    public ActionKeyMapping[] KeyMappings;

    private void Update()
    {
        foreach (var k in KeyMappings)
            if (Input.GetKeyDown(k.Key))
                HandleAction(k.Action);

    }

    private void HandleAction(MappableAction action)
    {
        switch (action)
        {
            case MappableAction.Pause:
                Time.timeScale = Time.timeScale > 0 ? 0 : 1f;
                break;
            case MappableAction.WinBattle:
                if(BattleManager.Instance.EnemyDeck != null)
                    Event.OnBattleFinished.Invoke(BattleManager.Instance.PlayerDeck, BattleManager.Instance.EnemyDeck);
                break;
            case MappableAction.LoseBattle:
                if (BattleManager.Instance.EnemyDeck != null)
                    Event.OnBattleFinished.Invoke(BattleManager.Instance.EnemyDeck, BattleManager.Instance.PlayerDeck);
                break;
            case MappableAction.GainXp:
                BattleManager.Instance.PlayerDeck?.Hero?.AwardXp(10);
                break;
            case MappableAction.GainGold:
                MapController.Instance.PlayerGold += 20;
                break;
            default:
                break;
        }
    }


#endif
}

