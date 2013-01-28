using GameStateManagement;
using Junkyard.Helpers;
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

        private readonly GraphicsDeviceManager _graphics;

        private readonly ScreenFactory _screenFactory;
        private readonly ScreenManager _screenManager;

        #endregion
        #region Ctors

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = PREFERRED_WIDTH;
            _graphics.PreferredBackBufferHeight = PREFERRED_HEIGHT;

            // Krzysztoff has an ancient computer and that's why he needs manual resolution settings
            // If you are lucky enough to have a more powerful machine, feel free to uncomment the following lines.
            _graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
#if !DEBUG
            _graphics.IsFullScreen = true;
#endif
            //why was it turned off?
            IsFixedTimeStep = true;
            // disable FPS throttling
            _graphics.SynchronizeWithVerticalRetrace = false;

            _screenFactory = new ScreenFactory();
            Services.AddService(typeof (IScreenFactory), _screenFactory);

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            IsMouseVisible = false;

            AddInitialScreens();
        }

        #endregion
        #region Overrides

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        #endregion
        #region Private methods

        private void AddInitialScreens()
        {
            // Activate the first screens.
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
        }

        #endregion

        protected override void LoadContent()
        {
            base.LoadContent();
            //initialize GraphicsDeciveHelper
            GraphicsDeviceHelper.Instance.Initialize(GraphicsDevice);
        }
    }
}