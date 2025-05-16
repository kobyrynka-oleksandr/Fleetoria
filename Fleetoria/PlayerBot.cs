using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public abstract class PlayerBot : Player
    {
        protected List<(int row, int col)> cells = FormCellsList(10, 10);
        public abstract (int row, int col) GetCellForAttack();
        protected static List<(int row, int col)> FormCellsList(int numOfRows, int numOfCols)
        {
            List<(int row, int col)> cellsList = new List<(int row, int col)>();

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    cellsList.Add((i, j));
                }
            }
            return cellsList;
        }
        public void RemoveMarkedCell(int row, int col)
        {
            (int row, int col) cell = (row, col);

            cells.Remove(cell);
        }
    }
}
