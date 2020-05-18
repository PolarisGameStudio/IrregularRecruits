using GameLogic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class IconManager : Singleton<IconManager>
    {
        [System.Serializable]
        public struct AbilityActionIcon { public PassiveAbility.ActionType ActionType; public Sprite Sprite; }

        public AbilityActionIcon[] AbilityActionIcons;

        public static Sprite GetAbilityIconSprite(PassiveAbility.ActionType actionType)
        {
            if (!Instance.AbilityActionIcons.Any(ai => ai.ActionType == actionType)) return null;

            return Instance.AbilityActionIcons.First(ai => ai.ActionType == actionType).Sprite;
        }
    }
}