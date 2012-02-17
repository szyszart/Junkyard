#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Junkyard.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        enum ActiveSection
        {
            Main,
            Play,
            Settings
        }
        private Texture2D mainTexture;
        private ContentManager content;
        private ActiveSection _activeSection = ActiveSection.Main;
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.            
            var playGameMenuEntry = new MenuEntry
                                              {
                                                  Text = "Graj"                                                  
                                              };

            var optionsMenuEntry = new MenuEntry
                                             {
                                                 Text = "Opcje",                                                 
                                             };
            var exitMenuEntry = new MenuEntry
                                          {
                                              Text = "Wyjscie",                                              
                                          };

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            
            menuLeft = new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.Left },
                true);

            menuRight = new InputAction(
                new Buttons[] {Buttons.DPadRight, Buttons.LeftThumbstickRight},
                new Keys[] {Keys.Right},
                true);

            //create maps list
            _maps = new List<string>
                        {
                            "test.lua",
                            "underworld.lua"
                        };
        }


        #endregion
        
        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedMap++;

                if (selectedMap >= _maps.Count)
                    selectedMap = 0;
            }

            if (_activeSection != ActiveSection.Main && menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                position = new Point(0,0);
                _activeSection = ActiveSection.Main;
            }

            if (_activeSection == ActiveSection.Main || _activeSection == ActiveSection.Play)
            {
                base.HandleInput(gameTime, input);
            }            
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (_activeSection == ActiveSection.Play)
            {
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                   new GamePlayScreen(_maps[selectedMap]));
            }
            else
            {
                _activeSection = ActiveSection.Play;
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                position = new Point(0, -viewport.Height);
            }
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            position = new Point(-viewport.Width, 0);
            _activeSection = ActiveSection.Settings;
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you siur you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            foreach (var me in MenuEntries)
            {
                me.Position = new Vector2(me.Position.X + position.X, me.Position.Y + position.Y);
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }       

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        private Point position = new Point(0,0);
        private Texture2D playTexture;
        private Texture2D settingsTexture;
        private InputAction menuLeft;
        private int selectedMap = 0;
        private List<string> _maps;
        private InputAction menuRight;

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            var menuRect = new Rectangle(position.X, position.Y, viewport.Width, viewport.Height);
            var playRect = new Rectangle(position.X,position.Y+viewport.Height,viewport.Width, viewport.Height);
            var settingsRect = new Rectangle(position.X + viewport.Width, position.Y, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(mainTexture, menuRect,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(playTexture, playRect,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(settingsTexture, settingsRect,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
           
            SpriteFont font = ScreenManager.Font;
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            spriteBatch.DrawString(font, _maps[selectedMap], new Vector2(viewport.Width*2/3, position.Y+viewport.Height*1.5f), Color.White, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                MenuEntries[0].Position = new Vector2(viewport.Width/2, (viewport.Height)/3);
                MenuEntries[1].Position = new Vector2(viewport.Width/3, (viewport.Height*2)/3);
                MenuEntries[2].Position = new Vector2((viewport.Width*2)/3, (viewport.Height*2)/3);

                mainTexture = content.Load<Texture2D>("Images/Menus/mainKabina");
                playTexture = content.Load<Texture2D>("Images/Menus/maszt01");
                settingsTexture = content.Load<Texture2D>("Images/Menus/settings");
            }
        }

        #endregion
    }
}
