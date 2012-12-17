using Microsoft.Xna.Framework;

namespace Junkyard.Camera
{
    internal abstract class Camera
    {
        #region Properties

        public virtual Vector3 Position { get; set; }
        public virtual Matrix Projection { get; protected set; }
        public virtual Matrix View { get; protected set; }

        #endregion
        #region Public methods

        public virtual void Update()
        {
        }

        #endregion
    }
}