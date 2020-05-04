using System;
using UnityEngine;
using Event = GameLogic.Event;

public class GameSettings : Singleton<GameSettings>
{
    [Serializable]
    public class SettingValue
    {
        public int Minimum;
        public int Maximum;
    }

    public int EnemyBattlefieldSize;
    public int EnemyDeckSize;
    public int EnemyPlaysPrTurn;
    //Player deck size
    public int PlayerDeckSize;
    //Player starting hand size
    public int PlayerStartingHandSize;
    public int PlayerPlaysPrTurn;
    //player draw amount
    public int DrawPrTurn;
    //Battlefield size
    public int MaxCreaturesOnBattlefield;
    //AttackOrderParadigm
    public AttackParadigm AttackOrderParadigm;
    public enum AttackParadigm
    {
        Random,
        HighestHealthFirst,
        LowestHealthFirst,
        HighestAttackFirst,
        LowestAttackFirst
    }
    //DamageToDeckSystem
    public DeckDamage DeckDamageParadigm;

    [Range(0, 10)]
    //make all rare creatures have abilities and all normal 
    public int MaxRareEnemiesPrCombat = 1;

    [Range(0.1f, 3f)]
    public float CombatSpeed = 1f;

    public bool AiControlledPlayer;

    public enum DeckDamage
    {
        DamageToTopCard,
        DoubleDamageToTopCard,
        AnyDamageKillsTopCard
    }

    public void AiControlsPlayer(bool ai)
    {
        AiControlledPlayer = ai;
    }
    public void SetCombatSpeed(float f)
    {
        CombatSpeed = f;
    }

    public void SetStartingHandSize(float val)
    {
        PlayerStartingHandSize = (int)val;
    }
    public void SetDrawsPrTurn(float val)
    {
        DrawPrTurn = (int)val;
    }
    public void SetPlayerActionsPrTurn(float val)
    {
        PlayerPlaysPrTurn = (int)val;
    }
    public void SetEnemyStartCreatures(float val)
    {
        EnemyBattlefieldSize = (int)val;
    }
    public void SetEnemyDeckSize(float val)
    {
        EnemyDeckSize = (int)val;
    }
    public void SetEnemyPlaysPrTurn(float val)
    {
        EnemyPlaysPrTurn = (int)val;
    }
    public void SetRareEnemiesPrBattle(float val)
    {
        MaxRareEnemiesPrCombat = (int)val;
    }

    //using same size for enemies and player right now
    internal static int StartingHandSize(bool enemy) => enemy ? Instance.PlayerStartingHandSize : Instance.PlayerStartingHandSize;

    public static int DeckSize(bool player) => player ? Instance.PlayerDeckSize : Instance.EnemyDeckSize;

}
