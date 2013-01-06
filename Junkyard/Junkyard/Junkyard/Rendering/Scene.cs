#region

using System.Collections.Generic;
using Junkyard.Camera;
using Microsoft.Xna.Framework;

#endregion

namespace Junkyard.Rendering
{
    internal class Scene
    {
        #region Private fields

        public List<Light> ShadowCastingLights;
        public List<Light> SimpleLights;

        #endregion
        #region Properties

        public Color Ambient { get; set; }
        public CameraManager CameraManager { get; set; }
        public SortedSet<Layer> Layers { get; protected set; }
        public List<Postprocess> Postprocesses { get; protected set; }
        public List<IDrawable> Unlayered { get; protected set; }

        #endregion
        #region Ctors

        public Scene()
        {
            ShadowCastingLights = new List<Light>();
            SimpleLights = new List<Light>();
            Postprocesses = new List<Postprocess>();
            Layers = new SortedSet<Layer>();
            Unlayered = new List<IDrawable>();
            Ambient = new Color(0.75f, 0.75f, 0.75f);
        }

        #endregion
    }
}