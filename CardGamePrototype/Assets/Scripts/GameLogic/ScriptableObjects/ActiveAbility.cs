using System;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class ActiveAbility : Ability
    {
        public bool OncePrCombat;
        private bool ActivatedThisTurn;

        private void OnEnable()
        {
            Event.OnCombatResolveStart.AddListener(RefreshAction);
        }

        private void OnDestroy()
        {
            Event.OnCombatResolveStart.RemoveListener(RefreshAction);
        }

        private void RefreshAction()
        {
            ActivatedThisTurn = false;
        }

        public void ActivateAbility(AbilityHolder owner)
        {
            //TODO: should have different costs?
            if (!owner.InDeck.DeckController.ActionAvailable() || ActivatedThisTurn)
                return;

            ActivatedThisTurn = true;

            ExecuteAction(owner, null);

            Event.OnPlayerAction.Invoke(owner.InDeck);
            //owner.InDeck.DeckController.UsedAction(owner.InDeck);
        }

        public override string Description(ICharacter owner)
        {
            return $"Activate: {ResultingAction.Description(owner)}.";
        }

    }

}