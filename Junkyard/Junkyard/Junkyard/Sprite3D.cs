using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public interface IDrawable
    {        
        void Draw(Effect effect);
    }

    public class Sprite3D : IDrawable
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        protected Texture2D texture;
        public virtual Texture2D Texture {             
            get 
            {
                return texture;
            }
            
            set 
            {
                texture = value;
            }
        }

        public Texture2D NormalMap { get; set; }
        public Color TintColor { get; set; }

        public virtual Matrix World
        {
            get
            {
                float flip = Flipped ? -1 : 1;
                return Matrix.CreateScale(new Vector3(flip * Scale.X, Scale.Y, Scale.Z)) * 
                    Matrix.CreateFromQuaternion(Rotation) * 
                    Matrix.CreateTranslation(Position);
            }
        }

        protected VertexPositionNormalTexture[] vertices;

        public virtual bool Flipped { get; set; }

        public Sprite3D() : this(null, null, Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }

        public Sprite3D(Texture2D texture, Vector3 position)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }

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

    public class ScaledSprite3D : Sprite3D
    {
        public readonly Vector2 DefaultPixelScale = new Vector2(0.002f, 0.002f);

        public override Texture2D Texture {             
            get 
            {
                return texture;
            }
            
            set 
            {
                texture = value;
                if (texture != null)
                    this.TexRect = texture.Bounds;
            }
        }

        // frame rectangle coords in pixels
        protected Rectangle texRect;
        public Rectangle TexRect {
            get
            {
                return texRect;
            }

            set
            {
                texRect = value;    

                float boxWidth = ((float)(texRect.Width) / Texture.Width);
                float boxHeight = ((float)(texRect.Height) / Texture.Height);

                float xLeft = ((float)(texRect.X) / Texture.Width);
                float yTop = ((float)(texRect.Y) / Texture.Height);       
     
                vertices[0].TextureCoordinate = new Vector2(xLeft, yTop);
                vertices[1].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop);
                vertices[2].TextureCoordinate = new Vector2(xLeft, yTop + boxHeight);
                vertices[3].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop + boxHeight);
            }        
        }

        // offset in pixels
        public Point Offset { get; set; }

        // world units per pixel
        public Vector2 PixelScale { get; set; }
        
        public override Matrix World
        {
            get
            {
                float sign = Flipped ? -1 : 1;
                return
                    Matrix.CreateScale(sign * TexRect.Width * PixelScale.X, TexRect.Height * PixelScale.Y, 1) *
                    Matrix.CreateTranslation(sign * Offset.X * PixelScale.X, Offset.Y * PixelScale.Y, 0) *
                    Matrix.CreateScale(Scale) * 
                    Matrix.CreateFromQuaternion(Rotation) * 
                    Matrix.CreateTranslation(Position);
            }
        }

        public ScaledSprite3D() : this(null, Vector3.Zero) { }

        public ScaledSprite3D(Texture2D texture, Vector3 position)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One) { }

        public ScaledSprite3D(Texture2D tex, Texture2D normalMap, Vector3 position, Quaternion rotation, Vector3 scale) : base(tex, normalMap, position, rotation, scale)
        {          
            this.Texture = tex;
            this.Offset = Point.Zero;
            this.PixelScale = DefaultPixelScale;

            vertices[0] = new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.Backward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Backward, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Backward, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Backward, new Vector2(1, 1));
        }
    }
}
