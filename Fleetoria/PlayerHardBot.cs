﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class PlayerHardBot : PlayerBot
    {
        private PlayerHuman human;
        private Random random = new Random();

        private enum AttackMode { Random, SearchDirection, FinishShip }
        private AttackMode mode = AttackMode.Random;

        private (int row, int col)? firstHit = null;
        private (int row, int col)? lastHit = null;
        private List<(int dRow, int dCol)> directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1) };
        private int currentDirectionIndex = -1;


        public PlayerHardBot(PlayerHuman human) : base()
        {
            this.human = human;
        }

        public override (int row, int col) GetCellForAttack()
        {
            (int row, int col) target;

            if (mode == AttackMode.Random)
            {
                target = GetRandomCell();
                if (human.IsShipPresent(target.row + 1, target.col + 1))
                {
                    Shuffle(directions, random);
                    firstHit = lastHit = target;
                    mode = AttackMode.SearchDirection;
                }
            }
            else if (mode == AttackMode.SearchDirection)
            {
                target = TryNextDirection();
            }
            else
            {
                target = TryContinueDirection();
            }

            return target;
        }

        private (int row, int col) GetRandomCell()
        {
            var index = random.Next(cells.Count);
            var cell = cells[index];
            cells.RemoveAt(index);
            return cell;
        }

        private (int row, int col) TryNextDirection()
        {
            while (++currentDirectionIndex < directions.Count)
            {
                var dir = directions[currentDirectionIndex];
                var next = (firstHit.Value.row + dir.dRow, firstHit.Value.col + dir.dCol);

                if (IsValidCell(next) && cells.Contains(next))
                {
                    cells.Remove(next);
                    if (human.IsShipPresent(next.Item1 + 1, next.Item2 + 1))
                    {
                        lastHit = next;
                        human.ChangeMatrixCellForMinus1(next.Item1, next.Item2);
                        if (human.IsShipDestroyed(next.Item1 + 1, next.Item2 + 1))
                        {
                            human.ChangeMatrixCellBack(next.Item1, next.Item2);
                            NotifyShipDestroyed();
                            return next;
                        }
                        else
                        {
                            human.ChangeMatrixCellBack(next.Item1, next.Item2);
                            mode = AttackMode.FinishShip;
                        }
                    }
                    return next;
                }
            }

            mode = AttackMode.FinishShip;
            currentDirectionIndex = -1;
            lastHit = firstHit;
            return GetCellForAttack();
        }

        private (int row, int col) TryContinueDirection()
        {
            if (currentDirectionIndex < 0 || currentDirectionIndex >= directions.Count)
            {
                mode = AttackMode.Random;
                firstHit = null;
                lastHit = null;
                currentDirectionIndex = -1;
                return GetCellForAttack();
            }

            var dir = directions[currentDirectionIndex];
            var next = (lastHit.Value.row + dir.dRow, lastHit.Value.col + dir.dCol);

            if (IsValidCell(next) && cells.Contains(next))
            {
                cells.Remove(next);
                if (human.IsShipPresent(next.Item1 + 1, next.Item2 + 1))
                {
                    lastHit = next;
                    human.ChangeMatrixCellForMinus1(next.Item1, next.Item2);
                    if (human.IsShipDestroyed(next.Item1 + 1, next.Item2 + 1))
                    {
                        human.ChangeMatrixCellBack(next.Item1, next.Item2);
                        NotifyShipDestroyed();
                        return next;
                    }
                    human.ChangeMatrixCellBack(next.Item1, next.Item2);
                }
                else
                {
                    lastHit = firstHit;
                    var reversed = (-dir.dRow, -dir.dCol);
                    currentDirectionIndex = directions.FindIndex(d => d == reversed);
                }
                return next;
            }
            else
            {
                lastHit = firstHit;
                var reversed = (-dir.dRow, -dir.dCol);
                currentDirectionIndex = directions.FindIndex(d => d == reversed);
                return TryContinueDirection();
            }
        }

        private bool IsValidCell((int row, int col) cell)
        {
            return cell.row >= 0 && cell.row < 10 && cell.col >= 0 && cell.col < 10;
        }

        private void NotifyShipDestroyed()
        {
            mode = AttackMode.Random;
            firstHit = null;
            lastHit = null;
            currentDirectionIndex = -1;
        }
        public static void Shuffle<T>(List<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
