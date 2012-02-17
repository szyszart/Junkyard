using GameStateManagement;
using Junkyard.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Constants

        private const int PREFERRED_HEIGHT = 768;
        private const int PREFERRED_WIDTH = 1024;

        #endregion
        #region Private fields

        private readonly GraphicsDeviceManager graphics;

        private readonly ScreenFactory screenFactory;
        private readonly ScreenManager screenManager;

        #endregion
        #region Ctors

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = PREFERRED_WIDTH;
            graphics.PreferredBackBufferHeight = PREFERRED_HEIGHT;

            // Krzysztoff has an ancient computer and that's why he needs manual resolution settings
            // If you are lucky enough to have a more powerful machine, feel free to uncomment the following lines.
            //graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
#if !DEBUG
            graphics.IsFullScreen = true;
#endif

            // disable FPS throttling
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            screenFactory = new ScreenFactory();
            Services.AddService(typeof (IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            IsMouseVisible = false;

            AddInitialScreens();
        }

        #endregion
        #region Overrides

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
        }

        #endregion
        #region Private methods

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
        }

        #endregion
    }
}