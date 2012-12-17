using System.Collections.Generic;

namespace Junkyard
{
    internal class BoardLayoutFinder
    {
        #region Private fields

        protected List<LayoutDescription> layouts;

        #endregion
        #region Ctors

        public BoardLayoutFinder()
        {
            layouts = new List<LayoutDescription>();
        }

        #endregion
        #region Public methods

        public void AddLayout(LayoutDescription desc)
        {
            layouts.Add(desc);
        }

        public void RemoveLayout(string name)
        {
            LayoutDescription toRemove = layouts.Find(x => x.Name == name);
            layouts.Remove(toRemove);
        }

        public IEnumerable<LayoutInstance> Scan(PuzzleBoard board)
        {
            var result = new List<LayoutInstance>();
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
                            var instance = new LayoutInstance(desc, new Point(x, board.Dimensions.Y - 1 - y));
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

        #endregion
        #region Protected methods

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

        #endregion
        #region Private methods

        private void ClearLayout(PuzzleBoard board, LayoutDescription desc, int startX, int startY)
        {
            int layoutWidth = desc.Bottom.Length;
            int layoutHeight = (string.IsNullOrEmpty(desc.Top) ? 1 : 2);

            for (int y = startY; y < startY + layoutHeight; y++)
                for (int x = startX; x < startX + layoutWidth; x++)
                {
                    board.Blocks[x, y] = BlockType.Empty;
                }
        }

        #endregion
    }
}