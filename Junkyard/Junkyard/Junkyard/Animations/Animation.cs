using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Animations
{
    public class Animation
    {
        #region Constants

        public const int DefaultFramesPerSecond = 25;

        #endregion
        #region Properties

        public int FrameCount
        {
            get { return Frames.Count; }
        }

        public List<Frame> Frames { get; protected set; }
        public int FramesPerSecond { get; set; }
        public Texture2D SpriteSheet { get; set; }

        #endregion
        #region Ctors

        public Animation(Texture2D texture) : this(texture, DefaultFramesPerSecond)
        {
        }

        public Animation(Texture2D texture, int fps)
        {
            Frames = new List<Frame>();
            SpriteSheet = texture;
            FramesPerSecond = fps;
        }

        #endregion
    }
}