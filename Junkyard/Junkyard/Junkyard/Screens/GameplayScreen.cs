#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameStateManagement;
using Junkyard.Animations;
using Junkyard.Camera;
using Junkyard.Entities;
using Junkyard.Entities.UnitFactories;
using Junkyard.Entities.Units;
using Junkyard.Helpers;
using Junkyard.Localization;
using Junkyard.Particles;
using Junkyard.Rendering;
using LuaInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Junkyard.Screens
{
    /// <summary>
    ///     This screen implements the actual game logic.
    /// </summary>
    internal class GamePlayScreen : GameScreen
    {
        #region Constants

        private const int DUST_PARTICLE_SYSTEM = 1;
        private const string UNITS_DIRECTORY = "Units";

        #endregion
        #region Private fields

        private readonly Vector3 SCENE_CENTER = new Vector3(-2.2f, 0.36f, 3.8f);

        private FreeCamera _camera;
        private CameraManager _cameraManager;

        private ContentManager _content;

        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _frameCounter;
        private int _frameRate;

        private readonly List<Light> _lights = new List<Light>
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

        private readonly string _map;
        private ParticleManager _particleManager;

        private readonly InputAction _pauseAction;
        private float _pauseAlpha;

        private Player _player1, _player2;

        private PuzzleBoardWidget _puzzleBoard1;
        private PuzzleBoardWidget _puzzleBoard2;
        private Scene _scene;
        private SimpleSceneRenderer _sceneRenderer;

        private Simulation _simulation;

        #endregion
        #region Ctors

        /// <summary>
        ///     Constructor.
        /// </summary>
        public GamePlayScreen(string map)
        {
            TransitionOnTime = TimeSpan.FromSeconds(4);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] {Buttons.Start, Buttons.Back},
                new[] {Keys.Escape},
                true);
            _map = map;
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
            var widget = (PuzzleBoardWidget) src;
            Player who = widget.Board.Player;

            // spawn a new unit
            BattleUnit unit = _simulation.Spawn(instance.Layout.Name);
            unit.Simulation = _simulation;
            unit.Player = who;
            unit.Avatar.Position = who.InitialPosition;
            _simulation.Add(unit);
        }

        #endregion
        #region Overrides

        /// <summary>
        ///     Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                return;
            }
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            _sceneRenderer = new SimpleSceneRenderer(ScreenManager.GraphicsDevice, _content)
                                 {
                                     RenderShadows = false
                                 };

            var pp = ScreenManager.GraphicsDevice.PresentationParameters;
            var aspectRatio = pp.BackBufferWidth/(float) pp.BackBufferHeight;
            _camera = new FreeCamera(SCENE_CENTER, MathHelper.ToRadians(45), aspectRatio, 0.1f,
                                     1000.0f);

            _cameraManager = new CameraManager(_camera);

            _scene = new Scene {CameraManager = _cameraManager, Ambient = new Color(0.7f, 0.7f, 0.7f)};

            var dirLight = new Light(LightType.Directional, Color.Red)
                               {
                                   Direction =
                                       new Vector3(0.45f, -0.15f, 0.875f),
                                   Position = new Vector3(5.6f, 7.6f, 12.0f)
                               };
            _scene.ShadowCastingLights.Add(dirLight);
            InitializeScene();

            // add the debug puzzleboard control
            int boardMargin = 15;
            int boardWidth = (pp.BackBufferWidth - 3*boardMargin)/3;
            int boardHeight = 5*boardWidth/7;

            var finder = new BoardLayoutFinder();
            ProcessLayouts(finder);

            var board1 = new PuzzleBoard(6, 5) {Player = _player1};
            board1.Randomize();
            _puzzleBoard1 = new PuzzleBoardWidget(this, _content, new Point(boardMargin, boardMargin),
                                                  new Point(boardWidth, boardHeight));
            _puzzleBoard1.LayoutFinder = finder;
            _puzzleBoard1.Board = board1;
            _puzzleBoard1.LayoutAccepted += LayoutAccepted;

            var board2 = new PuzzleBoard(6, 5);
            board2.Player = _player2;
            board2.Randomize();
            _puzzleBoard2 = new PuzzleBoardWidget(this, _content,
                                                  new Point(pp.BackBufferWidth - boardWidth - 2*boardMargin,
                                                            boardMargin), new Point(boardWidth, boardHeight));
            _puzzleBoard2.LayoutFinder = finder;
            _puzzleBoard2.Board = board2;

            _puzzleBoard2.LayoutAccepted += LayoutAccepted;

            //_particleManager = new ParticleManager((Game)ScreenManager.Game);
            //_particleManager.AddSystem<DustParticleSystem>(DUST_PARTICLE_SYSTEM, 10, player1.InitialPosition.Z + 0.1f);                

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        ///     Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            _sceneRenderer.Render(_scene);
            //SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, camera.Position.ToString(), new Vector2(0, 0), Color.White);
            //spriteBatch.End();

            _puzzleBoard1.Draw(gameTime);
            _puzzleBoard2.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha/2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
            _frameCounter++;
        }

        /// <summary>
        ///     Lets the game respond to player input. Unlike the Update method,
        ///     this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            var playerIndex = (int) ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            playerIndex = input.CurrentGamePadStates[playerIndex + 4].IsConnected ? playerIndex + 4 : playerIndex;
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            var gamePadDisconnected = !gamePadState.IsConnected &&
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

                if (keyboardState.IsKeyDown(Keys.Home))
                    _camera.Pitch += .01f;
                if (keyboardState.IsKeyDown(Keys.End))
                    _camera.Pitch -= .01f;

                if (keyboardState.IsKeyDown(Keys.PageDown))
                    _camera.Translate(new Vector3(0, -.02f, 0));
                if (keyboardState.IsKeyDown(Keys.PageUp))
                    _camera.Translate(new Vector3(0, .02f, 0));

                // temporary camera controls
                if (input.IsKeyPressed(Keys.I, null, out player))
                    _camera.Translate(Vector3.UnitZ*-0.1f);
                if (input.IsKeyPressed(Keys.K, null, out player))
                    _camera.Translate(Vector3.UnitZ*0.1f);
                if (input.IsKeyPressed(Keys.J, null, out player))
                    _camera.Translate(Vector3.UnitX*-0.1f);
                if (input.IsKeyPressed(Keys.L, null, out player))
                    _camera.Translate(Vector3.UnitX*0.1f);

                // player 1 temporary controls)););)
                if (input.IsNewKeyPress(Keys.W, null, out player))
                    _puzzleBoard1.Up();
                if (input.IsNewKeyPress(Keys.S, null, out player))
                    _puzzleBoard1.Down();
                if (input.IsNewKeyPress(Keys.A, null, out player))
                    _puzzleBoard1.Left();
                if (input.IsNewKeyPress(Keys.D, null, out player))
                    _puzzleBoard1.Right();

                if (input.IsNewKeyPress(Keys.Q, null, out player))
                    _puzzleBoard1.Select();
                if (input.IsNewKeyPress(Keys.E, null, out player))
                    _puzzleBoard1.Accept();
                if (input.IsNewKeyPress(Keys.R, null, out player))
                    _puzzleBoard1.Randomize();

                // player 2 temporary controls
                if (input.IsNewKeyPress(Keys.Up, null, out player))
                    _puzzleBoard2.Up();
                if (input.IsNewKeyPress(Keys.Down, null, out player))
                    _puzzleBoard2.Down();
                if (input.IsNewKeyPress(Keys.Left, null, out player))
                    _puzzleBoard2.Left();
                if (input.IsNewKeyPress(Keys.Right, null, out player))
                    _puzzleBoard2.Right();
                if (input.IsNewKeyPress(Keys.RightControl, null, out player))
                    _puzzleBoard2.Select();
                if (input.IsNewKeyPress(Keys.Enter, null, out player))
                    _puzzleBoard2.Accept();
                if (input.IsNewKeyPress(Keys.Back, null, out player))
                    _puzzleBoard2.Randomize();

                if (keyboardState.IsKeyDown(Keys.Z))
                    _camera.Yaw += .01f;
                if (keyboardState.IsKeyDown(Keys.X))
                    _camera.Yaw -= .01f;

                // DEBUG, remove ASAP
                if (input.IsNewKeyPress(Keys.D1, null, out player))
                {
                    SpawnUnit("menele_ranged", _player1, Vector3.Zero);
                }
                if (input.IsNewKeyPress(Keys.D2, null, out player))
                {
                    SpawnUnit("menel_ram", _player1, Vector3.Zero);
                }
                if (input.IsNewKeyPress(Keys.D3, null, out player))
                {
                    SpawnUnit("menele_ranged", _player2, Vector3.Zero);
                }
                if (input.IsNewKeyPress(Keys.D4, null, out player))
                {
                    SpawnUnit("menel_infantry", _player2, Vector3.Zero);
                }
                if (input.IsNewKeyPress(Keys.D5, null, out player))
                {
                    SpawnUnit("menel_boar", _player1, 6*Vector3.UnitY);
                }
                if (input.IsNewKeyPress(Keys.D6, null, out player))
                {
                    SpawnUnit("menel_boar", _player2, 6*Vector3.UnitY);
                }
                if (input.IsNewKeyPress(Keys.D7, null, out player))
                {
                    SpawnUnit("menel_infantry", _player2, Vector3.Zero);
                }
                if (input.IsNewKeyPress(Keys.D8, null, out player))
                {
                    SpawnUnit("menel_ram", _player2, Vector3.Zero);
                }
                // END OF DEBUG

                if (keyboardState.IsKeyDown(Keys.Delete))
                    _sceneRenderer.RenderShadows = !_sceneRenderer.RenderShadows;

                #endregion
                #region Gamepad controls                               

                Vector2 tsPos = gamePadState.ThumbSticks.Left;

                if (Math.Abs(tsPos.X) > 0.1f || Math.Abs(tsPos.Y) > 0.1f)
                    _camera.Translate(new Vector3(tsPos, 0.0f));

                if (gamePadState.IsButtonDown(Buttons.DPadUp))
                    _puzzleBoard1.Up();
                if (gamePadState.IsButtonDown(Buttons.DPadDown))
                    _puzzleBoard1.Down();
                if (gamePadState.IsButtonDown(Buttons.DPadLeft))
                    _puzzleBoard1.Left();
                if (gamePadState.IsButtonDown(Buttons.DPadRight))
                    _puzzleBoard1.Right();

                if (gamePadState.IsButtonDown(Buttons.X))
                    _puzzleBoard1.Select();
                if (gamePadState.IsButtonDown(Buttons.Y))
                    _puzzleBoard1.Accept();
                if (gamePadState.IsButtonDown(Buttons.A))
                    _puzzleBoard1.Randomize();

                #endregion
                _simulation.Tick(gameTime);
            }
        }


        /// <summary>
        ///     Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            _content.Unload();
        }

        /// <summary>
        ///     Updates the state of the game. This method checks the GameScreen.IsActive
        ///     property, so the game will stop updating when the pause menu is active,
        ///     or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            _pauseAlpha = coveredByOtherScreen
                              ? Math.Min(_pauseAlpha + 1f/32, 1)
                              : Math.Max(_pauseAlpha - 1f/32, 0);

            if (!IsActive) return;
            #region temporary stupid logic

            if (_player1.Hp == 0 || _player2.Hp == 0)
            {
                const string msgFormat = "{0} {1} {2}.";
                var winner = _player2.Hp == 0 ? " 1" : " 2";
                var message = String.Format(msgFormat, LR.Player, winner, LR.GamePlayScreen_HasWon);

                var battleOverMessage = new BattleOverScreen(message, false);

                battleOverMessage.Accepted += BattleOverScreenConfirmed;
                battleOverMessage.Cancelled += BattleOverScreenConfirmed;

                ScreenManager.AddScreen(battleOverMessage, ControllingPlayer);
            }

            #endregion
            _puzzleBoard1.Update(gameTime);
            _puzzleBoard2.Update(gameTime);

            _cameraManager.Update(gameTime);

            //timer section
            // FPS measurement
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
                //call callback
                DirectCamera();
            }
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

        protected void InitializeScene()
        {
            _simulation = new Simulation();

            LoadUnits();

            var layer = new Layer(-1.25f);
            layer.Drawables.Add(_simulation);

            _scene.Layers.Add(layer);

            // TODO: remove hardcoded paths
            //string[] files = System.IO.Directory.GetFiles("Content/Maps", "*.lua");
            var lua = LuaMachine.Instance;
            object[] luaResult = lua.DoFile("Content/Maps/" + _map);
            var tbl = (LuaTable) luaResult[0];

            foreach (
                var asset in
                    tbl.Values.Cast<LuaTable>().Select(el => LuaMachine.LoadAsset(el, _content)))
            {
                _scene.Unlayered.Add(asset);
            }

            //var ships = (LuaTable)luaResult["statki"];
            var ships = (LuaTable) lua["statki"];

            Sprite3D ship1 = LuaMachine.LoadAsset((LuaTable) ships["ship1"], _content);
            _scene.Unlayered.Add(ship1);

            Sprite3D ship2 = LuaMachine.LoadAsset((LuaTable) ships["ship2"], _content);
            _scene.Unlayered.Add(ship2);

            var initialPositionY = (float) (double) lua["initialY"];
            _simulation.GroundLevel = initialPositionY + 0.5f;

            _player1 = new Player("p1")
                           {
                               InitialPosition =
                                   new Vector3(ship1.Position.X, initialPositionY, ship1.Position.Z - 0.05f),
                               Direction = 1.0f,
                               Ship = new Ship(ship1.Position)
                           };
            _player2 = new Player("p2")
                           {
                               InitialPosition =
                                   new Vector3(ship2.Position.X, initialPositionY, ship2.Position.Z - 0.05f),
                               Direction = -1.0f,
                               Ship = new Ship(ship2.Position)
                           };

            _simulation.PlayerOne = _player1;
            _simulation.PlayerTwo = _player2;

            _scene.SimpleLights.InsertRange(0, _lights);
            _cameraManager.Camera.Position = new Vector3(ship1.Position.X, _camera.Position.Y, _camera.Position.Z);
            _cameraManager.Goto(new Vector3(ship2.Position.X, _camera.Position.Y, _camera.Position.Z));
        }

        protected void LoadUnits()
        {
            var lua = LuaMachine.Instance;
            // TODO: remove hardcoded paths
            string unitsPath = Path.Combine(_content.RootDirectory, UNITS_DIRECTORY);
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
                    _simulation.FactoryDispatcher.RegisterFactory(name, factory);
                }
            }
        }

        protected Dictionary<string, Animation> ProcessAnimations(string unitName, LuaTable data)
        {
            var animations = new Dictionary<string, Animation>();
            foreach (string animName in data.Keys)
            {
                var animation = (LuaTable) data[animName];

                var fps = (int) (double) animation["fps"];
                var sheetPath = (string) animation["spritesheet"];

                var currentTexture = _content.Load<Texture2D>(sheetPath);
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
            var lua = LuaMachine.Instance;
            object[] luaResult = lua.DoFile("Content/Config/layouts.lua");
            var tbl = (LuaTable) luaResult[0];

            var thumbnailsPath = (string) tbl["thumbnails"];
            var blockSizeTbl = (LuaTable) tbl["block_size"];
            var blockSize = LuaMachine.LuaTableToPoint(blockSizeTbl);

            Texture2D thumbnailSprites = null;
            if (thumbnailsPath != null)
                thumbnailSprites = _content.Load<Texture2D>(thumbnailsPath);

            foreach (var key in tbl.Keys)
            {
                if (!(key is double))
                {
                    continue;
                }
                var layoutTbl = (LuaTable) tbl[key];
                var top = (string) layoutTbl["top"];
                var bottom = (string) layoutTbl["bottom"];
                var unit = (string) layoutTbl["unit"];
                var coords = (LuaTable) layoutTbl["thumbnails"];

                var layout = new LayoutDescription(unit, top, bottom);

                if (coords != null && thumbnailSprites != null)
                {
                    // process thumbnails coordinates
                    layout.Thumbnails = thumbnailSprites;
                    layout.ThumbnailBlocks =
                        coords.Values.Cast<LuaTable>().Select(LuaMachine.LuaTableToPoint).ToArray();
                    layout.BlockSize = blockSize;
                }

                finder.AddLayout(layout);
            }
        }

        #endregion
        #region Private methods

        private void DirectCamera()
        {
            if (_cameraManager.State == CameraState.Static && _simulation.Units.Count == 0)
            {
                _cameraManager.Goto(SCENE_CENTER);
            }
            else if (_simulation.Units.Count != 0)
            {
                var unit = _simulation.Units.GetRandomElement();

                var avatar = unit.ReallyDead ? null : unit.Avatar;
                if (avatar != null)
                {
                    _cameraManager.Follow(avatar, RandomizationHelper.RandomBetween(0.2f, 0.5f));
                }
            }
        }

        private void SpawnUnit(string name, Player player, Vector3 offsetV)
        {
            BattleUnit unit = _simulation.Spawn(name);
            unit.Simulation = _simulation;
            unit.Player = player;
            unit.Avatar.Position = player.InitialPosition + offsetV;
            _simulation.Add(unit);
        }

        #endregion
    }
}