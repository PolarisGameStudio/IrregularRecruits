using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    //TODO: merge with ability
    public class Race : ScriptableObject
    {
        public new string name;
        public Sprite Icon;
        public string Description;
        public bool PlayerRace;
        public int GroupSize;
        public Ability.ActionType[] FavoriteActions;
        public Ability.Verb[] FavoriteTriggers;

    }
}