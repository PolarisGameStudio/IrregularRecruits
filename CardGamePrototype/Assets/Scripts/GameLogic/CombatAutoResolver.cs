using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameLogic
{
    //responsible for autoresolving combat rounds
    public class CombatAutoResolver
    {
        private Deck PlayerDeck, EnemyDeck;
        private List<Card> attackOrder;

        public CombatAutoResolver()
        {
            Event.OnCombatResolveStart.AddListener(ResolveCombat);
        }

        //TODO: should be removed and other
        public void StartCombat(Deck playerDeck, Deck enemyDeck)
        {
            PlayerDeck = playerDeck;
            EnemyDeck = enemyDeck;
        }

        //
        private void ResolveCombat()
        {
            if (PlayerDeck == null || EnemyDeck == null)
                return;


            attackOrder = new List<Card>();

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

            while (attackOrder.Any(c => c.Alive()))
            {
                var attacker = attackOrder.First(c => c.Alive());

                var otherDeck = attacker.InDeck == PlayerDeck ? EnemyDeck : PlayerDeck;

                var target = otherDeck.GetAttackTarget();

                if (target == null || attacker.Location != Deck.Zone.Battlefield)
                {
                    attackOrder.Remove(attacker);
                    continue;
                }

                Event.OnAttack.Invoke(attacker);

                Event.OnBeingAttacked.Invoke(target);

                target.Damage(attacker.Attack);

                if (!attacker.Ranged() && target.Location == Deck.Zone.Battlefield)
                    attacker.Damage(target.Attack);

                attackOrder.Remove(attacker);
            }

            FinishCombatRound();
        }


        private void FinishCombatRound()
        {
            Event.OnCombatResolveFinished.Invoke();

            if (EnemyDeck.Alive() == 0 || PlayerDeck.Alive() == 0)
                Event.OnBattleFinished.Invoke();
            else
                Event.OnTurnBegin.Invoke();
        }



    }
}