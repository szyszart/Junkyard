using System.Collections.Generic;
using Junkyard.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Entities.Units
{
    public class AnimatedBattleUnit : BattleUnit
    {
        #region Private fields

        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        protected string currentAnimation;
        protected int currentFrame = -1;
        protected float frameDuration;
        protected float timeElapsed = 0;

        #endregion
        #region Properties

        protected bool AnimationEnded
        {
            get
            {
                if (currentAnimation != null && Animations.ContainsKey(currentAnimation))
                {
                    return (currentFrame >= Animations[currentAnimation].FrameCount);
                }
                
                return true;
            }
        }

        #endregion
        #region Ctors

        public AnimatedBattleUnit()
        {
            Avatar = new ScaledSprite3D();
        }

        public AnimatedBattleUnit(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player)
        {
            Avatar = new ScaledSprite3D(texture, position) {Position = position};
        }

        #endregion
        #region Overrides

        public override void Draw(Effect effect)
        {
            if (currentFrame < Animations[currentAnimation].FrameCount)
            {
                Avatar.TexRect = Animations[currentAnimation].Frames[currentFrame].Rectangle;
                Avatar.Offset = Animations[currentAnimation].Frames[currentFrame].Offset;
            }
            Avatar.Draw(effect);
        }

        public override void OnTick(GameTime time)
        {
            timeElapsed += time.ElapsedGameTime.Milliseconds;
            if (timeElapsed >= frameDuration)
            {
                timeElapsed -= frameDuration;
                NextFrame();
            }

            base.OnTick(time);
        }

        #endregion
        #region Protected methods

        protected void AssertAnimation(string name)
        {
            if (currentAnimation == name)
            {
                return;
            }

            Avatar.Texture = Animations[name].SpriteSheet;
            ResetAnimation();
            currentAnimation = name;
            frameDuration = (1000.0f/Animations[currentAnimation].FramesPerSecond);
            timeElapsed = 0;
        }

        protected void NextFrame()
        {
            if (!AnimationEnded)
            {
                currentFrame++;
            }
        }

        protected void ResetAnimation()
        {
            currentFrame = 0;
        }

        #endregion
    }
}