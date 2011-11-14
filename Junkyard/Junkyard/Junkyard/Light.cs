using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    enum LightType
    {
        Directional,
        Point
    }

    class Light
    {
        public LightType Type { get; set; }
        public Color Color { get; set; }        
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float Attenuation { get; set; }

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
    }
}
