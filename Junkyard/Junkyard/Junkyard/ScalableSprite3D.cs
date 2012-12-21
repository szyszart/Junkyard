using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class ScaledSprite3D : Sprite3D
    {
        #region Private fields

        public readonly Vector2 DefaultPixelScale = new Vector2(0.002f, 0.002f);

        // frame rectangle coords in pixels
        protected Rectangle texRect;

        #endregion
        // offset in pixels
        #region Properties

        public Point Offset { get; set; }

        // world units per pixel
        public Vector2 PixelScale { get; set; }

        public Rectangle TexRect
        {
            get { return texRect; }

            set
            {
                texRect = value;

                float boxWidth = ((float) (texRect.Width)/Texture.Width);
                float boxHeight = ((float) (texRect.Height)/Texture.Height);

                float xLeft = ((float) (texRect.X)/Texture.Width);
                float yTop = ((float) (texRect.Y)/Texture.Height);

                vertices[0].TextureCoordinate = new Vector2(xLeft, yTop);
                vertices[1].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop);
                vertices[2].TextureCoordinate = new Vector2(xLeft, yTop + boxHeight);
                vertices[3].TextureCoordinate = new Vector2(xLeft + boxWidth, yTop + boxHeight);
            }
        }

        public override Texture2D Texture
        {
            get { return texture; }

            set
            {
                texture = value;
                if (texture != null)
                    TexRect = texture.Bounds;
            }
        }

        public override Matrix World
        {
            get
            {
                float sign = Flipped ? -1 : 1;
                return
                    Matrix.CreateScale(sign*TexRect.Width*PixelScale.X, TexRect.Height*PixelScale.Y, 1)*
                    Matrix.CreateTranslation(sign*Offset.X*PixelScale.X, Offset.Y*PixelScale.Y, 0)*
                    Matrix.CreateScale(Scale)*
                    Matrix.CreateFromQuaternion(Rotation)*
                    Matrix.CreateTranslation(Position);
            }
        }

        #endregion
        #region Ctors

        public ScaledSprite3D() : this(null, Vector3.Zero)
        {
        }

        public ScaledSprite3D(Texture2D texture, Vector3 position)
            : this(texture, null, position, Quaternion.CreateFromYawPitchRoll(0, 0, 0), Vector3.One)
        {
        }

        public ScaledSprite3D(Texture2D tex, Texture2D normalMap, Vector3 position, Quaternion rotation, Vector3 scale)
            : base(tex, normalMap, position, rotation, scale)
        {
            Texture = tex;
            Offset = Point.Zero;
            PixelScale = DefaultPixelScale;

            vertices[0] = new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.Backward, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Backward, new Vector2(1, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Backward, new Vector2(0, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Backward, new Vector2(1, 1));
        }

        #endregion
    }
}