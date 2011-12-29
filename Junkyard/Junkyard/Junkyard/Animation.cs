using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public struct Frame
    {
        public Rectangle Rectangle;
        public Point Offset;

        public Frame(Rectangle rect, Point offset)
        {
            Rectangle = rect;
            Offset = offset;
        }
    }

    public class Animation
    {
        public const int DefaultFramesPerSecond = 25;

        public List<Frame> Frames { get; protected set; }
        public int FramesPerSecond { get; set; }
        public Texture2D SpriteSheet { get; set; }

        public Animation(Texture2D texture) : this(texture, DefaultFramesPerSecond) { }

        public Animation(Texture2D texture, int fps)
        {
            Frames = new List<Frame>();
            SpriteSheet = texture;
            FramesPerSecond = fps;
        }

        public int FrameCount
        {
            get
            {
                return Frames.Count;
            }
        }
    }
}