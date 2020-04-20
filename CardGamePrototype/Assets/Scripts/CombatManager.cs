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
    public static bool CombatRunning;

    public Trait AvantgardeTrait;
    public Trait EtherealTrait;

    //to keep track of losses and kills
    private List<Card> InitialPlayerDeck, InitialEnemyDeck;

    public float AttackAnimationDuration = 1f;
    public float AbilityTriggerDuration = 1f;

    public static bool AbilityTriggering;

    private void Start()
    {
        Event.OnAbilityTrigger.AddListener((x,y,z) => StartCoroutine(WaitForTrigger()));

        Event.OnCombatFinished.AddListener(EndCombat);

        Event.OnPlayerAction.AddListener(() => PlayerActionsLeft--);

        Event.OnDeath.AddListener(c => c.BattleRepresentation?.CardAnimation.Dissolve());
        Event.OnRessurrect.AddListener(c => c.BattleRepresentation?.CardAnimation.UnDissolve());

        //TODO: this delay should be handled by the flow controller
        Event.OnDeath.AddListener(c=>c. ChangeLocation(Deck.Zone.Graveyard, 2f));
        Event.OnRessurrect.AddListener(c=>c. ChangeLocation(Deck.Zone.Battlefield,2f));

        Event.OnPlay.AddListener( c=>c. ChangeLocation(Deck.Zone.Hand, Deck.Zone.Battlefield));

        Event.OnWithdraw.AddListener(c=>c.ChangeLocation(Deck.Zone.Battlefield, Deck.Zone.Library));

        Event.OnDraw.AddListener(c => c.ChangeLocation(Deck.Zone.Library, Deck.Zone.Hand));
    }

    public static void StartCombat(Deck playerDeck, Deck opponentDeck)
    {
        Instance.BeginCombat(playerDeck, opponentDeck);
    }

    private void BeginCombat(Deck playerDeck, Deck opponentDeck)
    {
        CombatRunning = true;

        Turn = 0;

        PlayerDeck = playerDeck;
        EnemyDeck = opponentDeck;

        SetupUI(playerDeck);
        SetupUI(EnemyDeck);

        InitialPlayerDeck = PlayerDeck.AllCreatures();
        InitialEnemyDeck = EnemyDeck.AllCreatures();

        PlayerDeck.DrawInitialHand();
        EnemyDeck.DrawInitialHand(true);


        FlowController.AddEvent(() =>    Event.OnCombatStart.Invoke());

        StartCoroutine(NextTurn());
    }

    private void SetupUI(Deck deck)
    {
        foreach (var card in deck.AllCreatures())
        {
            if (card.BattleRepresentation)
                continue;

            var ui = Instantiate<CardUI>(BattleUI.Instance.CardPrefab);

            ui.Card = card;
            card.BattleRepresentation = ui;

            card.ChangeLocation(Deck.Zone.Library);

            BattleUI.Move(card, card.Location, deck.PlayerDeck);
        }
    }

    private void EndCombat()
    {
        CombatRunning = false;

        PlayerDeck.PackUp();
        EnemyDeck.PackUp();

        BattleSummary.ShowSummary(InitialPlayerDeck, InitialEnemyDeck, PlayerDeck.AllCreatures(), EnemyDeck.AllCreatures());
    }

    private IEnumerator NextTurn()
    {
        Debug.Log("Turn: " + Turn + " Started");

        yield return new WaitUntil(() => FlowController.ReadyForNextAction);

        //Enemy actions
        EnemyDeck.Draw(GameSettings.Instance.DrawPrTurn);

        yield return new WaitUntil(() => FlowController.ReadyForNextAction);
        EnemyDeck.AI?.MakeMoves();

        yield return new WaitUntil(() => FlowController.ReadyForNextAction);
        PlayerDeck.Draw(GameSettings.Instance.DrawPrTurn);

        FlowController.AddEvent(() => 
            Event.OnTurnBegin.Invoke());

        PlayerActionsLeft = GameSettings.Instance.PlayerPlaysPrTurn;


        yield return new WaitUntil(() => FlowController.ReadyForNextAction && (PlayerActionsLeft <= 0 || PlayerDeck.CreaturesInZone(Deck.Zone.Hand).Count == 0));

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

        attackOrder.AddRange(PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));
        attackOrder.AddRange(EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));

        //Differentiate between player and enemy creatures?
        switch (GameSettings.Instance.AttackOrderParadigm)
        {
            case GameSettings.AttackParadigm.Random:
                attackOrder = attackOrder.OrderBy(x => Random.value).ToList();
                break;
            case GameSettings.AttackParadigm.HighestHealthFirst:
                attackOrder = attackOrder.OrderByDescending(x => x.Creature.Health).ToList();
                break;
            case GameSettings.AttackParadigm.LowestHealthFirst:
                attackOrder = attackOrder.OrderBy(x => x.Creature.Health).ToList();
                break;
            case GameSettings.AttackParadigm.HighestAttackFirst:
                attackOrder = attackOrder.OrderByDescending(x => x.Creature.Attack).ToList();
                break;
            case GameSettings.AttackParadigm.LowestAttackFirst:
                attackOrder = attackOrder.OrderBy(x => x.Creature.Attack).ToList();
                break;
            default:
                break;
        };

         AttackAnimationDuration = 1f;

        //To prevent wrong attack positions, if card have not reaach battle field yet
        yield return new WaitForSeconds(AttackAnimationDuration);

        while (attackOrder.Any(c => c.Alive()))
        {
            yield return new WaitUntil(()=>FlowController.ReadyForNextAction);

            var attacker = attackOrder.First(c => c.Alive());

            var player = attacker.InDeck.PlayerDeck;

            var target = player ? EnemyDeck.GetAttackTarget() : PlayerDeck.GetAttackTarget();

            if (target == null ||attacker.Location != Deck.Zone.Battlefield)
            {
                attackOrder.Remove(attacker);
                break; 
            }

            //yield return new WaitUntil(() => !AbilityTriggering);
            FlowController.AddEvent(()=>
                Event.OnAttack.Invoke(attacker));

            //yield return new WaitUntil(() => !AbilityTriggering);
            FlowController.AddEvent(() =>
                Event.OnBeingAttacked.Invoke(attacker));

            StartCoroutine(AnimationSystem.AttackAnimation(attacker,target, AttackAnimationDuration));
            yield return new WaitForSeconds(AttackAnimationDuration);

            target.Damage(attacker.Attack);

            //yield return new WaitUntil(() => !AbilityTriggering);

            //TODO: test that units that die still get to damage
            if (!attacker.Ranged() && target.Location == Deck.Zone.Battlefield)
                attacker.Damage(target.Attack);

            //yield return new WaitUntil(() => !AbilityTriggering);

            attackOrder.Remove(attacker);
        }

        //To allow for death creatures to change zone
        yield return new WaitForSeconds(AttackAnimationDuration);

        //Debug.Log("Combat round finished. Enemies left: "+ EnemyDeck.Alive());

        FlowController.AddEvent(() =>
            Event.OnCombatRoundFinished.Invoke());

        //TODO: just wait on eventcontroller
        yield return new WaitUntil(() => !AbilityTriggering);

        if (EnemyDeck.Alive() == 0 || PlayerDeck.Alive() == 0)
            FlowController.AddEvent(() => Event.OnCombatFinished.Invoke());
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