using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{


    [CreateAssetMenu]
    public class DeckLibrary : SingletonScriptableObject<DeckLibrary>
    {
        [SerializeField]
        private List<HeroObject> Heroes;

#if UNITY_EDITOR
        public HeroObject TestHero;
#endif



        public static List<DeckObject> GetDecks()
        {
            return Instance.Heroes.Select(hero => hero.Deck).ToList();

        }

        public static List<HeroObject> GetHeroes(bool includeLocked = true)
        {
            if(includeLocked)
                return Instance.Heroes;


            List<HeroObject> heroObjects = Instance.Heroes.Where(LegacySystem.Instance.IsUnlocked).ToList();

#if UNITY_EDITOR
            heroObjects.Add(Instance.TestHero);
#endif

            return heroObjects;
        
        }
    }
}