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
            health = 0;
        }

        public void AddShipToMatrix(int row, int col, int deckCount, bool isRotated)
        {
            int rows = matrixOfBattle.GetLength(0);
            int cols = matrixOfBattle.GetLength(1);

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
            health += deckCount;
        }
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
        public bool IsCanBeAdded(int row, int col, int deckCount, bool isRotated)
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
        public void ClearData()
        {
            matrixOfBattle = new int[10, 10];
            health = 0;
        }
        public bool IsShipPresent(int row, int col)
        {
            return matrixOfBattle[row - 1, col - 1] == 1;
        }
        public void DestroyDeck(int row, int col)
        {
            matrixOfBattle[row - 1, col - 1] = -1;
            health -= 1;
        }
        public bool IsShipDestroyed(int row, int col)
        {
            int r = row - 1;
            int c = col - 1;

            if (r < 0 || r >= 10 || c < 0 || c >= 10)
                return false;

            if (matrixOfBattle[r, c] != -1 && matrixOfBattle[r, c] != 1)
                return false;

            int left = c;
            while (left > 0 && (matrixOfBattle[r, left - 1] == 1 || matrixOfBattle[r, left - 1] == -1))
                left--;

            int right = c;
            while (right < 9 && (matrixOfBattle[r, right + 1] == 1 || matrixOfBattle[r, right + 1] == -1))
                right++;

            bool horizontalDestroyed = true;
            for (int i = left; i <= right; i++)
            {
                if (matrixOfBattle[r, i] != -1)
                {
                    horizontalDestroyed = false;
                    break;
                }
            }

            if (horizontalDestroyed && right > left)
                return true;

            int up = r;
            while (up > 0 && (matrixOfBattle[up - 1, c] == 1 || matrixOfBattle[up - 1, c] == -1))
                up--;

            int down = r;
            while (down < 9 && (matrixOfBattle[down + 1, c] == 1 || matrixOfBattle[down + 1, c] == -1))
                down++;

            bool verticalDestroyed = true;
            for (int i = up; i <= down; i++)
            {
                if (matrixOfBattle[i, c] != -1)
                {
                    verticalDestroyed = false;
                    break;
                }
            }

            if (verticalDestroyed && down > up)
                return true;

            return matrixOfBattle[r, c] == -1 && left == right && up == down;
        }
        public List<(int row, int col)> GetDestroyedShipCells(int row, int col)
        {
            int r = row - 1;
            int c = col - 1;
            var result = new List<(int, int)>();

            if (r < 0 || r >= 10 || c < 0 || c >= 10)
                return result;

            if (matrixOfBattle[r, c] != -1 && matrixOfBattle[r, c] != 1)
                return result;

            int left = c;
            while (left > 0 && (matrixOfBattle[r, left - 1] == 1 || matrixOfBattle[r, left - 1] == -1))
                left--;

            int right = c;
            while (right < 9 && (matrixOfBattle[r, right + 1] == 1 || matrixOfBattle[r, right + 1] == -1))
                right++;

            bool horizontalDestroyed = true;
            for (int i = left; i <= right; i++)
            {
                if (matrixOfBattle[r, i] != -1)
                {
                    horizontalDestroyed = false;
                    break;
                }
            }

            if (horizontalDestroyed && right > left)
            {
                for (int i = left; i <= right; i++)
                    result.Add((r + 1, i + 1));
                return result;
            }

            int up = r;
            while (up > 0 && (matrixOfBattle[up - 1, c] == 1 || matrixOfBattle[up - 1, c] == -1))
                up--;

            int down = r;
            while (down < 9 && (matrixOfBattle[down + 1, c] == 1 || matrixOfBattle[down + 1, c] == -1))
                down++;

            bool verticalDestroyed = true;
            for (int i = up; i <= down; i++)
            {
                if (matrixOfBattle[i, c] != -1)
                {
                    verticalDestroyed = false;
                    break;
                }
            }

            if (verticalDestroyed && down > up)
            {
                for (int i = up; i <= down; i++)
                    result.Add((i + 1, c + 1));
                return result;
            }

            if (matrixOfBattle[r, c] == -1 && left == right && up == down)
            {
                result.Add((r + 1, c + 1));
                return result;
            }

            return new List<(int, int)>();
        }
    }
}
