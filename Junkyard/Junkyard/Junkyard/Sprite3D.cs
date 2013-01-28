using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Sprite3D : IDrawable
    {
        #region Private fields

        protected Texture2D texture;
        protected VertexPositionNormalTexture[] vertices;

        #endregion
        #region Properties

        public virtual bool Flipped { get; set; }
        public Texture2D NormalMap { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public virtual Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }

        public Color TintColor { get; set; }

        protected virtual Matrix World
        {
            get
            {
                float flip = Flipped ? -1 : 1;
                return Matrix.CreateScale(new Vector3(flip*Scale.X, Scale.Y, Scale.Z))*
                       Matrix.CreateFromQuaternion(Rotation)*
                       Matrix.CreateTranslation(Position);
            }
        }

        #endregion
        #region Ctors

        public Sprite3D()
            : this(null, null, Vector3.Zero, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }

        public Sprite3D(Texture2D texture, Vector3 position)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }

        public Sprite3D(Texture2D texture, Texture2D normalMap, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Texture = texture;
            NormalMap = normalMap;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            TintColor = Color.White;

            vertices = new VertexPositionNormalTexture[4];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(-1, +1, 0), Vector3.Backward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(+1, +1, 0), Vector3.Backward, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-1, -1, 0), Vector3.Backward, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(+1, -1, 0), Vector3.Backward, new Vector2(1, 1));
        }

        #endregion
        #region IDrawable Members

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

        #endregion
    }
}