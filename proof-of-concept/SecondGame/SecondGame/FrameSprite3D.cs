using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondGame
{
    struct SpriteSheetDimensions
    {
        public int width, height, numFrames;
        public SpriteSheetDimensions(int width, int height, int numFrames)
        {
            this.width = width;
            this.height = height;
            this.numFrames = numFrames;
        }
    }

    class FrameSprite3D : Sprite3D
    {
        public SpriteSheetDimensions gridDimensions { get; set; }
        protected Point _currentFrame;
        public Point currentFrame
        {
            get { return _currentFrame; }
            private set
            {
                _currentFrame = value;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].TextureCoordinate.X = _currentFrame.X * (1.0f / gridDimensions.width) + textureCoords[i].X * (1.0f / gridDimensions.width);
                    vertices[i].TextureCoordinate.Y = _currentFrame.Y * (1.0f / gridDimensions.height) + textureCoords[i].Y * (1.0f / gridDimensions.height);
                }
            }
        }

        public FrameSprite3D(Texture2D texture, Vector3 position, Quaternion rotation, Vector3 scale, SpriteSheetDimensions gridDimensions)
            : base(texture, position, rotation, scale)
        {
            this.gridDimensions = gridDimensions;
            currentFrame = new Point(0, 0);
        }
        public FrameSprite3D(Texture2D texture, Vector3 position, SpriteSheetDimensions gridDimensions)
            : this(texture, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One, gridDimensions)
        {
        }
        public void NextFrame()
        {
            Point current = currentFrame;
            current.X++;
            if (current.X >= gridDimensions.width)
            {
                current.X = 0;
                current.Y++;
                if (current.Y >= gridDimensions.height)
                    current.Y = 0;
            }
            int numFrames = current.Y * gridDimensions.width + current.X;
            if (numFrames >= gridDimensions.numFrames)
            {
                current.X = current.Y = 0;
            }

            currentFrame = current;
        }
        public void Reset()
        {
            currentFrame = Point.Zero;
        }
    }
}
