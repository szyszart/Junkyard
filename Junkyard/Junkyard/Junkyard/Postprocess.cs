using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Junkyard
{
    abstract class Postprocess
    {
        protected GraphicsDevice graphicsDevice;
        public Postprocess(GraphicsDevice device)
        {
            graphicsDevice = device;
        }
        public abstract void Apply();
    }
}
