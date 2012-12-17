using Microsoft.Xna.Framework;

namespace Junkyard
{
    internal enum LightType
    {
        Directional,
        Point
    }

    internal class Light
    {
        #region Properties

        public float Attenuation { get; set; }
        public Color Color { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public LightType Type { get; set; }

        #endregion
        #region Ctors

        public Light(LightType type, Color color, Vector3 pos, Vector3 dir, float att)
        {
            Type = type;
            Color = color;
            Position = pos;
            Direction = dir;
            Attenuation = att;
        }

        public Light(LightType type, Color color)
        {
            Type = type;
            Color = color;
        }

        #endregion
    }
}