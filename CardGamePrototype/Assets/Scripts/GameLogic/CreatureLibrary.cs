using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu]
    public class CreatureLibrary : SingletonScriptableObject<CreatureLibrary>
    {
        public Race[] AllRaces;
        public Creature[] EnemyCreatures;
    }

}