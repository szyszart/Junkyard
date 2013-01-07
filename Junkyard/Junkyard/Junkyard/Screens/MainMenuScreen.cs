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
using Junkyard.Localization;
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
    internal class MainMenuScreen : MenuScreen
    {
        #region Enums

        private enum ActiveSection
        {
            Main,
            Play,
            Settings
        }

        #endregion
        #region Private fields

        private ActiveSection _activeSection = ActiveSection.Main;
        private ContentManager _content;
        private Texture2D _mainTexture;

        private readonly List<string> _maps;
        private readonly InputAction _menuLeft;
        private InputAction _menuRight;
        private Texture2D _playTexture;
        private Point _position = new Point(0, 0);
        private int _selectedMap;
        private Texture2D _settingsTexture;

        #endregion
        #region Ctors

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.            
            var playGameMenuEntry = new MenuEntry
                                        {
                                            Text = LR.Play
                                        };

            var optionsMenuEntry = new MenuEntry
                                       {
                                           Text = LR.Settings,
                                       };
            var exitMenuEntry = new MenuEntry
                                    {
                                        Text = LR.Exit,
                                    };

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);


            _menuLeft = new InputAction(
                new[] {Buttons.DPadLeft, Buttons.LeftThumbstickLeft},
                new[] {Keys.Left},
                true);

            _menuRight = new InputAction(
                new[] {Buttons.DPadRight, Buttons.LeftThumbstickRight},
                new[] {Keys.Right},
                true);

            //create maps list
            _maps = new List<string>
                        {
                            "test.lua",
                            "underworld.lua"
                        };
        }

        #endregion
        #region Event handlers

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _position = new Point(-viewport.Width, 0);
            _activeSection = ActiveSection.Settings;
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (_activeSection == ActiveSection.Play)
            {
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                   new GamePlayScreen(_maps[_selectedMap]));
            }
            else
            {
                _activeSection = ActiveSection.Play;
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                _position = new Point(0, -viewport.Height);
            }
        }

        #endregion
        #region Overrides

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (_content == null)
                {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }


                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                MenuEntries[0].Position = new Vector2(viewport.Width/2, (viewport.Height)/3);
                MenuEntries[1].Position = new Vector2(viewport.Width/3, (viewport.Height*2)/3);
                MenuEntries[2].Position = new Vector2((viewport.Width*2)/3, (viewport.Height*2)/3);

                _mainTexture = _content.Load<Texture2D>("Images/Menus/mainKabina");
                _playTexture = _content.Load<Texture2D>("Images/Menus/maszt01");
                _settingsTexture = _content.Load<Texture2D>("Images/Menus/settings");
            }
        }

        protected override void UpdateMenuEntryLocations()
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            var menuRect = new Rectangle(_position.X, _position.Y, viewport.Width, viewport.Height);
            var playRect = new Rectangle(_position.X, _position.Y + viewport.Height, viewport.Width, viewport.Height);
            var settingsRect = new Rectangle(_position.X + viewport.Width, _position.Y, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(_mainTexture, menuRect,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(_playTexture, playRect,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(_settingsTexture, settingsRect,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            SpriteFont font = ScreenManager.Font;
            var origin = new Vector2(0, font.LineSpacing/2);
            spriteBatch.DrawString(font, _maps[_selectedMap],
                                   new Vector2(viewport.Width*2/3, _position.Y + viewport.Height*1.5f), Color.White, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                _selectedMap++;

                if (_selectedMap >= _maps.Count)
                    _selectedMap = 0;
            }

            if (_activeSection != ActiveSection.Main && menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                _position = new Point(0, 0);
                _activeSection = ActiveSection.Main;
            }

            if (_activeSection == ActiveSection.Main || _activeSection == ActiveSection.Play)
            {
                base.HandleInput(gameTime, input);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            foreach (MenuEntry me in MenuEntries)
            {
                me.Position = new Vector2(me.Position.X + _position.X, me.Position.Y + _position.Y);
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you siur you want to exit?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        #endregion
    }
}