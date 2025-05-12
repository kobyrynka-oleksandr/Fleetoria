using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fleetoria
{
    public class Player
    {
        public int[,] matrixOfBattle;
        public int health;

        public Player()
        {
            matrixOfBattle = new int[10, 10];
            health = 20;
        }

        public void AddShipToMatrix(int row, int col, int deckCount, bool isRotated)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

            // Спочатку ставимо корабель
            for (int i = 0; i < deckCount; i++)
            {
                int r = row + (isRotated ? 0 : i);
                int c = col + (isRotated ? i : 0);

                if (r >= 0 && r < rows && c >= 0 && c < cols)
                {
                    matrixOfBattle[r, c] = 1;
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
                        if (matrixOfBattle[r, c] == 0)
                        {
                            matrixOfBattle[r, c] = 2;
                        }
                    }
                }
            }
        }
        public void ClearMatrixWhenShipMovedOnGrid(int row, int col, int deckCount, bool isRotated)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

            // 1. Очистити сам корабель
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
        }
        private bool HasAdjacentShip(int r, int c)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = r + dr;
                    int nc = c + dc;

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                    {
                        if (matrixOfBattle[nr, nc] == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public bool isCanBeAdded(int row, int col, int deckCount, bool isRotated)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

            for (int i = 0; i < deckCount; i++)
            {
                int r = isRotated ? row : row + i;
                int c = isRotated ? col + i : col;

                if (r < 0 || r >= rows || c < 0 || c >= cols)
                    return false;

                if (matrixOfBattle[r, c] == 1 || matrixOfBattle[r, c] == 2)
                    return false;
            }

            return true;
        }
        public string MBMatrix()
        {
            string mString = "";
            
            for (int i = 0; i < matrixOfBattle.GetLength(0); i++)
            {
                for (int j = 0; j < matrixOfBattle.GetLength(1); j++)
                {
                    mString += matrixOfBattle[i, j] + " ";
                }
                mString += "\n";
            }

            return mString;
        }
    }
}
