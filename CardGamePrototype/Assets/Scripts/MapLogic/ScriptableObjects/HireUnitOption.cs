using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapLogic
{
    [CreateAssetMenu(menuName = "Create Map Objects/Hire Units Option")]
    public class HireUnitOption : MapOption
    {
        public List<Creature> Units;

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

            //AddMinionScreen.Instance.SetupChoice(selected[0], selected[1], selected[2]);
        }


    }
}