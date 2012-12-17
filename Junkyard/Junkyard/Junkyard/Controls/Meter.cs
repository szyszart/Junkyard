using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Junkyard.Controls
{
    internal class Meter : Widget
    {
        #region Ctors

        public Meter(GameScreen screen, ContentManager content, Point position) : base(screen, content)
        {
        }

        #endregion
        #region Overrides

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        #endregion
    }
}