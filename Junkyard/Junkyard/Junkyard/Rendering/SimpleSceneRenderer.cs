#region

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Junkyard.Rendering
{
    internal class SimpleSceneRenderer : ISceneRenderer
    {
        #region Private fields

        private ContentManager _contentManager;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Effect _lightDepthNormalEffect;

        private readonly RenderTarget2D _lightDepthTarget;

        private readonly RenderTarget2D _lightNormalTarget;
        private readonly Effect _lightPointEffect;

        private readonly RenderTarget2D _lightPointTarget;

        private readonly Model _pointLightSphere;
        private readonly RenderTarget2D _postprocessRenderTarget;
        private readonly Effect _sceneRenderEffect;
        private readonly RenderTarget2D _sceneRenderTarget;
        private readonly Effect _shadowDepthEffect;
        private readonly RenderTarget2D _shadowDepthTarget;
        private readonly SpriteBatch _spriteBatch;

        private readonly int _viewHeight;
        private readonly int _viewWidth;

        #endregion
        #region Properties

        public bool RenderShadows { get; set; }
        public float ShadowFarPlane { get; set; }
        public float ShadowNearPlane { get; set; }

        #endregion
        #region Ctors

        public SimpleSceneRenderer(GraphicsDevice device, ContentManager content)
        {
            RenderShadows = true;

            _graphicsDevice = device;

            _contentManager = content;
            ShadowNearPlane = 0.1f;
            ShadowFarPlane = 1000.0f;

            _spriteBatch = new SpriteBatch(device);

            PresentationParameters pp = device.PresentationParameters;
            _viewWidth = pp.BackBufferWidth;
            _viewHeight = pp.BackBufferHeight;

            // initialize render targets
            _shadowDepthTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                   false, SurfaceFormat.Single, DepthFormat.Depth24);
            _lightDepthTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                  false, SurfaceFormat.Single, DepthFormat.Depth24);
            _lightNormalTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                   false, SurfaceFormat.Color, DepthFormat.Depth24);
            _lightPointTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                  false, SurfaceFormat.Color, DepthFormat.Depth24);
            _sceneRenderTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                   false, pp.BackBufferFormat, pp.DepthStencilFormat,
                                                   pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);
            _postprocessRenderTarget = new RenderTarget2D(device, _viewWidth, _viewHeight,
                                                         false, pp.BackBufferFormat, pp.DepthStencilFormat,
                                                         pp.MultiSampleCount,
                                                         RenderTargetUsage.DiscardContents);

            // load and initialize effects
            _shadowDepthEffect = content.Load<Effect>("Effects/ShadowDepth");

            _sceneRenderEffect = content.Load<Effect>("Effects/SceneRender");
            _sceneRenderEffect.Parameters["ViewportWidth"].SetValue(_viewWidth);
            _sceneRenderEffect.Parameters["ViewportHeight"].SetValue(_viewHeight);

            _lightDepthNormalEffect = content.Load<Effect>("Effects/LightDepthNormal");

            _lightPointEffect = content.Load<Effect>("Effects/LightPointLight");
            _lightPointEffect.Parameters["ViewportWidth"].SetValue(_viewWidth);
            _lightPointEffect.Parameters["ViewportHeight"].SetValue(_viewHeight);

            // load and initialize the sphere model (used to draw point lights)
            _pointLightSphere = content.Load<Model>("Models/sphere");
            _pointLightSphere.Meshes[0].MeshParts[0].Effect = _lightPointEffect;
        }

        #endregion
        #region Private methods

        private RenderTarget2D DoPostprocessing(Scene scene)
        {
            RenderTarget2D current = _sceneRenderTarget;
            foreach (Postprocess p in scene.Postprocesses)
            {
                current = current == _sceneRenderTarget ? _postprocessRenderTarget : _sceneRenderTarget;
                _graphicsDevice.SetRenderTarget(current);
                p.Apply();
            }
            // return the render target with the final scene
            return current;
        }

        private void DrawGeometry(Scene scene)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.SetRenderTarget(_sceneRenderTarget);

            _graphicsDevice.Clear(Color.Gray);
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;

            _sceneRenderEffect.Parameters["LightTexture"].SetValue(_lightPointTarget);
            _sceneRenderEffect.Parameters["AmbientColor"].SetValue(scene.Ambient.ToVector3());
            _sceneRenderEffect.Parameters["View"].SetValue(scene.CameraManager.Camera.View);
            _sceneRenderEffect.Parameters["Projection"].SetValue(scene.CameraManager.Camera.Projection);
            if (RenderShadows && scene.ShadowCastingLights.Count != 0)
            {
                // TODO: use the matrices that have already been computed
                Light shadowCaster = scene.ShadowCastingLights.First();
                Matrix shadowView = Matrix.CreateLookAt(shadowCaster.Position, shadowCaster.Direction, Vector3.Up);
                Matrix shadowProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1,
                                                                              ShadowNearPlane, ShadowFarPlane);

                _sceneRenderEffect.Parameters["ShadowView"].SetValue(shadowView);
                _sceneRenderEffect.Parameters["ShadowProjection"].SetValue(shadowProjection);
                _sceneRenderEffect.Parameters["ShadowLightPosition"].SetValue(shadowCaster.Position);
                _sceneRenderEffect.Parameters["ShadowMap"].SetValue(_shadowDepthTarget);
            }

            // draw the non-layered geometry first (Z-buffer enabled)
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(_sceneRenderEffect);

            // draw the layered geometry (back-to-front order, Z-buffer read-only)
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(_sceneRenderEffect);
        }

        private void DrawPointLights(Scene scene)
        {
            Matrix viewProjection = scene.CameraManager.Camera.View*scene.CameraManager.Camera.Projection;
            Matrix invViewProjection = Matrix.Invert(viewProjection);
            _lightPointEffect.Parameters["InvViewProjection"].SetValue(invViewProjection);

            _graphicsDevice.SetRenderTarget(_lightPointTarget);
            _graphicsDevice.Clear(Color.Black);
            _graphicsDevice.BlendState = BlendState.Additive;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;

            _lightPointEffect.Parameters["DepthTexture"].SetValue(_lightDepthTarget);
            _lightPointEffect.Parameters["NormalTexture"].SetValue(_lightNormalTarget);

            // draw a sphere for each point light in the scene
            foreach (Light light in scene.SimpleLights)
            {
                if (light.Type != LightType.Point)
                    throw new ArgumentException("SimpleSceneRenderer can only render point lights.");

                Matrix wvp = Matrix.CreateScale(light.Attenuation)*Matrix.CreateTranslation(light.Position)*
                             viewProjection;
                _lightPointEffect.Parameters["WorldViewProjection"].SetValue(wvp);
                _lightPointEffect.Parameters["CameraPosition"].SetValue(scene.CameraManager.Camera.Position);
                _lightPointEffect.Parameters["LightColor"].SetValue(light.Color.ToVector3());
                _lightPointEffect.Parameters["LightPosition"].SetValue(light.Position);
                _lightPointEffect.Parameters["LightAttenuation"].SetValue(light.Attenuation);
                float dist = Vector3.Distance(scene.CameraManager.Camera.Position, light.Position);

                if (dist < light.Attenuation)
                    _graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                _pointLightSphere.Meshes[0].Draw();

                _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
        }

        private void PrepareLightsGBuffer(Scene scene)
        {
            _graphicsDevice.SetRenderTargets(_lightNormalTarget, _lightDepthTarget);
            _graphicsDevice.Clear(Color.White);
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.BlendState = BlendState.Opaque;

            _lightDepthNormalEffect.Parameters["View"].SetValue(scene.CameraManager.Camera.View);
            _lightDepthNormalEffect.Parameters["Projection"].SetValue(scene.CameraManager.Camera.Projection);

            // draw all objects that should be lit by a point light
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(_lightDepthNormalEffect);

            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(_lightDepthNormalEffect);
        }

        private void PrepareShadowDepthBuffer(Scene scene)
        {
            // make sure that there is only one shadow casting light
            if (scene.ShadowCastingLights.Count > 1)
                throw new ArgumentException(
                    "SimpleSceneRenderer cannot render scenes that have more than one shadow casting light.");

            _graphicsDevice.SetRenderTarget(_shadowDepthTarget);
            _graphicsDevice.Clear(Color.White);
            _graphicsDevice.BlendState = BlendState.Opaque;

            if (scene.ShadowCastingLights.Count == 0)
                return;

            Light shadowCaster = scene.ShadowCastingLights.First();
            Matrix shadowView = Matrix.CreateLookAt(shadowCaster.Position, shadowCaster.Direction, Vector3.Up);
            Matrix shadowProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, ShadowNearPlane,
                                                                          ShadowFarPlane);

            _shadowDepthEffect.Parameters["View"].SetValue(shadowView);
            _shadowDepthEffect.Parameters["Projection"].SetValue(shadowProjection);

            _graphicsDevice.RasterizerState = RasterizerState.CullNone;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;
            // draw all objects that cast shadows
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(_shadowDepthEffect);

            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(_shadowDepthEffect);
        }

        #endregion
        #region ISceneRenderer Members

        public void Render(Scene scene)
        {
            // Step 1: shadow depth buffer
            if (RenderShadows)
                PrepareShadowDepthBuffer(scene);

            // Step 2: point lights G-buffer (depth and normals)
            PrepareLightsGBuffer(scene);

            // Step 3: draw the point lights
            DrawPointLights(scene);

            // Step 4: draw the geometry (combining it with the lighting data)
            // Revert the blending and depth render states
            DrawGeometry(scene);

            // Step 5: do the postprocessing
            RenderTarget2D finalImage = DoPostprocessing(scene);

            // Step 6: blit the final image
            _graphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, null);
            _spriteBatch.Draw(finalImage, new Rectangle(0, 0, _viewWidth, _viewHeight), Color.White);
            _spriteBatch.End();
        }

        #endregion
    }
}