using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace GameLogic
{
    [CreateAssetMenu]
    public partial class Ability : ScriptableObject
    {
        [SerializeField]
        private float Value;
        public enum Verb
        {
            ATTACKS,
            IsATTACKED,
            KILLS,
            DIES,
            ETB,
            IsDAMAGED,
            IsHealed,
            Draw,
            Withdraw,
            RoundEnd,
            COUNT
        }
        public enum ActionType
        {
            Kill,
            DealDamage,
            StatPlus,
            StatMinus,
            Withdraw,
            //Discard, use kill in hand instead
            Heal,
            Resurrect,
            Draw,
            Charm,
            Summon,
            Clone,
            Copy,
            COUNT
        }
        public enum Count
        {
            All, One, Two, Three,
            COUNT
        }

        public Trigger TriggerCondition;
        public Action ResultingAction;

        public override string ToString()
        {
            return base.ToString();
        }
        public string Description(Creature owner)
        {
            return $"{TriggerCondition.Description(owner)}, {ResultingAction.Description(owner)}.";
        }

        public void SetupListeners(Card _owner)
        {
            AbilityProcessor.GetTrigger(TriggerCondition.TriggerAction).SetupListener(_owner, TriggerCondition.Subjekt, ExecuteIfTrue);
            //TODO: replace with CardEvent Reference
        }


        public void RemoveListeners(Card _owner)
        {
            AbilityProcessor.GetTrigger(TriggerCondition.TriggerAction).RemoveListener(_owner, TriggerCondition.Subjekt, ExecuteIfTrue);

        }


        public float GetValue()
        {
            Value = TriggerCondition.GetValue() * ResultingAction.GetValue();
            return Value;
        }


        private void ExecuteIfTrue(Card instigator, Card abilityOwner, Noun subject)
        {
            if (subject.CorrectNoun(instigator, abilityOwner))
                ExecuteAction(abilityOwner, instigator);
        }

        private void ExecuteAction(Card owner, Card triggerExecuter)
        {
            Debug.Log("Trigger: " + TriggerCondition.Description(owner.Creature) + " is true");
            Debug.Log("Executing: " + ResultingAction.Description(owner.Creature));
            owner.OnAbilityTrigger.Invoke();

            List<Card> targets = GetTargets(ResultingAction.Target, ResultingAction.TargetCount, Deck.Zone.Battlefield, owner, triggerExecuter);

            AbilityProcessor.GetAction(ResultingAction.ActionType).ExecuteAction(this, owner, targets);
        }

        public List<Card> GetTargets(Noun targetType, Count count, Deck.Zone location, Card _owner, Card triggerExecuter)
        {
            List<Card> cardsInZone = BattleManager.Instance.GetCardsInZone(targetType.Location);

            List<Card> cs = cardsInZone.Where(c =>
                targetType.CorrectCharacter(c, _owner, triggerExecuter) &&
                targetType.CorrectAllegiance(c, _owner) &&
                targetType.CorrectDamageState(c) &&
                targetType.CorrectRace(c, _owner)).ToList();

            return TakeCount(cs, ResultingAction.TargetCount);
        }

        private List<Card> TakeCount(List<Card> cards, Count count)
        {
            if (cards.Count == 0) return cards;

            switch (count)
            {
                case Count.All:
                    return cards;
                case Count.One:
                    return new List<Card>() { cards[Random.Range(0, cards.Count())] };
                case Count.Two:
                    cards.OrderBy(o => Random.value);
                    return cards.Take(2).ToList();
                case Count.Three:
                    cards.OrderBy(o => Random.value);
                    return cards.Take(2).ToList();
                default:
                    return cards;
            }
        }

    }
}