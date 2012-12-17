using System;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    public enum BlockType
    {
        Empty,
        Other,
        Wood,
        Metal,
        Fire,
        Grog,
    }

    public class PuzzleBoard
    {
        #region Private fields

        private static readonly Random random = new Random();

        #endregion
        #region Properties

        public BlockType[,] Blocks { get; protected set; }
        public Point Dimensions { get; protected set; }
        public Player Player { get; set; }

        #endregion
        #region Ctors

        public PuzzleBoard(int width, int height)
        {
            Blocks = new BlockType[width,height];
            Dimensions = new Point(width, height);
        }

        #endregion
        #region Public methods

        public void Clear()
        {
            for (int y = 0; y < Dimensions.Y; y++)
                for (int x = 0; x < Dimensions.X; x++)
                {
                    Blocks[x, y] = BlockType.Empty;
                }
        }

        public void MoveDown(int numRows)
        {
            if (Dimensions.Y <= 0 || numRows == 0)
                return;

            if (numRows >= Dimensions.Y)
            {
                Clear();
                return;
            }

            for (int i = Dimensions.Y - 1; i >= numRows; i--)
            {
                for (int x = 0; x < Dimensions.X; x++)
                    Blocks[x, i] = Blocks[x, i - numRows];
            }

            Randomize(numRows);
        }

        public void Randomize()
        {
            Randomize(Dimensions.Y);
        }

        public void Randomize(int upTo)
        {
            Array values = Enum.GetValues(typeof (BlockType));
            for (int y = 0; y < upTo; y++)
                for (int x = 0; x < Dimensions.X; x++)
                {
                    // skip BlockType.Empty and BlockType.Other
                    Blocks[x, y] = (BlockType) values.GetValue(random.Next(2, values.Length));
                }
        }

        public void Swap(Point a, Point b)
        {
            BlockType tmp = Blocks[a.X, a.Y];
            Blocks[a.X, a.Y] = Blocks[b.X, b.Y];
            Blocks[b.X, b.Y] = tmp;
        }

        #endregion
    }
}