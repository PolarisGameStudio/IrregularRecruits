using Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Event = GameLogic.Event;

[CreateAssetMenu]
public class GameSettings : SingletonScriptableObject<GameSettings>
{

    public AudioMixer mixer;
    public int EnemyDeckSize = 50;
    //Player deck size
    public int PlayerDeckSize = 5;
    //Player starting hand size
    public int StartingHandSize = 3;
    public int PlaysPrTurn = 2;
    //player draw amount
    public int DrawPrTurn = 1;
    public int EnemyDrawsPrTurn = 2;
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
    public IntType Volume;

    public BoolType AiControlledPlayer ;

    public BoolType AutoEndTurn;

    public BoolType VibrateEnabled;

    public enum DeckDamage
    {
        DamageToTopCard,
        DoubleDamageToTopCard,
        AnyDamageKillsTopCard
    }

    public static int DeckSize() =>  Instance.EnemyDeckSize;

    public void AiControlsPlayer(bool ai)
    {
        AiControlledPlayer.Value = ai;
    }

    //todo: move to soundcontroller
    public void SetVolume(float arg0)
    {
        if (arg0 < -19f)
            arg0 = -80f;

        mixer.SetFloat("MasterVolume", arg0);
        Volume.Value = (int)arg0;
    }

    public void Vibration(bool enabled)
    {
        VibrateEnabled.Value = enabled;
    }

    public IEnumerator ImportRoutine()
    {
        yield return new WaitUntil(() => DataHandler.Instance.PersistantDataObject.databaseLoaded);

        const string table = "PlayerPrefs";

        CombatSpeed = DataHandler.Instance.GetData<FloatType>("Speed", table, 1f.ToString());
        AiControlledPlayer = DataHandler.Instance.GetData<BoolType>("AutoPlayer", table, "false");
        AutoEndTurn = DataHandler.Instance.GetData<BoolType>("AutoEndTurn", table, "true");
        VibrateEnabled = DataHandler.Instance.GetData<BoolType>("VibrationEnabled", table, "true");
        Volume = DataHandler.Instance.GetData<IntType>("MasterVolume", table, "0");

        SetVolume(Volume.Value);

    }

    public static float Speed()
    {
        return Instance.CombatSpeed.Value;
    }
}
