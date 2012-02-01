#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion
#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using GameStateManagement;
using Junkyard.Entities;
using Junkyard.Helpers;
using Junkyard.Particles;
using LuaInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Junkyard.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    internal class GamePlayScreen : GameScreen
    {
        #region Constants

        private const int DUST_PARTICLE_SYSTEM = 1;
        private const string UnitsDirectory = "Images\\Units";

        #endregion
        #region Private fields

        private readonly List<Light> Lights = new List<Light>
                                                  {
                                                      new Light(LightType.Point, Color.Red,
                                                                new Vector3(-2.3f, -0.7f, -0.6f), Vector3.Zero, 0.8f),
                                                      new Light(LightType.Point, Color.Green,
                                                                new Vector3(-1.0f, -0.1f, -0.6f), Vector3.Zero, 0.8f),
                                                      new Light(LightType.Point, Color.Pink,
                                                                new Vector3(-2.0f, -1.2f, -0.6f), Vector3.Zero, 0.9f),
                                                      new Light(LightType.Point, Color.Pink,
                                                                new Vector3(1.0f, -0.1f, -0.2f), Vector3.Zero, 0.8f),
                                                  };

        private readonly InputAction _pauseAction;
        private float _pauseAlpha;

        private FreeCamera camera;
        private ContentManager content;

        private TimeSpan elapsedTime = TimeSpan.Zero;        
        private int frameCounter;
        private int frameRate;
        private Player player1, player2;

        private PuzzleBoardWidget puzzleBoard1;
        private PuzzleBoardWidget puzzleBoard2;
        private Scene scene;
        private SimpleSceneRenderer sceneRenderer;

        private Simulation simulation;
        private SpriteFont spriteFont;
        private ParticleManager _particleManager;

        #endregion
        #region Ctors

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] {Buttons.Start, Buttons.Back},
                new[] {Keys.Escape},
                true);
        }

        #endregion
        #region Event handlers

        private void BattleOverScreenConfirmed(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                               new MainMenuScreen());
        }

        private void LayoutAccepted(Widget src, LayoutInstance instance)
        {
            PuzzleBoardWidget widget = (PuzzleBoardWidget)src;
            Player who = widget.Board.Player;

            // spawn a new unit
            BattleUnit unit = simulation.Spawn(instance.Layout.Name);
            unit.Simulation = simulation;
            unit.Player = who;
            unit.Avatar.Position = who.InitialPosition;
            simulation.Add(unit);
        }

        #endregion
        #region Overrides

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                {
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                }
                sceneRenderer = new SimpleSceneRenderer(ScreenManager.GraphicsDevice, content);
                sceneRenderer.RenderShadows = false;

                PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
                float aspectRatio = pp.BackBufferWidth/(float) pp.BackBufferHeight;
                camera = new FreeCamera(new Vector3(-2.2f, 0.36f, 3.8f), MathHelper.ToRadians(45), aspectRatio, 0.1f,
                                        1000.0f);

                scene = new Scene();
                scene.Camera = camera;
                scene.Ambient = new Color(0.7f, 0.7f, 0.7f);

                spriteFont = content.Load<SpriteFont>("Fonts/sample");

                var dirLight = new Light(LightType.Directional, Color.Red);
                dirLight.Direction = new Vector3(0.45f, -0.15f, 0.875f);
                dirLight.Position = new Vector3(5.6f, 7.6f, 12.0f);
                scene.ShadowCastingLights.Add(dirLight);                

                InitializeScene();               

                // add the debug puzzleboard control
                int boardMargin = 15;
                int boardWidth = (pp.BackBufferWidth - 3*boardMargin)/3;
                int boardHeight = 5*boardWidth/7;

                var finder = new BoardLayoutFinder();
                ProcessLayouts(finder);

                var board1 = new PuzzleBoard(6, 5);
                board1.Player = player1;
                board1.Randomize();
                puzzleBoard1 = new PuzzleBoardWidget(this, content, new Point(boardMargin, boardMargin),
                                                     new Point(boardWidth, boardHeight));
                puzzleBoard1.LayoutFinder = finder;
                puzzleBoard1.Board = board1;
                puzzleBoard1.LayoutAccepted += LayoutAccepted;

                var board2 = new PuzzleBoard(6, 5);
                board2.Player = player2;
                board2.Randomize();
                puzzleBoard2 = new PuzzleBoardWidget(this, content,
                                                     new Point(pp.BackBufferWidth - boardWidth - 2*boardMargin,
                                                               boardMargin), new Point(boardWidth, boardHeight));
                puzzleBoard2.LayoutFinder = finder;
                puzzleBoard2.Board = board2;

                puzzleBoard2.LayoutAccepted += LayoutAccepted;

                _particleManager = new ParticleManager((Game)ScreenManager.Game);
                _particleManager.AddSystem<DustParticleSystem>(DUST_PARTICLE_SYSTEM, 10, player1.InitialPosition.Z + 0.1f);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            sceneRenderer.Render(scene);
            //SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, camera.Position.ToString(), new Vector2(0, 0), Color.White);
            //spriteBatch.End();

            puzzleBoard1.Draw(gameTime);
            puzzleBoard2.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha/2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
            frameCounter++;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            var playerIndex = (int) ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

            playerIndex = input.CurrentGamePadStates[playerIndex + 4].IsConnected ? playerIndex + 4 : playerIndex;
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise handle input.                
                #region temporary keybord controls                                               

                //if (keyboardState.IsKeyDown(Keys.I))
                //    camera.Translate(new Vector3(0, 0, -.02f));                    
                //    //unit.Avatar.Flipped = !unit.Avatar.Flipped;

                //if (keyboardState.IsKeyDown(Keys.K))
                //    camera.Translate(new Vector3(0, 0, .02f));
                //if (keyboardState.IsKeyDown(Keys.J))
                //    camera.Position += new Vector3(-.02f, 0, 0);
                //if (keyboardState.IsKeyDown(Keys.L))
                //    camera.Position += new Vector3(.02f, 0, 0);

                if (keyboardState.IsKeyDown(Keys.Home))
                    camera.Pitch += .01f;
                if (keyboardState.IsKeyDown(Keys.End))
                    camera.Pitch -= .01f;

                if (keyboardState.IsKeyDown(Keys.PageDown))
                    camera.Translate(new Vector3(0, -.02f, 0));
                if (keyboardState.IsKeyDown(Keys.PageUp))
                    camera.Translate(new Vector3(0, .02f, 0));

                // temporary camera controls
                if (input.IsKeyPressed(Keys.I, null, out player))
                    camera.Translate(Vector3.UnitZ*-0.1f);
                if (input.IsKeyPressed(Keys.K, null, out player))
                    camera.Translate(Vector3.UnitZ*0.1f);
                if (input.IsKeyPressed(Keys.J, null, out player))
                    camera.Translate(Vector3.UnitX*-0.1f);
                if (input.IsKeyPressed(Keys.L, null, out player))
                    camera.Translate(Vector3.UnitX*0.1f);

                // player 1 temporary controls)););)
                if (input.IsNewKeyPress(Keys.W, null, out player))
                    puzzleBoard1.Up();
                if (input.IsNewKeyPress(Keys.S, null, out player))
                    puzzleBoard1.Down();
                if (input.IsNewKeyPress(Keys.A, null, out player))
                    puzzleBoard1.Left();
                if (input.IsNewKeyPress(Keys.D, null, out player))
                    puzzleBoard1.Right();

                if (input.IsNewKeyPress(Keys.Q, null, out player))
                    puzzleBoard1.Select();
                if (input.IsNewKeyPress(Keys.E, null, out player))
                    puzzleBoard1.Accept();
                if (input.IsNewKeyPress(Keys.R, null, out player))
                    puzzleBoard1.Randomize();

                // player 2 temporary controls
                if (input.IsNewKeyPress(Keys.Up, null, out player))
                    puzzleBoard2.Up();
                if (input.IsNewKeyPress(Keys.Down, null, out player))
                    puzzleBoard2.Down();
                if (input.IsNewKeyPress(Keys.Left, null, out player))
                    puzzleBoard2.Left();
                if (input.IsNewKeyPress(Keys.Right, null, out player))
                    puzzleBoard2.Right();
                if (input.IsNewKeyPress(Keys.RightControl, null, out player))
                    puzzleBoard2.Select();
                if (input.IsNewKeyPress(Keys.Enter, null, out player))
                    puzzleBoard2.Accept();
                if (input.IsNewKeyPress(Keys.Back, null, out player))
                    puzzleBoard2.Randomize();

                if (keyboardState.IsKeyDown(Keys.Z))
                    camera.Yaw += .01f;
                if (keyboardState.IsKeyDown(Keys.X))
                    camera.Yaw -= .01f;

                // DEBUG, remove ASAP
                //if (input.IsNewKeyPress(Keys.D1, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menele_ranged");
                //    unit.Simulation = simulation;
                //    unit.Player = player1;
                //    unit.Avatar.Position = player1.InitialPosition;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D2, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_ram");
                //    unit.Simulation = simulation;
                //    unit.Player = player1;
                //    unit.Avatar.Position = player1.InitialPosition;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D3, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menele_ranged");
                //    unit.Simulation = simulation;
                //    unit.Player = player2;
                //    unit.Avatar.Position = player2.InitialPosition;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D4, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_infantry");
                //    unit.Simulation = simulation;
                //    unit.Player = player2;
                //    unit.Avatar.Position = player2.InitialPosition;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D5, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_boar");
                //    unit.Simulation = simulation;
                //    unit.Player = player1;
                //    unit.Avatar.Position = player1.InitialPosition + 6 * Vector3.UnitY;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D6, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_boar");
                //    unit.Simulation = simulation;
                //    unit.Player = player2;
                //    unit.Avatar.Position = player2.InitialPosition + 6 * Vector3.UnitY;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D7, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_infantry");
                //    unit.Simulation = simulation;
                //    unit.Player = player2;
                //    unit.Avatar.Position = player2.InitialPosition;
                //    simulation.Add(unit);
                //}
                //if (input.IsNewKeyPress(Keys.D8, null, out player))
                //{
                //    BattleUnit unit = (BattleUnit)simulation.Spawn("menel_infantry");
                //    unit.Simulation = simulation;
                //    unit.Player = player2;
                //    unit.Avatar.Position = player2.InitialPosition;
                //    simulation.Add(unit);
                //}
                // END OF DEBUG

                if (keyboardState.IsKeyDown(Keys.Delete))
                    sceneRenderer.RenderShadows = !sceneRenderer.RenderShadows;

                #endregion
                #region Gamepad controls                               

                Vector2 tsPos = gamePadState.ThumbSticks.Left;

                if (Math.Abs(tsPos.X) > 0.1f || Math.Abs(tsPos.Y) > 0.1f)
                    camera.Translate(new Vector3(tsPos, 0.0f));

                if (gamePadState.IsButtonDown(Buttons.DPadUp))
                    puzzleBoard1.Up();
                if (gamePadState.IsButtonDown(Buttons.DPadDown))
                    puzzleBoard1.Down();
                if (gamePadState.IsButtonDown(Buttons.DPadLeft))
                    puzzleBoard1.Left();
                if (gamePadState.IsButtonDown(Buttons.DPadRight))
                    puzzleBoard1.Right();

                if (gamePadState.IsButtonDown(Buttons.X))
                    puzzleBoard1.Select();
                if (gamePadState.IsButtonDown(Buttons.Y))
                    puzzleBoard1.Accept();
                if (gamePadState.IsButtonDown(Buttons.A))
                    puzzleBoard1.Randomize();

                #endregion
                simulation.Tick(gameTime);

                // FPS measurement
                elapsedTime += gameTime.ElapsedGameTime;
                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }

                camera.Update();
            }
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f/32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f/32, 0);

            if (!IsActive) return;
            #region temporary stupid logic

            if (player1.Hp == 0 || player2.Hp == 0)
            {
                var winner = player2.Hp == 0 ? "Player 1" : "Player 2";
                var message = winner + " has won!";

                var battleOverMessage = new BattleOverScreen(message, false);

                battleOverMessage.Accepted += BattleOverScreenConfirmed;
                battleOverMessage.Cancelled += BattleOverScreenConfirmed;

                ScreenManager.AddScreen(battleOverMessage, ControllingPlayer);
            }

            puzzleBoard1.Update(gameTime);
            puzzleBoard2.Update(gameTime);

            //if (gameTime.TotalGameTime.Milliseconds % 259 == 0 && simulation.Units.Count != 0)
            //{
            //    var randomUnitPos = simulation.Units.GetRandomElement().Avatar.Position;
            //    _particleManager[DUST_PARTICLE_SYSTEM].AddParticles(new Vector2(randomUnitPos.X,randomUnitPos.Y));
            //}
            #endregion
        }

        #endregion
        #region Protected methods

        protected BattleUnitFactory CreateFactory(string kind)
        {
            switch (kind)
            {
                case "infantry":
                    return new InfantryFactory();
                case "projectile":
                    return new ProjectileFactory();
                case "ranged":
                    return new RangedFactory();
                case "meteor":
                    return new MeteorFactory();
                default:
                    throw new ArgumentException("Unknown battle unit type.");
            }
        }

        protected void DoUnitLoading()
        {
            Lua lua = LuaMachine.Instance;
            string unitsPath = Path.Combine(content.RootDirectory, UnitsDirectory);
            foreach (string dir in Directory.GetDirectories(unitsPath, "*", SearchOption.AllDirectories))
            {
                foreach (string file in Directory.GetFiles(dir, "*.lua"))
                {
                    object[] retVals = lua.DoFile(file);
                    var unitData = (LuaTable) retVals[0];

                    var name = (string) unitData["name"];
                    var kind = (string) unitData["class"];

                    // process the animations
                    var animationData = (LuaTable) unitData["animations"];
                    Dictionary<string, Animation> animations = ProcessAnimations(name, animationData);

                    BattleUnitFactory factory = CreateFactory(kind);
                    factory.Params = unitData;
                    factory.Animations = animations;
                    simulation.FactoryDispatcher.RegisterFactory(name, factory);                    
                }
            }
        }

        protected void InitializeScene()
        {
            simulation = new Simulation();
            simulation.GroundLevel = -0.4f;

            DoUnitLoading();            

            var layer = new Layer(-1.25f);
            layer.Drawables.Add(simulation);

            scene.Layers.Add(layer);

            // TODO: remove hardcoded paths
            //string[] files = System.IO.Directory.GetFiles("Content/Maps", "*.lua");
            Lua lua = LuaMachine.Instance;
            object[] luaResult = lua.DoFile("Content/Maps/test.lua");
            var tbl = (LuaTable) luaResult[0];

            foreach (LuaTable el in tbl.Values)
            {
                Sprite3D asset = LuaMachine.LoadAsset(el, content);
                scene.Unlayered.Add(asset);
            }

            //var ships = (LuaTable)luaResult["statki"];
            var ships = (LuaTable) lua["statki"];

            Sprite3D ship1 = LuaMachine.LoadAsset((LuaTable) ships["ship1"], content);
            scene.Unlayered.Add(ship1);

            Sprite3D ship2 = LuaMachine.LoadAsset((LuaTable) ships["ship2"], content);
            scene.Unlayered.Add(ship2);

            player1 = new Player("p1");            
            player1.InitialPosition = new Vector3(ship1.Position.X , -1.1f, ship1.Position.Z - 0.05f);            
            player1.Direction = 1.0f;
            player1.Ship = new Ship(ship1.Position);
            player2 = new Player("p2");
            player2.InitialPosition = new Vector3(ship2.Position.X, -1.1f, ship2.Position.Z - 0.05f);
            player2.Direction = -1.0f;
            player2.Ship = new Ship(ship2.Position);

            simulation.PlayerOne = player1;
            simulation.PlayerTwo = player2;

            scene.SimpleLights.InsertRange(0, Lights);
        }

        protected Dictionary<string, Animation> ProcessAnimations(string unitName, LuaTable data)
        {
            var animations = new Dictionary<string, Animation>();
            foreach (string animName in data.Keys)
            {
                var animation = (LuaTable) data[animName];

                var fps = (int) (double) animation["fps"];
                var sheetPath = (string) animation["spritesheet"];

                var currentTexture = content.Load<Texture2D>(sheetPath);
                var currentAnimation = new Animation(currentTexture, fps);

                var frames = (LuaTable) animation["frames"];
                foreach (LuaTable frame in frames.Values)
                {
                    var x = (int) (double) frame["x"];
                    var y = (int) (double) frame["y"];
                    var w = (int) (double) frame["width"];
                    var h = (int) (double) frame["height"];
                    var rect = new Rectangle(x, y, w, h);

                    var offx = (int) (double) frame["offsetx"];
                    var offy = (int) (double) frame["offsety"];
                    var offset = new Point(offx, offy);

                    currentAnimation.Frames.Add(new Frame(rect, offset));
                }
                animations[animName] = currentAnimation;
            }
            return animations;
        }

        protected void ProcessLayouts(BoardLayoutFinder finder)
        {
            Lua lua = LuaMachine.Instance;
            object[] luaResult = lua.DoFile("Content/Config/layouts.lua");
            var tbl = (LuaTable) luaResult[0];

            var thumbnailsPath = (string) tbl["thumbnails"];
            var blockSizeTbl = (LuaTable) tbl["block_size"];
            Point blockSize = LuaMachine.LuaTableToPoint(blockSizeTbl);

            Texture2D thumbnailSprites = null;
            if (thumbnailsPath != null)
                thumbnailSprites = content.Load<Texture2D>(thumbnailsPath);

            foreach (object key in tbl.Keys)
            {
                if (key is double)
                {
                    var layoutTbl = (LuaTable) tbl[key];
                    var top = (string) layoutTbl["top"];
                    var bottom = (string) layoutTbl["bottom"];
                    var unit = (string) layoutTbl["unit"];
                    var coords = (LuaTable) layoutTbl["thumbnails"];

                    var layout = new LayoutDescription(unit, top, bottom);

                    if (coords != null && thumbnailSprites != null)
                    {
                        // process thumbnails coordinates
                        var points = new List<Point>();
                        foreach (object k in coords.Keys)
                        {
                            var coord = (LuaTable) coords[k];
                            points.Add(LuaMachine.LuaTableToPoint(coord));
                        }
                        layout.Thumbnails = thumbnailSprites;
                        layout.ThumbnailBlocks = points.ToArray();
                        layout.BlockSize = blockSize;
                    }

                    finder.AddLayout(layout);
                }
            }
        }

        #endregion
    }
}