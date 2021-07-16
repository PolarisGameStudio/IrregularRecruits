using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace GameLogic
{
    //responsible for autoresolving combat rounds
    public class CombatAutoResolver
    {
        private Deck PlayerDeck, EnemyDeck;
        private List<Card> AttackOrder;

        private UnityAction NextTurnAction;

        public CombatAutoResolver(UnityAction nextTurn)
        {
            Event.OnCombatResolveStart.AddListener(ResolveCombat);
            NextTurnAction = nextTurn;
        }

        //TODO: should be removed and other
        public void StartCombat(Deck playerDeck, Deck enemyDeck)
        {
            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;
        }

        //TODO: make static and parse the decks
        private void ResolveCombat()
        {
            if (PlayerDeck == null || EnemyDeck == null)
                return;


            AttackOrder = new List<Card>();

            AttackOrder.AddRange(EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));
            AttackOrder.AddRange(PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));

            foreach (var c in EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.Ward()))
                c.Warded = true;
            foreach (var c in PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.Ward()))
                c.Warded = true;

            //Differentiate between player and enemy creatures?
            switch (GameSettings.Instance.AttackOrderParadigm)
            {
                case GameSettings.AttackParadigm.OrderedInTurns:
                    var order = new List<Card>();

                    bool enm = EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Count >= PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Count;

                    while (AttackOrder.Any())
                    {
                        Card select;

                        if (AttackOrder.Any(o => (o.InDeck == EnemyDeck) == enm))
                            select = AttackOrder.First(o => (o.InDeck == EnemyDeck) == enm);
                        else
                            select = AttackOrder.First();

                        order.Add(select);

                        AttackOrder.Remove(select);

                        enm = !enm;
                    }

                    AttackOrder = order;                    
                    break;

                case GameSettings.AttackParadigm.Random:
                    AttackOrder = AttackOrder.OrderBy(x => UnityEngine.Random.value).ToList();
                    break;
                case GameSettings.AttackParadigm.HighestHealthFirst:
                    AttackOrder = AttackOrder.OrderByDescending(x => x.Creature.Health).ToList();
                    break;
                case GameSettings.AttackParadigm.LowestHealthFirst:
                    AttackOrder = AttackOrder.OrderBy(x => x.Creature.Health).ToList();
                    break;
                case GameSettings.AttackParadigm.HighestAttackFirst:
                    AttackOrder = AttackOrder.OrderByDescending(x => x.Creature.Attack).ToList();
                    break;
                case GameSettings.AttackParadigm.LowestAttackFirst:
                    AttackOrder = AttackOrder.OrderBy(x => x.Creature.Attack).ToList();
                    break;
                default:
                    break;
            };


            foreach (var ferocioues in AttackOrder.Where(c => c.Ferocity()).ToArray())
            {
                var idx = AttackOrder.IndexOf(ferocioues);

                //inserts a copy in the order
                AttackOrder.Insert(idx, ferocioues);
            }

            while (AttackOrder.Any(c => c.Alive()))
            {
                AbilityWithEffect.AbilityStackCount = 0;

                var attacker = AttackOrder.First(c => c.Alive());

                var otherDeck = attacker.InDeck == PlayerDeck ? EnemyDeck : PlayerDeck;

                var target = otherDeck.GetAttackTarget(attacker.Assassin());

                if (target == null || attacker.Location != Deck.Zone.Battlefield)
                {
                    AttackOrder.Remove(attacker);
                    continue;
                }

                attacker.AttackCard(target);

                AttackOrder.Remove(attacker);
            }

            AbilityWithEffect.AbilityStackCount = 0;

            FinishCombatRound();
        }


        private void FinishCombatRound()
        {
            Event.OnCombatResolveFinished.Invoke();

            if (Battle. BattleOver())
            {
                Battle.HandleBattleOver();
            }
            else
                NextTurnAction.Invoke();
        }


    }
}