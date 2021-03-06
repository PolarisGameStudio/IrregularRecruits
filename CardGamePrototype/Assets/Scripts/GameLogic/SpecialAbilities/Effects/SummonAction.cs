using System.Collections.Generic;

namespace GameLogic
{
    public class SummonAction : AbilityEffect
    {
        public override EffectType ActionType => EffectType.Summon;

        public override string Description(string target, int amount, bool firstPerson, Creature summon)
        {
            return $"summon a {summon.Attack}/{summon.Health} {summon.name}" ;
        }

        public override bool CanExecute(AbilityWithEffect ability, AbilityHolder owner, List<Card> potentialTargets)
        {
            return true;
        }

        public override void ExecuteEffect(AbilityWithEffect ability, AbilityHolder owner, List<Card> targets)
        {
            var summon = ability.ResultingAction.Summons;

            Event.OnAbilityExecution.Invoke(ability, owner, targets);

            if (owner == null || owner.InDeck == null || !summon)
                return;
            Summon(summon, owner);
        }

        private static void Summon(Creature summon, AbilityHolder owner)
        {
            var card = new Card(summon);

            owner.InDeck.AddCard(card);

            Event.OnSummon.Invoke(card,card.Location);

            card.ChangeLocation(Deck.Zone.Battlefield);

        }

        public override float GetValue(float targetValue, int amount)
        {
            return 2* targetValue;
        }
    }
}