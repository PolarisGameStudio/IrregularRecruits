using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : Singleton<CombatManager>
{
    public int Turn = 0;
    public static int PlayerActionsLeft;
    public static Deck PlayerDeck;
    public static Deck EnemyDeck;

    //to keep track of losses and kills
    private List<Card> InitialPlayerDeck, InitialEnemyDeck;

    public float AttackAnimationDuration = 1f;
    public float AbilityTriggerDuration = 1f;

    public static bool AbilityTriggering;

    private void Start()
    {
        Event.OnAbilityTrigger.AddListener(c => StartCoroutine(WaitForTrigger()));

        Event.OnAttack.AddListener(c=> Debug.Log("Event: "+ c +  ": Attacking"));
        Event.OnBeingAttacked.AddListener(c=> Debug.Log("Event: "+ c +  ": Is Attacked"));
        Event.OnDamaged.AddListener(c=> Debug.Log("Event: "+ c +  ": Is Damaged"));
        Event.OnDeath.AddListener(c=> Debug.Log("Event: "+ c +  ": Is dead"));
        Event.OnHealed.AddListener(c=> Debug.Log("Event: "+ c +  ": Is healed"));
        Event.OnKill.AddListener(c=> Debug.Log("Event: "+ c +  ": Killed a minion"));
        Event.OnWithdraw.AddListener(c=> Debug.Log("Event: "+ c +  ": Withdrew"));
        Event.OnCombatFinished.AddListener(()=> Debug.Log("Event: Combat Finished"));
        Event.OnCombatRoundFinished.AddListener(()=> Debug.Log("Event: Combat Round Finished"));
        Event.OnCombatStart.AddListener(()=> Debug.Log("Event: Combat started"));
        Event.OnGameOver.AddListener(()=> Debug.Log("Event: Game Over"));
    }

    public static void StartCombat(Deck playerDeck, Deck opponentDeck)
    {
        Instance.BeginCombat(playerDeck, opponentDeck);
    }

    private void BeginCombat(Deck playerDeck, Deck opponentDeck)
    {
        Turn = 0;

        PlayerDeck = playerDeck;
        EnemyDeck = opponentDeck;

        InitialPlayerDeck = PlayerDeck.AllCreatures();
        InitialEnemyDeck = EnemyDeck.AllCreatures();

        PlayerDeck.DrawInitialHand();
        EnemyDeck.DrawInitialHand(true);

        Event.OnCombatStart.Invoke();

        StartCoroutine(NextTurn());
    }
    private void EndCombat()
    {
        Event.OnCombatFinished.Invoke();
        PlayerDeck.PackUp();
        EnemyDeck.PackUp();

        BattleUI.ShowSummary(InitialPlayerDeck, InitialEnemyDeck, PlayerDeck.AllCreatures(), EnemyDeck.AllCreatures());
    }

    private IEnumerator NextTurn()
    {
        //Enemy actions
        EnemyDeck.Draw(GameSettings.Instance.DrawPrTurn);
        EnemyDeck.AI?.MakeMoves();

        PlayerDeck.Draw(GameSettings.Instance.DrawPrTurn);

        PlayerActionsLeft = GameSettings.Instance.PlayerPlaysPrTurn;

        yield return new WaitUntil(() => PlayerActionsLeft <= 0 || PlayerDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0);

        StartCoroutine(ResolveCombat());
    }

    private IEnumerator WaitForTrigger()
    {
        AbilityTriggering = true;

        //Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(AbilityTriggerDuration);

        AbilityTriggering = false;
        //Time.timeScale = 1f;

    }

    //TODO: Should this be a routine to wait for animations?
    private IEnumerator ResolveCombat()
    {
        //Debug.Log("Resolving combat");

        var attackOrder = new List<Card>();

        attackOrder.AddRange(PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.Attack > 0));
        attackOrder.AddRange(EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.Attack > 0));

        switch (GameSettings.Instance.AttackOrderParadigm)
        {
            case GameSettings.AttackParadigm.Random:
                attackOrder.OrderBy(x => Random.value);
                break;
            case GameSettings.AttackParadigm.HighestHealthFirst:
                attackOrder.OrderByDescending(x => x.Creature.Health);
                break;
            case GameSettings.AttackParadigm.LowestHealthFirst:
                attackOrder.OrderBy(x => x.Creature.Health);
                break;
            case GameSettings.AttackParadigm.HighestAttackFirst:
                attackOrder.OrderByDescending(x => x.Creature.Attack);
                break;
            case GameSettings.AttackParadigm.LowestAttackFirst:
                attackOrder.OrderBy(x => x.Creature.Attack);
                break;
            default:
                break;
        };

         AttackAnimationDuration = 1f;

        //To prevent wrong attack positions, if card have not reaach battle field yet
        yield return new WaitForSeconds(AttackAnimationDuration);

        while (attackOrder.Any(c => c.Alive()))
        {
            var attacker = attackOrder.First(c => c.Alive());

            var player = attacker.InDeck.PlayerDeck;

            var target = player ? EnemyDeck.GetAttackTarget() : PlayerDeck.GetAttackTarget();

            if (!target)
            {
                attackOrder.Remove(attacker);
                break; 
            }

            yield return new WaitUntil(() => !AbilityTriggering);
            Event.OnAttack.Invoke(attacker);

            yield return new WaitUntil(() => !AbilityTriggering);
            Event.OnBeingAttacked.Invoke(attacker);

            StartCoroutine(AnimationSystem.AttackAnimation(attacker,target, AttackAnimationDuration));
            yield return new WaitForSeconds(AttackAnimationDuration);

            target.CurrentHealth -= attacker.Attack;

            yield return new WaitUntil(() => !AbilityTriggering);

            if (!attacker.Ranged())
                attacker.CurrentHealth -= target.Attack;

            yield return new WaitUntil(() => !AbilityTriggering);

            attackOrder.Remove(attacker);
        }

        //To allow for death creatures to change zone
        yield return new WaitForSeconds(AttackAnimationDuration);

        //Debug.Log("Combat round finished. Enemies left: "+ EnemyDeck.Alive());

        Event.OnCombatRoundFinished.Invoke();

        yield return new WaitUntil(() => !AbilityTriggering);

        if (EnemyDeck.Alive() == 0 || PlayerDeck.Alive() == 0)
            EndCombat();
        else
            StartCoroutine(NextTurn());
    }

    public static List<Card> GetCardsInZone(Deck.Zone zone)
    {
        List<Card> cardsInZone = new List<Card>();

        cardsInZone.AddRange(PlayerDeck?.CreaturesInZone(zone));
        cardsInZone.AddRange(EnemyDeck?.CreaturesInZone(zone));

        return cardsInZone;
    }


    public static void Pause()
    {
        Time.timeScale = 0;
    }

    public static void UnPause()
    {
        Time.timeScale = 1;
    }
}