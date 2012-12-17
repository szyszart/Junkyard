using Microsoft.Xna.Framework;

namespace Junkyard.Camera
{
    internal class FreeCamera : PerspectiveCamera
    {
        #region Private fields

        private Vector3 target;
        private Vector3 translation;

        #endregion
        #region Properties

        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        #endregion
        #region Ctors

        public FreeCamera(Vector3 position, float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
            : base(fieldOfView, aspectRatio, nearPlane, farPlane)
        {
            Yaw = Pitch = Roll = 0;
            Position = position;
            translation = Vector3.Zero;
        }

        #endregion
        #region Overrides

        public override void Update()
        {
            base.Update();

            Matrix rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            translation = Vector3.Transform(translation, rotation);
            Position += translation;
            translation = Vector3.Zero;

            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            target = Position + forward;
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            View = Matrix.CreateLookAt(Position, target, up);
        }

        #endregion
        #region Public methods

        public void Translate(Vector3 t)
        {
            translation += t;
        }

        #endregion
    }
}