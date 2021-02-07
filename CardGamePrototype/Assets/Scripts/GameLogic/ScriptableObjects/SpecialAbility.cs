using UnityEngine;

namespace GameLogic
{
    public abstract class SpecialAbility : ScriptableObject
    {

        public string Name;
        public Sprite Icon;

        public abstract string Description(ICharacter owner);

        public virtual void SetupListeners(AbilityHolder _owner)
        {

        }


    }
}