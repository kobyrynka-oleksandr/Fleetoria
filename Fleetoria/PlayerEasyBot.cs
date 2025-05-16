using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class PlayerEasyBot : PlayerBot
    {
        public PlayerEasyBot() : base() { }
        public override (int row, int col) GetCellForAttack()
        {
            var random = new Random();

            (int row, int col) cell = cells[random.Next(cells.Count)];

            cells.Remove(cell);

            return cell;
        }
    }
}
