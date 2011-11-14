using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    interface IDrawable
    {        
        void Draw(Effect effect);        
    }

    class Sprite3D : IDrawable
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D NormalMap { get; set; }
        public Color TintColor { get; set; }

        public Matrix World
        {
            get
            {
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }
        }

        protected VertexPositionNormalTexture[] vertices;

        public Sprite3D(Texture2D texture, Texture2D normalMap, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Texture = texture;
            this.NormalMap = normalMap;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.TintColor = Color.White;            
            
            vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1, +1, 0), Vector3.Backward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(+1, +1, 0), Vector3.Backward, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-1, -1, 0), Vector3.Backward, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(+1, -1, 0), Vector3.Backward, new Vector2(1, 1));
        }
                
        public Sprite3D(Texture2D texture, Vector3 position)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }        

        public void Draw(Effect effect)
        {             
            effect.Parameters["World"].SetValue(World);
            if (effect.Parameters["Texture"] != null)
                effect.Parameters["Texture"].SetValue(Texture);

            if (effect.Parameters["NormalMap"] != null)
                effect.Parameters["NormalMap"].SetValue(NormalMap);
                
            if (effect.Parameters["NormalMapEnabled"] != null)
                effect.Parameters["NormalMapEnabled"].SetValue(NormalMap != null);
            
            if (effect.Parameters["DiffuseColor"] != null)
                effect.Parameters["DiffuseColor"].SetValue(TintColor.ToVector3());                                   

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }
        }
    }

    struct SpriteSheetDimensions
    {
        public int Width, Height, NumFrames;
        public SpriteSheetDimensions(int width, int height, int numFrames)
        {
            this.Width = width;
            this.Height = height;
            this.NumFrames = numFrames;
        }
    }

    class FrameSprite3D : Sprite3D
    {
        public SpriteSheetDimensions gridDimensions { get; set; }
        protected Point currentFrame;
        public Point CurrentFrame
        {
            get { return currentFrame; }
            private set
            {
                currentFrame = value;
                UpdateTexCoords();
            }
        }

        protected void UpdateTexCoords()
        {
            float boxWidth = (1.0f / gridDimensions.Width);
            float boxHeight = (1.0f / gridDimensions.Height);

            float xLeft = currentFrame.X * boxWidth;
            float yTop = currentFrame.Y * boxHeight;

            vertices[0].TextureCoordinate = new Vector2(xLeft, yTop);
            vertices[1].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop);
            vertices[2].TextureCoordinate = new Vector2(xLeft, yTop + boxHeight);
            vertices[3].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop + boxHeight);
        }

        public FrameSprite3D(Texture2D texture, Texture2D normalMap, Vector3 position, Quaternion rotation, Vector3 scale, SpriteSheetDimensions gridDimensions)
            : base(texture, normalMap, position, rotation, scale)
        {
            this.gridDimensions = gridDimensions;
            CurrentFrame = Point.Zero;
        }

        public FrameSprite3D(Texture2D texture, Vector3 position, SpriteSheetDimensions gridDimensions)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One, gridDimensions)
        {
        }

        public void NextFrame()
        {
            currentFrame.X++;
            if (currentFrame.X >= gridDimensions.Width)
            {
                currentFrame.X = 0;
                currentFrame.Y++;
                if (currentFrame.Y >= gridDimensions.Height)
                    currentFrame.Y = 0;
            }
            int numFrames = currentFrame.Y * gridDimensions.Width + currentFrame.X;
            if (numFrames >= gridDimensions.NumFrames)
            {
                currentFrame.X = currentFrame.Y = 0;
            }
            UpdateTexCoords();
        }

        public void Reset()
        {
            CurrentFrame = Point.Zero;
        }
    }
}
