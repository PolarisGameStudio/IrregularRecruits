using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Race")]
    //TODO: merge with ability
    public class Race : ScriptableObject
    {
        public new string name;
        public Sprite Icon;
        public Sprite Shield;
        public string Description;

        public ActionType[] FavoriteActions;
        public Verb[] FavoriteTriggers;

        public Race[] FriendRaces;
    }
}