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
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using GameStateManagement;
#endregion

namespace Junkyard.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Structures
        private struct AnimationData
        {
            public Texture2D texture;
            public SpriteSheetDimensions dimensions;
            public string name;
            public AnimationData(Texture2D texture, SpriteSheetDimensions dims, string name)
            {
                this.texture = texture;
                this.dimensions = dims;
                this.name = name;
            }
        }
        #endregion

        #region Fields

        ContentManager _content;
        float _pauseAlpha;
        InputAction _pauseAction;       

        #endregion

        #region Krzysztoff's Fields
        private static Dictionary<string, SpriteSheetDimensions> animationParams = new Dictionary<string, SpriteSheetDimensions>()
        {
            { "walk",       new SpriteSheetDimensions(4, 2,  8) },
            { "attack",     new SpriteSheetDimensions(6, 4, 22) },
            { "die",        new SpriteSheetDimensions(5, 3, 13) },
            { "idle",       new SpriteSheetDimensions(3, 2,  6) },
            { "postwalk",   new SpriteSheetDimensions(4, 2,  8) },
            { "prewalk",    new SpriteSheetDimensions(4, 2,  7) },            
        };

        private List<Light> Lights = new List<Light>()
        {
            new Light(LightType.Point, Color.Red,   new Vector3(-2.3f, -0.7f, -0.6f), Vector3.Zero, 0.8f),            
            new Light(LightType.Point, Color.Green, new Vector3(-1.0f, -0.1f, -0.6f), Vector3.Zero, 0.8f),
            new Light(LightType.Point, Color.Pink,  new Vector3(-2.0f, -1.2f, -0.6f), Vector3.Zero, 0.9f),
            new Light(LightType.Point, Color.Pink,  new Vector3( 1.0f, -0.1f, -0.2f), Vector3.Zero, 0.8f),
        };

        private List<AnimationData> animations;
                
        private SpriteFont spriteFont;        

        private Scene scene;
        private SimpleSceneRenderer sceneRenderer;
        private FreeCamera camera;
        private FrameSprite3D guy;
        private Sprite3D ship;
        private Sprite3D ship2;
        private Vector3 P1SpriteInitialPosition = new Vector3(-1f, -0.2f, -1.25f);

        private int currentAnimation = 0;

        private float timePerFrame = 1000 / 16;
        private float frameTime = 0;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int timeFromAnimationChange = 0;
        private int animationChangeDelay = 200;

        private PuzzleBoardWidget puzzleBoard1;
        private PuzzleBoardWidget puzzleBoard2;

        private Simulation simulation;
        // TODO: just testing
        private SimpleBattleUnit unit;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {            
            if (!instancePreserved)
            {
                simulation = new Simulation();

                if (_content == null)
                {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                    sceneRenderer = new SimpleSceneRenderer(ScreenManager.GraphicsDevice, _content);
                    sceneRenderer.RenderShadows = true;
                }

                PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
                float aspectRatio = (float)pp.BackBufferWidth / (float)pp.BackBufferHeight;
                camera = new FreeCamera(new Vector3(0.8f, 2.5f, 9.8f), MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000.0f);

                scene = new Scene();
                scene.Camera = camera;
                scene.Ambient = new Color(0.7f, 0.7f, 0.7f);

                spriteFont = _content.Load<SpriteFont>("Fonts/sample");
                // load some sprite sheets
                animations = new List<AnimationData>();
                foreach (KeyValuePair<String, SpriteSheetDimensions> entry in animationParams)
                {
                    Texture2D texture = _content.Load<Texture2D>(String.Format("Images/units/nuk/{0}", entry.Key));
                    animations.Add(new AnimationData(texture, entry.Value, entry.Key));
                }

                Light dirLight = new Light(LightType.Directional, Color.Red);
                dirLight.Direction = new Vector3(0.45f, -0.15f, 0.875f);
                dirLight.Position = new Vector3(5.6f, 7.6f, 12.0f);
                scene.ShadowCastingLights.Add(dirLight);

                animations = new List<AnimationData>();
                foreach (KeyValuePair<String, SpriteSheetDimensions> entry in animationParams)
                {
                    Texture2D texture = _content.Load<Texture2D>(String.Format("Images/units/nuk/{0}", entry.Key));
                    animations.Add(new AnimationData(texture, entry.Value, entry.Key));
                }

                InitializeScene();

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();

                // add the debug puzzleboard control
                int boardMargin = 15;
                int boardWidth = (pp.BackBufferWidth - 3 * boardMargin) / 2;
                int boardHeight = 5 * boardWidth / 6;

                BoardLayoutFinder finder = new BoardLayoutFinder();

                finder.AddLayout("infantry3", new LayoutDescription("mm", "mm"));
                finder.AddLayout("infantry2", new LayoutDescription(null, "ff"));
                finder.AddLayout("infantry", new LayoutDescription("ww", "gg"));

                PuzzleBoard board1 = new PuzzleBoard(6, 5);                
                board1.Randomize();
                puzzleBoard1 = new PuzzleBoardWidget(this, _content, new Point(boardMargin, boardMargin), new Point(boardWidth, boardHeight));
                puzzleBoard1.LayoutFinder = finder;
                puzzleBoard1.Board = board1;
                puzzleBoard1.LayoutMatches += this.LayoutMatched;

                PuzzleBoard board2 = new PuzzleBoard(6, 5);
                board2.Randomize();
                puzzleBoard2 = new PuzzleBoardWidget(this, _content, new Point(pp.BackBufferWidth - boardWidth - 2 * boardMargin, boardMargin), new Point(boardWidth, boardHeight));
                puzzleBoard2.LayoutFinder = finder;
                puzzleBoard2.Board = board2;
                puzzleBoard2.LayoutMatches += this.LayoutMatched;
            }
        }

        protected void InitializeScene()
        {
            Layer layer = new Layer(-1.25f);           
            Vector3 spriteInitialPosition = new Vector3(2.0f, -0.2f, -1.25f);

            BattleUnitSkin skin = new BattleUnitSkin();
            skin.SetAnimation("attack", new Junkyard.AnimationData(new SpriteSheetDimensions(6, 4, 22), _content.Load<Texture2D>("Images/units/nuk/attack")));
            skin.SetAnimation("die", new Junkyard.AnimationData(new SpriteSheetDimensions(5, 3, 13), _content.Load<Texture2D>("Images/units/nuk/die")));
            skin.SetAnimation("idle", new Junkyard.AnimationData(new SpriteSheetDimensions(3, 2, 6), _content.Load<Texture2D>("Images/units/nuk/idle")));
            skin.SetAnimation("postwalk", new Junkyard.AnimationData(new SpriteSheetDimensions(4, 2, 8), _content.Load<Texture2D>("Images/units/nuk/postwalk")));
            skin.SetAnimation("prewalk", new Junkyard.AnimationData(new SpriteSheetDimensions(4, 2, 7), _content.Load<Texture2D>("Images/units/nuk/prewalk")));
            skin.SetAnimation("walk", new Junkyard.AnimationData(new SpriteSheetDimensions(4, 2, 8), _content.Load<Texture2D>("Images/units/nuk/walk")));
            unit = new SimpleBattleUnit(simulation, skin, spriteInitialPosition + new Vector3(-2.5f, 0f, 0f), new Vector3(3.6f, -0.2f, -1.25f));
            layer.Drawables.Add(unit.Avatar);

            guy = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(-2.5f, 0f, 0f), animations[currentAnimation].dimensions);                        
            //layer.Drawables.Add(guy);
            FrameSprite3D sprite = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(0.5f, 0f, 0.0f), animations[currentAnimation].dimensions);
            //layer.Drawables.Add(sprite);
            scene.Layers.Add(layer);

            //string[] files = System.IO.Directory.GetFiles("Content/Maps", "*.lua");
            var lua = LuaMachine.state;
            object[] luaResult = lua.DoFile("Content/Maps/test.lua");
            var tbl = (LuaInterface.LuaTable)luaResult[0];
            
            Texture2D texture;
            // tutaj copypasta, bo jestem juz bardzo zmeczony
            foreach (LuaInterface.LuaTable el in tbl.Values)
            {
                texture = _content.Load<Texture2D>((string)el["assetName"]);
                var pos = el["pos"] as LuaInterface.LuaTable;                
                var ypr = el["yawpitchroll"] as LuaInterface.LuaTable;
                var scale = el["scale"] as LuaInterface.LuaTable;

                var asset = new Sprite3D(
                    texture,
                    null,
                    new Vector3((float)(double)pos[1], (float)(double)pos[2], (float)(double)pos[3]),
                    Quaternion.CreateFromYawPitchRoll((float)(double)ypr[1], (float)(double)ypr[2], (float)(double)ypr[3]),
                    new Vector3((float)(double)scale[1], (float)(double)scale[2], (float)(double)scale[3])
                    );

                var normalMap = el["normalMap"];
                if (normalMap != null)
                    asset.NormalMap = _content.Load<Texture2D>((string)normalMap);

                scene.Unlayered.Add(asset);
            }

            var ships = (LuaInterface.LuaTable)luaResult[1];

            foreach (object key in ships.Keys)
            {
                var el = ships[key] as LuaInterface.LuaTable;                
                texture = _content.Load<Texture2D>((string)el["assetName"]);
                var pos = el["pos"] as LuaInterface.LuaTable;
                var ypr = el["yawpitchroll"] as LuaInterface.LuaTable;
                var scale = el["scale"] as LuaInterface.LuaTable;

                var asset = new Sprite3D(
                    texture,
                    null,
                    new Vector3((float)(double)pos[1], (float)(double)pos[2], (float)(double)pos[3]),
                    Quaternion.CreateFromYawPitchRoll((float)(double)ypr[1], (float)(double)ypr[2], (float)(double)ypr[3]),
                    new Vector3((float)(double)scale[1], (float)(double)scale[2], (float)(double)scale[3])
                    );

                var normalMap = el["normalMap"];
                if (normalMap != null)
                    asset.NormalMap = _content.Load<Texture2D>((string)normalMap);

                if ((string)key == "ship1")
                    ship = asset;
                else if ((string)key == "ship2")
                    ship2 = asset;

                scene.Unlayered.Add(asset);
            }

            scene.SimpleLights.InsertRange(0, Lights);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            _content.Unload();            
        }


        #endregion

        #region Update and Draw

        private void LayoutMatched(Widget src, string name)
        {
            Vector3 initialPos;
            bool flipped;
            if (src == puzzleBoard1)
            {
                initialPos = new Vector3(-0.9f, -0.2f, -1.25f);
                flipped = false;
            }
            else {
                initialPos = new Vector3(5.0f, -0.2f, -1.25f);
                flipped = true;
            }            
            FrameSprite3D f = new FrameSprite3D(animations[currentAnimation].texture, initialPos, animations[currentAnimation].dimensions);
            f.Flipped = flipped;
            scene.Layers.Min.Drawables.Add(f);            
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
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                #region temporary stupid logic
                foreach (Layer layer in scene.Layers)
                {
                    // prawie jak Lisp
                    layer.Drawables.ForEach(x => { ((Sprite3D)x).Position += new Vector3((((Sprite3D)x).Flipped ? -1 : 1)* 0.03f, 0, 0); });
                    layer.Drawables.RemoveAll(x => ((Sprite3D)x).Distance(((Sprite3D)x).Flipped ? ship : ship2) < 0.9); 
                    //layer.Drawables.RemoveAll(x => ((Sprite3D)x).Distance(ship2) < 0.9);
                }

                puzzleBoard1.Update(gameTime);
                puzzleBoard2.Update(gameTime);
                
                #endregion
            }
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
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
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

                if (keyboardState.IsKeyDown(Keys.I))
                    //camera.Translate(new Vector3(0, 0, -.02f));                    
                    unit.Avatar.Flipped = !unit.Avatar.Flipped;
                if (keyboardState.IsKeyDown(Keys.K))
                    camera.Translate(new Vector3(0, 0, .02f));
                if (keyboardState.IsKeyDown(Keys.J))
                    camera.Position += new Vector3(-.02f, 0, 0);
                if (keyboardState.IsKeyDown(Keys.L))
                    camera.Position += new Vector3(.02f, 0, 0);
                if (keyboardState.IsKeyDown(Keys.Home))
                    camera.Pitch += .01f;
                if (keyboardState.IsKeyDown(Keys.End))
                    camera.Pitch -= .01f;

                if (keyboardState.IsKeyDown(Keys.PageDown))
                    camera.Translate(new Vector3(0, -.02f, 0));
                if (keyboardState.IsKeyDown(Keys.PageUp))
                    camera.Translate(new Vector3(0, .02f, 0));

                // player 1 temporary controls
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
                if (input.IsNewKeyPress(Keys.Enter, null, out player))
                    puzzleBoard2.Select();
                if (input.IsNewKeyPress(Keys.Back, null, out player))
                    puzzleBoard2.Randomize();

                if (keyboardState.IsKeyDown(Keys.J))
                    guy.Position += new Vector3(-0.1f, 0, 0);
                if (keyboardState.IsKeyDown(Keys.L))
                    guy.Position += new Vector3(0.1f, 0, 0);

                if (keyboardState.IsKeyDown(Keys.Z))
                    camera.Yaw += .01f;
                if (keyboardState.IsKeyDown(Keys.X))
                    camera.Yaw -= .01f;

                if (keyboardState.IsKeyDown(Keys.Delete))
                    sceneRenderer.RenderShadows = !sceneRenderer.RenderShadows;
                #endregion

                timeFromAnimationChange += gameTime.ElapsedGameTime.Milliseconds;
                if (timeFromAnimationChange >= animationChangeDelay)
                {                    
                    if (keyboardState.IsKeyDown(Keys.Space))
                    {
                        timeFromAnimationChange = 0;
                        currentAnimation++;
                        if (currentAnimation >= animations.Count)
                            currentAnimation = 0;

                        guy.Texture = animations[currentAnimation].texture;
                        guy.GridDimensions = animations[currentAnimation].dimensions;
                        guy.Reset();
                    }
                }

                frameTime += gameTime.ElapsedGameTime.Milliseconds;
                if (frameTime >= timePerFrame)
                {
                    frameTime -= timePerFrame;
                    foreach (Layer layer in scene.Layers)
                    {
                        foreach (IDrawable drawable in layer.Drawables)
                        {
                            // some dirty business going on here!
                            if (drawable is FrameSprite3D)
                            {
                                FrameSprite3D s = (FrameSprite3D)drawable;
                                s.NextFrame();
                                if (s.AnimationFinished)
                                    s.Reset();
                            }
                        }
                    }                    
                    unit.Update(gameTime);
                }               

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
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);           

            sceneRenderer.Render(scene);

            // Our player and enemy are both actually just text strings.
            //spriteBatch.Begin();
            //SpriteBatch spriteBatch = ScreenManager.SpriteBatch;           
            //spriteBatch.DrawString(spriteFont, animations[currentAnimation].name, Vector2.Zero, Color.White);            
            //string info = string.Format("cam: {0}, yaw: {1}", camera.Position.ToString(), camera.Yaw);
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 20), Color.White);
            //info = string.Format("FPS: {0}, guy: {1}", frameRate, guy.Position.ToString());
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 40), Color.White);
            //spriteBatch.End();

            puzzleBoard1.Draw(gameTime);
            puzzleBoard2.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            frameCounter++;
        }


        #endregion
    }
}