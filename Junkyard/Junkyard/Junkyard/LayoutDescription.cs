using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class LayoutDescription
    {
        #region Properties

        public Point BlockSize { get; set; }
        public string Bottom { get; set; }
        public string Name { get; set; }
        public Point[] ThumbnailBlocks { get; set; }
        public Texture2D Thumbnails { get; set; }
        public string Top { get; set; }

        #endregion
        #region Ctors

        public LayoutDescription(string name, string top, string bottom)
        {
            Name = name;
            Top = top;
            Bottom = bottom;
        }

        #endregion
    }
}