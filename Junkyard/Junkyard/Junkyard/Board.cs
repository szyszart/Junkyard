using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private static Random random = new Random();

        public BlockType[,] Blocks { get; protected set; }
        public Point Dimensions { get; protected set; }
        public Player Player { get; set; }
        public PuzzleBoard(int width, int height)
        {
            Blocks = new BlockType[width, height];
            Dimensions = new Point(width, height);
        }

        public void Randomize()
        {
            Randomize(Dimensions.Y);
        }

        public void Randomize(int upTo)
        {
            Array values = Enum.GetValues(typeof(BlockType));
            for (int y = 0; y < upTo; y++)
                for (int x = 0; x < Dimensions.X; x++)
                {
                    // skip BlockType.Empty and BlockType.Other
                    Blocks[x, y] = (BlockType)values.GetValue(random.Next(2, values.Length));
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

        public void MoveDown(int numRows)
        {
            if (Dimensions.Y <= 0 || numRows == 0)
                return;

            if (numRows >= Dimensions.Y) {
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
    }

    public class LayoutDescription
    {
        public string Name { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public Texture2D Thumbnails { get; set; }
        public Point BlockSize { get; set; }
        public Point[] ThumbnailBlocks { get; set; }
        
        public LayoutDescription(string name, string top, string bottom)
        {
            Name = name;
            Top = top;
            Bottom = bottom;
        }
    }

    public class LayoutInstance
    {
        public LayoutDescription Layout { get; set; }
        public Point Location { get; set; }
        public LayoutInstance(LayoutDescription desc, Point loc)
        {
            Layout = desc;
            Location = loc;
        }
    }

    class BoardLayoutFinder
    {
        protected List<LayoutDescription> layouts;

        public BoardLayoutFinder()
        {
            layouts = new List<LayoutDescription>();
        }

        public void AddLayout(LayoutDescription desc)
        {
            layouts.Add(desc);
        }

        public void RemoveLayout(string name)
        {
            var toRemove = layouts.Find(x => x.Name == name);            
            layouts.Remove(toRemove);
        }

        public IEnumerable<LayoutInstance> Scan(PuzzleBoard board)
        {
            List<LayoutInstance> result = new List<LayoutInstance>();
            if (board.Dimensions.Y < 1)
                return result;

            int numFreeBlocks = 2 * board.Dimensions.X;
            foreach (LayoutDescription desc in layouts)
            {                
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
                            LayoutInstance instance = new LayoutInstance(desc, new Point(x, board.Dimensions.Y - 1 - y));
                            result.Add(instance);
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