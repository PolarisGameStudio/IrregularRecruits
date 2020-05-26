using System.Collections.Generic;

namespace GameLogic
{
    public class SummonAction : AbilityAction
    {
        public override PassiveAbility.ActionType ActionType => PassiveAbility.ActionType.Summon;

        public override string Description(string target, int amount, Creature summon)
        {
            return $"summon a {summon.Attack}/{summon.Health} {summon.name}" ;
        }

        public override void ExecuteAction(Ability ability, AbilityHolder owner, List<Card> targets)
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

            Event.OnSummon.Invoke(card);

            card.ChangeLocation(Deck.Zone.Battlefield);

        }

        public override float GetValue(float targetValue, int amount)
        {
            return targetValue;
        }
    }
}