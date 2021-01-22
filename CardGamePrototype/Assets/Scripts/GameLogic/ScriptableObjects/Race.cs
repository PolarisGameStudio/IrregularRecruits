using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Race")]
    //TODO: merge with ability
    public class Race : ScriptableObject
    {
        public new string name;
        public Sprite Icon;
        public string Description;
        public Ability.ActionType[] FavoriteActions;
        public PassiveAbility.Verb[] FavoriteTriggers;

        public Race[] FriendRaces;
    }
}