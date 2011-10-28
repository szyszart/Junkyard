using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondGame
{
    class Sprite3D
    {
        protected static Vector2[] textureCoords = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 scale { get; set; }
        public Texture2D texture { get; set; }
        protected VertexPositionNormalTexture[] vertices;               

        public Sprite3D(Texture2D texture, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.texture = texture;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            vertices = new VertexPositionNormalTexture[4];            
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1, 1, 0), Vector3.Forward, textureCoords[0]);
            vertices[1] = new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Forward, textureCoords[1]);
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-1, -1, 0), Vector3.Forward, textureCoords[2]);
            vertices[3] = new VertexPositionNormalTexture(new Vector3(1, -1, 0), Vector3.Forward, textureCoords[3]);
        }
        public Sprite3D(Texture2D texture, Vector3 position)
            : this(texture, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }
        public virtual void Draw(BasicEffect effect, Camera camera)
        {            
            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;
            effect.World = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.Texture = texture;
            effect.TextureEnabled = true;          

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();                
                effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }
        }
    }
}
