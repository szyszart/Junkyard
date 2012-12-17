using Microsoft.Xna.Framework;

namespace Junkyard.Entities
{
    public class Ship
    {
        #region Properties

        public Vector3 Position { get; set; }

        #endregion
        #region Ctors

        public Ship(Vector3 pos)
        {
            Position = pos;
        }

        #endregion
    }
}