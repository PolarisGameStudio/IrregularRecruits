using System.Collections.Generic;
using System.Linq;


namespace GameLogic
{
    //responsible for autoresolving combat rounds
    public class CombatAutoResolver
    {
        private Deck PlayerDeck, EnemyDeck;
        private List<Card> attackOrder;
        public int Turn = 0;

        private const int MaxTurns = 100;

        public CombatAutoResolver()
        {
            Event.OnCombatResolveStart.AddListener(ResolveCombat);
        }

        //TODO: should be removed and other
        public void StartCombat(Deck playerDeck, Deck enemyDeck)
        {
            Turn = 0;
            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;
        }

        //TODO: make static and parse the decks
        private void ResolveCombat()
        {
            if (PlayerDeck == null || EnemyDeck == null)
                return;

            Turn++;

            attackOrder = new List<Card>();

            attackOrder.AddRange(EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));
            attackOrder.AddRange(PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Where(c => c.CanAttack()));

            //Differentiate between player and enemy creatures?
            switch (GameSettings.Instance.AttackOrderParadigm)
            {
                case GameSettings.AttackParadigm.OrderedInTurns:
                    var order = new List<Card>();

                    bool enm = EnemyDeck.CreaturesInZone(Deck.Zone.Battlefield).Count >= PlayerDeck.CreaturesInZone(Deck.Zone.Battlefield).Count;

                    while (attackOrder.Any())
                    {
                        Card select;

                        if (attackOrder.Any(o => (o.InDeck == EnemyDeck) == enm))
                            select = attackOrder.First(o => (o.InDeck == EnemyDeck) == enm);
                        else
                            select = attackOrder.First();

                        order.Add(select);

                        attackOrder.Remove(select);

                        enm = !enm;
                    }

                    attackOrder = order;                    
                    break;

                case GameSettings.AttackParadigm.Random:
                    attackOrder = attackOrder.OrderBy(x => UnityEngine.Random.value).ToList();
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

            while (attackOrder.Any(c => c.Alive()))
            {
                AbilityWithEffect.AbilityStackCount = 0;

                var attacker = attackOrder.First(c => c.Alive());

                var otherDeck = attacker.InDeck == PlayerDeck ? EnemyDeck : PlayerDeck;

                var target = otherDeck.GetAttackTarget();

                if (target == null || attacker.Location != Deck.Zone.Battlefield)
                {
                    attackOrder.Remove(attacker);
                    continue;
                }

                attacker.AttackCard(target);

                attackOrder.Remove(attacker);
            }

            AbilityWithEffect.AbilityStackCount = 0;

            FinishCombatRound();
        }


        private void FinishCombatRound()
        {
            Event.OnCombatResolveFinished.Invoke();

            if (EnemyDeck.Alive() == 0 || Turn >= MaxTurns)
                Event.OnBattleFinished.Invoke(PlayerDeck);
            else if (PlayerDeck.Alive() == 0)
                Event.OnBattleFinished.Invoke(EnemyDeck);
            else
                Event.OnTurnBegin.Invoke();
        }



    }
}