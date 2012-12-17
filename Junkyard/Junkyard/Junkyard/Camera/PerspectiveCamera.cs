using Microsoft.Xna.Framework;

namespace Junkyard.Camera
{
    internal abstract class PerspectiveCamera : Camera
    {
        #region Ctors

        public PerspectiveCamera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }

        #endregion
    }
}