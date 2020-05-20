    using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{

    [CreateAssetMenu]
    public class HeroObject : ScriptableObject
    {
        public new string name;
        public Race Race;

        public Sprite Portrait;
        public Ability StartingAbility;

        public LevelOption Class;
        public LevelOption RaceOption;

        [CreateAssetMenu]
        [Serializable]
        public class LevelOption : ScriptableObject
        {
            public List<PassiveAbility> Options = new List<PassiveAbility>();
        }


        //first level is 0
        public List<PassiveAbility> GetLevelChoices(int level)
        {
            var choices = new List<PassiveAbility>();

            if (Class && Class.Options.Count > level)
            {
                choices.Add(Class.Options[level]);
            }
            if (RaceOption && RaceOption.Options.Count > level)
            {
                choices.Add(RaceOption.Options[level]);
            }

            return choices;
        }
    }
}