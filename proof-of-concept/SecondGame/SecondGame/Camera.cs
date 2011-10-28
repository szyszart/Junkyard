using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondGame
{
    class Camera
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        protected GraphicsDevice GraphicsDevice { get; set; }

        private void GenerateProjectionMatrix(float fieldOfView, float nearPlane, float farPlane)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            float aspectRatio = (float)pp.BackBufferWidth / (float)pp.BackBufferHeight;            
            Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }

        public Camera(GraphicsDevice graphicsDevice, float nearPlane = 0.1f, float farPlane = 10000.0f)
        {
            this.GraphicsDevice = graphicsDevice;
            GenerateProjectionMatrix(MathHelper.PiOver4, nearPlane, farPlane);
        }

        public virtual void Update()
        {
        }
    }
}
