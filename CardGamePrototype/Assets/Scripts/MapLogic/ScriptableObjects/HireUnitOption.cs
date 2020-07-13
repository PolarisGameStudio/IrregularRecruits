using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Hire Units Option")]
    public class HireUnitOption : MapOption
    {
        public List<Creature> Units = new List<Creature>();

        public HireUnitOption(Race race, int cR)
        {
            var maxCr = cR / 3;

            var potential = CreatureLibrary.Instance.EnemyCreatures.Where(c => c.Race == race).OrderBy(c=> Random.value).ToList();

            //Allow uniques check?

            var forHire = Mathf.Min(potential.Count, 3);

            while(Units.Count < forHire)
            {
                Creature creature;

                if (potential.Any(p => p.CR <= maxCr))
                    creature = potential.First(p => p.CR <= maxCr);
                else
                    creature = potential.First();

                Units.Add(creature);

                potential.Remove(creature);
            }

        }

        public override float Difficulty()
        {
            if (!Units.Any())
                return 0;

            return Units.Max(u => u.CR);
        }

        public override void ExecuteOption(MapNode owner)
        {
            var selected = new Creature[3];

            for (int i = 0; i < 3; i++)
            {
                var possible = Units.Where(u=> !selected.Contains(u)).ToList();

                if (possible.Any())
                    selected[i] = possible[Random.Range(0, possible.Count())];
            }

            Debug.Log(Units.Select(c=>c.name).Aggregate("Choose between ",(res,next) => res + "; "+ next));

            //AddMinionScreen.Instance.SetupChoice(selected[0], selected[1], selected[2]);
        }


    }
}