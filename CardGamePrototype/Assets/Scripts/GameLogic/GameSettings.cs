using System;
using UnityEngine;
using Event = GameLogic.Event;

[CreateAssetMenu]
public class GameSettings : SingletonScriptableObject<GameSettings>
{

    public int EnemyDeckSize = 3;
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
        LowestAttackFirst
    }
    //DamageToDeckSystem
    public DeckDamage DeckDamageParadigm = DeckDamage.DamageToTopCard;
    
    public float CombatSpeed = 1f;

    public bool AiControlledPlayer = false;

    public enum DeckDamage
    {
        DamageToTopCard,
        DoubleDamageToTopCard,
        AnyDamageKillsTopCard
    }

    public static int DeckSize() =>  Instance.EnemyDeckSize;

}
