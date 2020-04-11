using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[Range(0.1f,3f)]
	public float CombatSpeed = 1f;

	public enum DeckDamage 
	{ 
		DamageToTopCard,
		DoubleDamageToTopCard,
		AnyDamageKillsTopCard
	}

	//using same size for enemies and player right now
	internal static int StartingHandSize(bool enemy) => enemy ? Instance.PlayerStartingHandSize : Instance.PlayerStartingHandSize;

	public static int DeckSize(bool player) => player ? Instance.PlayerDeckSize : Instance.EnemyDeckSize;

	private void Awake()
	{
		//Set up event hierachy
		Event.OnPlay.AddListener(c => Event.OnPlayerAction.Invoke());
	}
}
