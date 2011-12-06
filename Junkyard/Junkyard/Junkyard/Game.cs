using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GameStateManagement;
using Junkyard.Screens;

namespace Junkyard
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        //#region Structures
        //private struct AnimationData
        //{
        //    public Texture2D texture;
        //    public SpriteSheetDimensions dimensions;
        //    public string name;
        //    public AnimationData(Texture2D texture, SpriteSheetDimensions dims, string name)
        //    {
        //        this.texture = texture;
        //        this.dimensions = dims;
        //        this.name = name;
        //    }
        //}
        //#endregion

        #region Fields

        private GraphicsDeviceManager graphics;       

        ScreenManager screenManager;
        ScreenFactory screenFactory;
        
        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024; // 640;
            graphics.PreferredBackBufferHeight = 768; // 480;
            
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);    
            

            AddInitialScreens();            
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

            screenManager.AddScreen(new MainMenuScreen(), null);           
        }        

        protected override void UnloadContent()
        {
        }

        #region Draw and update
        //protected override void Update(GameTime gameTime)
        //{
        //    #region temporary keybord controls
        //    //KeyboardState keyboardState = Keyboard.GetState();            
        //    //if (keyboardState.IsKeyDown(Keys.Escape))
        //    //    this.Exit();

        //    //if (keyboardState.IsKeyDown(Keys.Up))
        //    //    camera.Translate(new Vector3(0, 0, -.02f));
        //    //if (keyboardState.IsKeyDown(Keys.Down))
        //    //    camera.Translate(new Vector3(0, 0, .02f));
        //    //if (keyboardState.IsKeyDown(Keys.Left))
        //    //    camera.Position += new Vector3(-.02f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.Right))
        //    //    camera.Position += new Vector3(.02f, 0, 0);

        //    //if (keyboardState.IsKeyDown(Keys.Home))
        //    //    camera.Pitch += .01f;
        //    //if (keyboardState.IsKeyDown(Keys.End))
        //    //    camera.Pitch -= .01f;

        //    //if (keyboardState.IsKeyDown(Keys.PageDown))
        //    //    camera.Translate(new Vector3(0, -.02f, 0));
        //    //if (keyboardState.IsKeyDown(Keys.PageUp))
        //    //    camera.Translate(new Vector3(0, .02f, 0));

        //    //if (keyboardState.IsKeyDown(Keys.W))
        //    //    Lights[0].Position += new Vector3(0, 0, 0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.S))
        //    //    Lights[0].Position += new Vector3(0, 0, -0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.A))
        //    //    Lights[0].Position += new Vector3(-0.1f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.D))
        //    //    Lights[0].Position += new Vector3(0.1f, 0, 0);

        //    ////if (keyboardState.IsKeyDown(Keys.I))
        //    ////    guy.Position += new Vector3(0, 0, 0.1f);
        //    ////if (keyboardState.IsKeyDown(Keys.K))
        //    ////    guy.Position += new Vector3(0, 0, -0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.J))
        //    //    guy.Position += new Vector3(-0.1f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.L))
        //    //    guy.Position += new Vector3(0.1f, 0, 0);

        //    //if (keyboardState.IsKeyDown(Keys.Z))
        //    //    camera.Yaw += .01f;
        //    //if (keyboardState.IsKeyDown(Keys.X))
        //    //    camera.Yaw -= .01f;

        //    //if (keyboardState.IsKeyDown(Keys.Delete))
        //    //    sceneRenderer.RenderShadows = !sceneRenderer.RenderShadows;
        //    #endregion

        //    //temporary gamepad controls
        //    var gPadState = input.GetGamePad(ExtendedPlayerIndex.Five).GetState();            
        //    var thumbSticks = gPadState.ThumbSticks;
        //    if (Math.Abs(thumbSticks.Left.Y) > 0.1f)
        //        camera.Translate(new Vector3(0, 0, -thumbSticks.Left.Y/3));
        //    if (Math.Abs(thumbSticks.Left.X) > 0.1f)
        //        camera.Position += new Vector3(thumbSticks.Left.X/3, 0, 0);
            

        //    timeFromAnimationChange += gameTime.ElapsedGameTime.Milliseconds;
        //    if (timeFromAnimationChange >= animationChangeDelay)
        //    {
        //        if (keyboardState.IsKeyDown(Keys.Space) || gPadState.IsButtonDown(Buttons.X))
        //        {
        //            timeFromAnimationChange = 0;
        //            currentAnimation++;
        //            if (currentAnimation >= animations.Count)
        //                currentAnimation = 0;

        //            guy.Texture = animations[currentAnimation].texture;
        //            guy.gridDimensions = animations[currentAnimation].dimensions;
        //            guy.Reset();
        //        }
        //    }

        //    frameTime += gameTime.ElapsedGameTime.Milliseconds;
        //    if (frameTime >= timePerFrame)
        //    {
        //        frameTime -= timePerFrame;
        //        guy.NextFrame();
        //    }

        //    camera.Update();

        //    elapsedTime += gameTime.ElapsedGameTime;
        //    if (elapsedTime > TimeSpan.FromSeconds(1))
        //    {
        //        elapsedTime -= TimeSpan.FromSeconds(1);
        //        frameRate = frameCounter;
        //        frameCounter = 0;
        //    }

        //    #region temporary stupid logic
        //    foreach (Layer layer in scene.Layers)
        //    {
        //        layer.Drawables.ForEach(x => { if (x != guy) ((Sprite3D)x).Position += new Vector3(0.03f, 0.0f, 0.0f); });
        //        layer.Drawables.RemoveAll(x => ((Sprite3D)x).Distance(ship2) < 1.5);                
        //    }
        //    #endregion

        //    camera.Update();            //czemu dwa razy camera.Update() ?
        //    base.Update(gameTime);
        //}

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            //sceneRenderer.Render(scene);

            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, animations[currentAnimation].name, Vector2.Zero, Color.White);
            //string info = string.Format("cam: {0}, yaw: {1}", camera.Position.ToString(), camera.Yaw);
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 20), Color.White);
            //info = string.Format("FPS: {0}, guy: {1}", frameRate, guy.Position.ToString());
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 40), Color.White);

            //spriteBatch.End();

            base.Draw(gameTime);
            //frameCounter++;
        }
        #endregion
    }
}
