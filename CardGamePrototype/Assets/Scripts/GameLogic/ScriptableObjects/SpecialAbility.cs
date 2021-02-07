using UnityEngine;

namespace GameLogic
{
    public abstract class SpecialAbility : ScriptableObject
    {

        public string Name;
        public Sprite Icon;

        public abstract string Description(ICharacter owner);

        public void RemoveListeners(AbilityHolder _owner)
        {
            if (!_owner.ListenersInitialized)
                Debug.LogWarning("removing non-initialized listeners ");

            Debug.Log($"Remove listener for {name}: {_owner}");

            _owner.RemoveListenerAction?.Invoke();

            _owner.ListenersInitialized = false;
            _owner.RemoveListenerAction = null;
        }

        public virtual void SetupListeners(AbilityHolder _owner)
        {

        }

        public virtual float GetValue() => 1f;


    }
}