using Microsoft.Xna.Framework;

namespace Junkyard.Animations
{
    public struct Frame
    {
        #region Private fields

        public Point Offset;
        public Rectangle Rectangle;

        #endregion
        #region Ctors

        public Frame(Rectangle rect, Point offset)
        {
            Rectangle = rect;
            Offset = offset;
        }

        #endregion
    }
}