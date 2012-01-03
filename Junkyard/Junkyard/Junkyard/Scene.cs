using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    class Layer : IComparable<Layer>
    {
        public float Z { get; set; }
        public List<IDrawable> Drawables { get; protected set; }

        public Layer(float z)
        {
            this.Z = z;
            Drawables = new List<IDrawable>();
        }

        public int CompareTo(Layer other)
        {
            return other.Z.CompareTo(Z);
        }
    }

    class Scene
    {
        public Camera Camera { get; set; }
        public List<Light> ShadowCastingLights;
        public List<Light> SimpleLights;
        public List<Postprocess> Postprocesses { get; protected set; }
        public SortedSet<Layer> Layers { get; protected set; }
        public List<IDrawable> Unlayered { get; protected set; }
        public Color Ambient { get; set; }

        public Scene()
        {
            ShadowCastingLights = new List<Light>();
            SimpleLights = new List<Light>();
            Postprocesses = new List<Postprocess>();
            Layers = new SortedSet<Layer>();
            Unlayered = new List<IDrawable>();
            Ambient = new Color(0.75f, 0.75f, 0.75f);
        }
    }

    interface ISceneRenderer
    {
        void Render(Scene scene);
    }

    class SimpleSceneRenderer : ISceneRenderer
    {
        public float ShadowFarPlane { get; set; }
        public float ShadowNearPlane { get; set; }
        public bool RenderShadows { get; set; }

        private Effect shadowDepthEffect, lightDepthNormalEffect,
            lightPointEffect, sceneRenderEffect;

        private RenderTarget2D shadowDepthTarget, lightDepthTarget,
            lightNormalTarget, lightPointTarget, sceneRenderTarget,
            postprocessRenderTarget;

        private Model pointLightSphere;
        private GraphicsDevice graphicsDevice;
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;

        private int viewWidth, viewHeight;        

        public SimpleSceneRenderer(GraphicsDevice device, ContentManager content)
        {
            RenderShadows = true;

            graphicsDevice = device;            

            contentManager = content;
            ShadowNearPlane = 0.1f;
            ShadowFarPlane = 1000.0f;
            
            spriteBatch = new SpriteBatch(device);

            PresentationParameters pp = device.PresentationParameters;
            viewWidth = pp.BackBufferWidth;
            viewHeight = pp.BackBufferHeight;

            // initialize render targets
            shadowDepthTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, SurfaceFormat.Single, DepthFormat.Depth24);
            lightDepthTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, SurfaceFormat.Single, DepthFormat.Depth24);
            lightNormalTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, SurfaceFormat.Color, DepthFormat.Depth24);
            lightPointTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, SurfaceFormat.Color, DepthFormat.Depth24);
            sceneRenderTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                RenderTargetUsage.DiscardContents);
            postprocessRenderTarget = new RenderTarget2D(device, viewWidth, viewHeight,
                false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            // load and initialize effects
            shadowDepthEffect = content.Load<Effect>("Effects/ShadowDepth");

            sceneRenderEffect = content.Load<Effect>("Effects/SceneRender");
            sceneRenderEffect.Parameters["ViewportWidth"].SetValue(viewWidth);
            sceneRenderEffect.Parameters["ViewportHeight"].SetValue(viewHeight);

            lightDepthNormalEffect = content.Load<Effect>("Effects/LightDepthNormal");

            lightPointEffect = content.Load<Effect>("Effects/LightPointLight");
            lightPointEffect.Parameters["ViewportWidth"].SetValue(viewWidth);
            lightPointEffect.Parameters["ViewportHeight"].SetValue(viewHeight);

            // load and initialize the sphere model (used to draw point lights)
            pointLightSphere = content.Load<Model>("Models/sphere");
            pointLightSphere.Meshes[0].MeshParts[0].Effect = lightPointEffect;
        }

        private void PrepareShadowDepthBuffer(Scene scene)
        {
            // make sure that there is only one shadow casting light
            if (scene.ShadowCastingLights.Count > 1)
                throw new ArgumentException("SimpleSceneRenderer cannot render scenes that have more than one shadow casting light.");

            graphicsDevice.SetRenderTarget(shadowDepthTarget);
            graphicsDevice.Clear(Color.White);
            graphicsDevice.BlendState = BlendState.Opaque;

            if (scene.ShadowCastingLights.Count == 0)
                return;

            Light shadowCaster = scene.ShadowCastingLights.First();
            Matrix shadowView = Matrix.CreateLookAt(shadowCaster.Position, shadowCaster.Direction, Vector3.Up);
            Matrix shadowProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, ShadowNearPlane, ShadowFarPlane);                    

            shadowDepthEffect.Parameters["View"].SetValue(shadowView);
            shadowDepthEffect.Parameters["Projection"].SetValue(shadowProjection);

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            // draw all objects that cast shadows
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(shadowDepthEffect);

            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(shadowDepthEffect);
        }

        private void PrepareLightsGBuffer(Scene scene)
        {
            graphicsDevice.SetRenderTargets(lightNormalTarget, lightDepthTarget);            
            graphicsDevice.Clear(Color.White);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;

            lightDepthNormalEffect.Parameters["View"].SetValue(scene.Camera.View);
            lightDepthNormalEffect.Parameters["Projection"].SetValue(scene.Camera.Projection);

            // draw all objects that should be lit by a point light
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(lightDepthNormalEffect);

            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(lightDepthNormalEffect);
        }

        private void DrawPointLights(Scene scene)
        {
            Matrix viewProjection = scene.Camera.View * scene.Camera.Projection;
            Matrix invViewProjection = Matrix.Invert(viewProjection);            
            lightPointEffect.Parameters["InvViewProjection"].SetValue(invViewProjection);
            
            graphicsDevice.SetRenderTarget(lightPointTarget);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DepthStencilState = DepthStencilState.None;

            lightPointEffect.Parameters["DepthTexture"].SetValue(lightDepthTarget);
            lightPointEffect.Parameters["NormalTexture"].SetValue(lightNormalTarget);

            // draw a sphere for each point light in the scene
            foreach (Light light in scene.SimpleLights)
            {
                if (light.Type != LightType.Point)
                    throw new ArgumentException("SimpleSceneRenderer can only render point lights.");

                Matrix wvp = Matrix.CreateScale(light.Attenuation) * Matrix.CreateTranslation(light.Position) * viewProjection;
                lightPointEffect.Parameters["WorldViewProjection"].SetValue(wvp);
                lightPointEffect.Parameters["CameraPosition"].SetValue(scene.Camera.Position);
                lightPointEffect.Parameters["LightColor"].SetValue(light.Color.ToVector3());
                lightPointEffect.Parameters["LightPosition"].SetValue(light.Position);
                lightPointEffect.Parameters["LightAttenuation"].SetValue(light.Attenuation);
                float dist = Vector3.Distance(scene.Camera.Position, light.Position);

                if (dist < light.Attenuation)
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                
                pointLightSphere.Meshes[0].Draw();

                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
        }
        
        private void DrawGeometry(Scene scene)
        {
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;            
            graphicsDevice.SetRenderTarget(sceneRenderTarget);

            graphicsDevice.Clear(Color.Gray);
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            sceneRenderEffect.Parameters["LightTexture"].SetValue(lightPointTarget);
            sceneRenderEffect.Parameters["AmbientColor"].SetValue(scene.Ambient.ToVector3());
            sceneRenderEffect.Parameters["View"].SetValue(scene.Camera.View);
            sceneRenderEffect.Parameters["Projection"].SetValue(scene.Camera.Projection);
            if (RenderShadows && scene.ShadowCastingLights.Count != 0)
            {
                // TODO: use the matrices that have already been computed
                Light shadowCaster = scene.ShadowCastingLights.First();
                Matrix shadowView = Matrix.CreateLookAt(shadowCaster.Position, shadowCaster.Direction, Vector3.Up);
                Matrix shadowProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, ShadowNearPlane, ShadowFarPlane);

                sceneRenderEffect.Parameters["ShadowView"].SetValue(shadowView);
                sceneRenderEffect.Parameters["ShadowProjection"].SetValue(shadowProjection);
                sceneRenderEffect.Parameters["ShadowLightPosition"].SetValue(shadowCaster.Position);
                sceneRenderEffect.Parameters["ShadowMap"].SetValue(shadowDepthTarget);
            }
                
            // draw the non-layered geometry first (Z-buffer enabled)
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            foreach (IDrawable item in scene.Unlayered)
                item.Draw(sceneRenderEffect);

            // draw the layered geometry (back-to-front order, Z-buffer read-only)
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            foreach (Layer layer in scene.Layers)
                foreach (IDrawable item in layer.Drawables)
                    item.Draw(sceneRenderEffect);                      
        }

        private RenderTarget2D DoPostprocessing(Scene scene)
        {
            RenderTarget2D current = sceneRenderTarget;
            foreach (Postprocess p in scene.Postprocesses)
            {
                if (current == sceneRenderTarget)
                    current = postprocessRenderTarget;
                else
                    current = sceneRenderTarget;
                graphicsDevice.SetRenderTarget(current);
                p.Apply();
            }
            // return the render target with the final scene
            return current;
        }

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
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, null);
            spriteBatch.Draw(finalImage, new Rectangle(0, 0, viewWidth, viewHeight), Color.White);
            spriteBatch.End();
        }
    }

}
