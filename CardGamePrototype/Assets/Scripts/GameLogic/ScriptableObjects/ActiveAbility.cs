using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class ActiveAbility : Ability
    {
        public bool OncePrCombat;

        public void ActivateAbility(AbilityHolder owner)
        {
            //TODO: should have different costs?
            if (!owner.InDeck.DeckController.ActionAvailable())
                return;

            ExecuteAction(owner, null);

            owner.InDeck.DeckController.UsedAction(owner.InDeck);
        }

        public override string Description(ICharacter owner)
        {
            return $"{owner.GetName()} {ResultingAction.Description(owner)}.";
        }

    }

}