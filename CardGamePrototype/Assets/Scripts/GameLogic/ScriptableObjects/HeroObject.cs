using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{

    [CreateAssetMenu(menuName = "Create Game Objects/Hero")]
    public partial class HeroObject : ScriptableObject
    {
        public new string name;
        public Race Race;

        public Sprite Portrait;
        public Ability StartingAbility;

        public LevelOption Class;
        public LevelOption RaceOption;


        //first level is 0
        //TODO: remove
        public List<Ability> GetLevelChoices(int level)
        {
            var choices = new List<Ability>();

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