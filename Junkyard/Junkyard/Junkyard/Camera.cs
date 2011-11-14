using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    abstract class Camera
    {
        public virtual Matrix View { get; protected set; }
        public virtual Matrix Projection { get; protected set; }
        public virtual Vector3 Position { get; set; }
        public virtual void Update() 
        {
        }
    }

    abstract class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }
    }

    class FreeCamera : PerspectiveCamera
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        private Vector3 target;
        private Vector3 translation;

        public FreeCamera(Vector3 position, float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
            : base(fieldOfView, aspectRatio, nearPlane, farPlane)
        {
            Yaw = Pitch = Roll = 0;
            Position = position;
            translation = Vector3.Zero;
        }

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

        public void Translate(Vector3 t)
        {
            translation += t;
        }        
    }
}
