using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public interface IDrawable
    {
        #region Public methods

        void Draw(Effect effect);

        #endregion
    }
}