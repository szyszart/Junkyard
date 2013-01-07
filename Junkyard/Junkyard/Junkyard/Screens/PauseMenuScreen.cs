#region File Description

//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion
#region Using Statements



#endregion
using Junkyard.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Screens
{
    /// <summary>
    ///   The pause menu comes up over the top of the game,
    ///   giving the player options to resume or quit.
    /// </summary>
    internal class PauseMenuScreen : MenuScreen
    {
        #region Ctors

        /// <summary>
        ///   Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            var resumeGameMenuEntry = new MenuEntry("Resume Game");
            var quitGameMenuEntry = new MenuEntry("Quit Game");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        #endregion
        #region Event handlers

        /// <summary>
        ///   Event handler for when the user selects ok on the "are you sure
        ///   you want to quit" message box. This uses the loading screen to
        ///   transition from the game back to the main menu screen.
        /// </summary>
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                               new MainMenuScreen());
        }

        /// <summary>
        ///   Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var confirmQuitMessageBox = new MessageBoxScreen(LR.ConfirmExit);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        #endregion

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            var initPosition = new Point(viewport.Width / 2, viewport.Height / 2);
            foreach (MenuEntry me in MenuEntries)
            {
                me.Position = new Vector2(me.Position.X + initPosition.X, me.Position.Y + initPosition.Y);
            }
        }
    }
}