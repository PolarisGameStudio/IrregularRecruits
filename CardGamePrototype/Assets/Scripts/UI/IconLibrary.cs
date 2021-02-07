using GameLogic;
using System;
using System.Linq;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu]
    public class IconLibrary : SingletonScriptableObject<IconLibrary>
    {
        [System.Serializable]
        public struct AbilityActionIcon { public ActionType ActionType; public Sprite Sprite; }

        public AbilityActionIcon[] AbilityActionIcons;

        public static Sprite GetAbilityIconSprite(ActionType actionType)
        {
            if (!Instance.AbilityActionIcons.Any(ai => ai.ActionType == actionType)) return null;

            return Instance.AbilityActionIcons.First(ai => ai.ActionType == actionType).Sprite;
        }

    }
}