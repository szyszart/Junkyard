using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    enum BlockType
    {
        Empty,
        Wood,
        Metal,
        Fire,
        Grog
    }

    class PuzzleBoard
    {
        private static Random random = new Random();

        public BlockType[,] Blocks { get; protected set; }
        public Point Dimensions { get; protected set; }
        public PuzzleBoard(int width, int height)
        {
            Blocks = new BlockType[width, height];
            Dimensions = new Point(width, height);
        }

        public void Randomize()
        {
            Array values = Enum.GetValues(typeof(BlockType));
            for (int y = 0; y < Dimensions.Y; y++)
                for (int x = 0; x < Dimensions.X; x++)
                {
                    Blocks[x, y] = (BlockType)values.GetValue(random.Next(1, values.Length));
                }
        }

        public void Swap(Point a, Point b)
        {
            BlockType tmp = Blocks[a.X, a.Y];
            Blocks[a.X, a.Y] = Blocks[b.X, b.Y];
            Blocks[b.X, b.Y] = tmp;
        }

        public void Clear()
        {
            for (int y = 0; y < Dimensions.Y; y++)
                for (int x = 0; x < Dimensions.X; x++)
                {
                    Blocks[x, y] = BlockType.Empty;
                }
        }
    }


    class LayoutDescription
    {
        public string Top { get; set; }
        public string Bottom { get; set; }
        public LayoutDescription(string top, string bottom)
        {
            Top = top;
            Bottom = bottom;
        }
    }

    class BoardLayoutFinder
    {
        protected Dictionary<string, LayoutDescription> layouts;

        public BoardLayoutFinder()
        {
            layouts = new Dictionary<string, LayoutDescription>();
        }

        public void AddLayout(string name, LayoutDescription desc)
        {
            layouts.Add(name, desc);
        }

        public void RemoveLayout(string name)
        {
            layouts.Remove(name);
        }

        public IEnumerable<string> Scan(PuzzleBoard board)
        {
            List<string> result = new List<string>();
            if (board.Dimensions.Y < 1)
                return result;

            int numFreeBlocks = 2 * board.Dimensions.X;
            foreach (KeyValuePair<string, LayoutDescription> layout in layouts)
            {
                LayoutDescription desc = layout.Value;
                int layoutWidth = desc.Bottom.Length;
                int layoutHeight = string.IsNullOrEmpty(desc.Top) ? 1 : 2;
                if (layoutWidth > board.Dimensions.X)
                    continue;

                for (int y = ((layoutHeight == 1) ? 0 : 1); y >= layoutHeight - 1; y--)
                {
                    for (int x = 0; x <= board.Dimensions.X - layoutWidth; x += 2)
                    {
                        if (MatchLayout(board, desc, x, board.Dimensions.Y - 1 - y))
                        {
                            result.Add(layout.Key);
                            ClearLayout(board, desc, x, board.Dimensions.Y - 1 - y);
                            x += layoutWidth;
                            numFreeBlocks -= layoutWidth * layoutHeight;
                            if (numFreeBlocks <= 0)
                                return result;
                        }
                    }
                }
            }
            return result;
        }

        protected bool MatchLayout(PuzzleBoard board, LayoutDescription desc, int startX, int startY)
        {
            int layoutWidth = desc.Bottom.Length;
            int layoutHeight = (string.IsNullOrEmpty(desc.Top) ? 1 : 2);
            string pattern = desc.Top + desc.Bottom;

            int i = 0;
            for (int y = startY; y < startY + layoutHeight; y++)
                for (int x = startX; x < startX + layoutWidth; x++)
                {
                    if (board.Blocks[x, y] != CharToBlock(pattern[i++]))
                        return false;
                }
            return true;
        }

        void ClearLayout(PuzzleBoard board, LayoutDescription desc, int startX, int startY)
        {
            int layoutWidth = desc.Bottom.Length;
            int layoutHeight = (string.IsNullOrEmpty(desc.Top) ? 1 : 2);

            int i = 1;
            for (int y = startY; y < startY + layoutHeight; y++)
                for (int x = startX; x < startX + layoutWidth; x++)
                {
                    board.Blocks[x, y] = BlockType.Empty;
                }
        }

        protected BlockType CharToBlock(char c)
        {
            switch (c)
            {
                case 'w':
                case 'W':
                    return BlockType.Wood;
                case 'm':
                case 'M':
                    return BlockType.Metal;
                case 'g':
                case 'G':
                    return BlockType.Grog;
                case 'f':
                case 'F':
                    return BlockType.Fire;
                default:
                    return BlockType.Empty;
            }
        }
    }
}