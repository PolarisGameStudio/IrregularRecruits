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
            if (!owner.GetDeck().DeckController.ActionAvailable())
                return;

            ExecuteAction(owner, null);

            owner.GetDeck().DeckController.UsedAction(owner.GetDeck());
        }
    }
}