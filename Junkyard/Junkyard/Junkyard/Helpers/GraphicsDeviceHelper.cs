#region

using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Junkyard.Helpers
{
    public class GraphicsDeviceHelper
    {
        #region Private fields

        private static GraphicsDeviceHelper _instance;

        #endregion
        #region Properties

        public static GraphicsDeviceHelper Instance
        {
            get { return _instance ?? (_instance = new GraphicsDeviceHelper()); }
        }

        public float CurrentAspectRatio
        {
            get
            {
                Debug.Assert(GraphicsDevice != null);
                return GraphicsDevice.Viewport.AspectRatio;
            }
        }

        public int CurrentViewportHeight
        {
            get
            {
                Debug.Assert(GraphicsDevice != null);
                return GraphicsDevice.Viewport.Height;
            }
        }

        public int CurrentViewportWidth
        {
            get
            {
                Debug.Assert(GraphicsDevice != null);
                return GraphicsDevice.Viewport.Width;
            }
        }

        public GraphicsDevice GraphicsDevice { get; set; }

        #endregion
        #region Ctors

        private GraphicsDeviceHelper()
        {
        }

        #endregion
        #region Public methods

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        #endregion
    }
}