using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondGame
{
    class SimpleCamera: Camera
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public Vector3 Position { get; set; }

        private Vector3 target;
        private Vector3 translation;

        public SimpleCamera(GraphicsDevice graphicsDevice, Vector3 position): base(graphicsDevice)
        {
            Yaw = Pitch = 0;
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
