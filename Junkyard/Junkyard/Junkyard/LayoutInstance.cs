using Microsoft.Xna.Framework;

namespace Junkyard
{
    public class LayoutInstance
    {
        #region Properties

        public LayoutDescription Layout { get; set; }
        public Point Location { get; set; }

        #endregion
        #region Ctors

        public LayoutInstance(LayoutDescription desc, Point loc)
        {
            Layout = desc;
            Location = loc;
        }

        #endregion
    }
}