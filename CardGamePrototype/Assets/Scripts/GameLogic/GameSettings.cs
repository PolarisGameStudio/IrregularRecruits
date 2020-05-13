using System;
using Event = GameLogic.Event;

public class GameSettings
{
    private static GameSettings instance;

    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
                instance = new GameSettings();
            return instance;
        }
        private set => instance = value;
    }


    public int EnemyBattlefieldSize = 2;
    public int EnemyDeckSize = 1;
    //Player deck size
    public int PlayerDeckSize = 5;
    //Player starting hand size
    public int StartingHandSize = 3;
    public int PlaysPrTurn = 2;
    //player draw amount
    public int DrawPrTurn = 2;
    //Battlefield size
    public int MaxCreaturesOnBattlefield;
    //AttackOrderParadigm
    public AttackParadigm AttackOrderParadigm = AttackParadigm.Random;
    public enum AttackParadigm
    {
        Random,
        HighestHealthFirst,
        LowestHealthFirst,
        HighestAttackFirst,
        LowestAttackFirst
    }
    //DamageToDeckSystem
    public DeckDamage DeckDamageParadigm = DeckDamage.DamageToTopCard;

    //make all rare creatures have abilities and all normal 
    public int MaxRareEnemiesPrCombat = 1;

    public float CombatSpeed = 1f;

    public bool AiControlledPlayer = false;

    public enum DeckDamage
    {
        DamageToTopCard,
        DoubleDamageToTopCard,
        AnyDamageKillsTopCard
    }


    //TODO: move the settings to a gamesettings Behaviour
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
        StartingHandSize = (int)val;
    }
    public void SetDrawsPrTurn(float val)
    {
        DrawPrTurn = (int)val;
    }
    public void SetPlayerActionsPrTurn(float val)
    {
        PlaysPrTurn = (int)val;
    }
    public void SetEnemyStartCreatures(float val)
    {
        EnemyBattlefieldSize = (int)val;
    }
    public void SetEnemyDeckSize(float val)
    {
        EnemyDeckSize = (int)val;
    }
    public void SetRareEnemiesPrBattle(float val)
    {
        MaxRareEnemiesPrCombat = (int)val;
    }


    public static int DeckSize(bool player) => player ? Instance.PlayerDeckSize : Instance.EnemyDeckSize;

}
