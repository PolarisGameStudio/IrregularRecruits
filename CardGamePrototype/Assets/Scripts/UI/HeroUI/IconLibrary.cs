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
        public struct AbilityEffectIcon { public EffectType ActionType; public Sprite Sprite; }

        public AbilityEffectIcon[] AbilityActionIcons;

        [System.Serializable]
        public struct AbilityTriggerIcon { public TriggerType Trigger; public Sprite Sprite; }

        public AbilityTriggerIcon[] AbilityTriggerIcons;

        public static Sprite GetAbilityIconSprite(EffectType actionType)
        {
            if (!Instance.AbilityActionIcons.Any(ai => ai.ActionType == actionType)) return null;

            return Instance.AbilityActionIcons.First(ai => ai.ActionType == actionType).Sprite;
        }

        internal static Sprite GetAbilityIconSprite(TriggerType effectTrigger)
        {
            if (!Instance.AbilityTriggerIcons.Any(ai => ai.Trigger == effectTrigger)) return null;

            return Instance.AbilityTriggerIcons.First(ai => ai.Trigger == effectTrigger).Sprite;
        }
    }
}