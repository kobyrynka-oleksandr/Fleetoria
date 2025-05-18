using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class PlayerHuman : Player
    {
        public PlayerHuman() : base() { }
        public void ClearMatrixWhenShipMovedOnGrid(int row, int col, int deckCount, bool isRotated)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

            for (int i = 0; i < deckCount; i++)
            {
                int r = row + (isRotated ? 0 : i);
                int c = col + (isRotated ? i : 0);

                if (r >= 0 && r < rows && c >= 0 && c < cols)
                {
                    matrixOfBattle[r, c] = 0;
                }
            }

            int startRow = row - 1;
            int endRow = isRotated ? row + 1 : row + deckCount;
            int startCol = col - 1;
            int endCol = isRotated ? col + deckCount : col + 1;

            for (int r = startRow; r <= endRow; r++)
            {
                for (int c = startCol; c <= endCol; c++)
                {
                    if (r >= 0 && r < rows && c >= 0 && c < cols)
                    {
                        if (matrixOfBattle[r, c] == 2 && !HasAdjacentShip(r, c))
                        {
                            matrixOfBattle[r, c] = 0;
                        }
                    }
                }
            }
            health -= deckCount;
        }
    }
}
