using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Event = GameLogic.Event;

namespace MapLogic
{


    [CreateAssetMenu(menuName = "Create Map Objects/Hire Units Option")]
    public class HireUnitOption : MapOption
    {
        public List<Creature> Units = new List<Creature>();
        public readonly Race OptionRace;
        public readonly int CR;

        public HireUnitOption(Race race, int cR)
        {
            CR = cR;

            OptionRace = race;

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

            if (Units.Count < 3) 
                selected = Units.ToArray();
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    var possible = Units.Where(u => !selected.Contains(u)).ToList();

                    if (possible.Any())
                        selected[i] = possible[Random.Range(0, possible.Count())];
                }
            }

            Debug.Log(Units.Select(c=>c.name).Aggregate("Choose between ",(res,next) => res + "; "+ next));

            Event.OnHireMinions.Invoke(selected);
        }


    }
}