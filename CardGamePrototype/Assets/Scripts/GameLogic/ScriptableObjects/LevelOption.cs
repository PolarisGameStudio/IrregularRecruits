using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
        [CreateAssetMenu]
        [Serializable]
        public class LevelOption : ScriptableObject
        {
            public List<Ability> Options = new List<Ability>();
        }
}