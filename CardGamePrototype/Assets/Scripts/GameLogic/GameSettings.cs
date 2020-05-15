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



    public static int DeckSize(bool player) => player ? Instance.PlayerDeckSize : Instance.EnemyDeckSize;

}
