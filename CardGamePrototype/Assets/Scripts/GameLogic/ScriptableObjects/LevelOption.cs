using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(menuName = "Create Game Objects/Level Option")]
    [Serializable]
        public class LevelOption : ScriptableObject
        {
            public List<Ability> Options = new List<Ability>();
        }
}