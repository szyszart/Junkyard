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
        #region Fields

        private const int preferredWidth = 1024, preferredHeight = 768;
        private GraphicsDeviceManager graphics;       

        ScreenManager screenManager;
        ScreenFactory screenFactory;
        
        #endregion

        public Game()
        {            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = preferredWidth;
            graphics.PreferredBackBufferHeight = preferredHeight;
                      
            // disable FPS throttling
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);            

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
            
            this.IsMouseVisible = true;
            
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);         
            base.Draw(gameTime);            
        }
        #endregion
    }
}
