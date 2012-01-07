using Microsoft.Xna.Framework;

namespace Junkyard.Entities
{
    public class Ship
    {
        #region Properties
        public Vector3 Position{ get; set; }        

        public Ship(Vector3 pos)
        {
            Position = pos;
        }

        #endregion
    }
}
