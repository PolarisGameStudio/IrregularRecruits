using Data;
using System;
using System.Collections;
using UnityEngine;
using Event = GameLogic.Event;

[CreateAssetMenu]
public class GameSettings : SingletonScriptableObject<GameSettings>
{

    public IntType EnemyDeckSize = new IntType();
    //Player deck size
    public int PlayerDeckSize = 5;
    //Player starting hand size
    public int StartingHandSize = 3;
    public int PlaysPrTurn = 2;
    //player draw amount
    public int DrawPrTurn = 1;
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
        LowestAttackFirst,
        OrderedInTurns
    }
    //DamageToDeckSystem
    public DeckDamage DeckDamageParadigm = DeckDamage.DamageToTopCard;
    
    //Slower means faster
    public FloatType CombatSpeed ;

    public BoolType AiControlledPlayer ;

    public BoolType AutoEndTurn;

    public enum DeckDamage
    {
        DamageToTopCard,
        DoubleDamageToTopCard,
        AnyDamageKillsTopCard
    }

    public static int DeckSize() =>  Instance.EnemyDeckSize.Value;

    public void AiControlsPlayer(bool ai)
    {
        AiControlledPlayer.Value = ai;
    }

    public IEnumerator ImportRoutine()
    {
        yield return new WaitUntil(() => DataHandler.Instance.PersistantDataObject.databaseLoaded);

        const string table = "PlayerPrefs";

        CombatSpeed = DataHandler.Instance.GetData<FloatType>("Speed", table, 1f.ToString());
        AiControlledPlayer = DataHandler.Instance.GetData<BoolType>("AutoPlayer", table, "false");
        AutoEndTurn = DataHandler.Instance.GetData<BoolType>("AutoEndTurn", table, "true");

    }

    public static float Speed()
    {
        return Instance.CombatSpeed.Value;
    }
}
