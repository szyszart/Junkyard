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
            { "attack",     new SpriteSheetDimensions(6, 4, 22) },
            { "die",        new SpriteSheetDimensions(5, 3, 13) },
            { "idle",       new SpriteSheetDimensions(3, 2,  6) },
            { "postwalk",   new SpriteSheetDimensions(4, 2,  8) },
            { "prewalk",    new SpriteSheetDimensions(4, 2,  7) },
            { "walk",       new SpriteSheetDimensions(4, 2,  8) }            
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
        //private InputManager input;

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
                if (_content == null)
                {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                    sceneRenderer = new SimpleSceneRenderer(ScreenManager.GraphicsDevice, _content);
                    sceneRenderer.RenderShadows = true;
                }

                PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
                float aspectRatio = (float)pp.BackBufferWidth / (float)pp.BackBufferHeight;
                camera = new FreeCamera(new Vector3(0f, 0.95f, 5.6f), MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000.0f);

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
            }
        }

        protected void InitializeScene()
        {
            Layer layer = new Layer(-1.25f);                      

            Vector3 spriteInitialPosition = new Vector3(1.0f, -0.2f, -1.25f);
            guy = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(-2.5f, 0f, 0f), animations[currentAnimation].dimensions);
            layer.Drawables.Add(guy);
            FrameSprite3D sprite = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(0.5f, 0f, 0.0f), animations[currentAnimation].dimensions);
            layer.Drawables.Add(sprite);
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

            //texture = _content.Load<Texture2D>("Images/Others/powietrze_male");
            //Sprite3D floor = new Sprite3D(texture, null, new Vector3(0, -1.2f, -21.0f), Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), new Vector3(20.0f, 20.0f, 1.0f));
            //scene.Unlayered.Add(floor);

            //texture = _content.Load<Texture2D>("Images/Others/lightbulb");
            //Sprite3D bulb = new Sprite3D(texture, null, new Vector3(0, 0, 0), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(0.1f, 0.1f, 1.0f));
            //scene.Unlayered.Add(bulb);

            //texture = _content.Load<Texture2D>("Images/Others/blisko");
            //Sprite3D hills = new Sprite3D(texture, null, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(10.0f, 2.0f, 1.0f));
            //texture = _content.Load<Texture2D>("Images/Others/blisko_norm");
            //hills.NormalMap = texture;
            //scene.Unlayered.Add(hills);

            //texture = _content.Load<Texture2D>("Images/Others/oddalenie");
            //Sprite3D distantHills = new Sprite3D(texture, null, new Vector3(0.0f, -1.0f, -10.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(20.0f, 6.0f, 1.0f));
            //scene.Unlayered.Add(distantHills);

            //texture = _content.Load<Texture2D>("Images/Others/kapitan");
            //ship = new Sprite3D(texture, null, new Vector3(-4.0f, 0.0f, -1.2f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(2.0f, 2.0f, 1.0f));
            //texture = _content.Load<Texture2D>("Images/Others/kapitan_norm");
            //ship.NormalMap = texture;
            //scene.Unlayered.Add(ship);

            //texture = _content.Load<Texture2D>("Images/Others/kapitan");
            //ship2 = new Sprite3D(texture, null, new Vector3(6.0f, 0.5f, -1.2f), Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(180), MathHelper.ToRadians(5), 0), new Vector3(2.0f, 2.0f, 1.0f));
            //texture = _content.Load<Texture2D>("Images/Others/kapitan_norm");
            //ship2.NormalMap = texture;
            //scene.Unlayered.Add(ship2);

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
                    layer.Drawables.ForEach(x => { if (x != guy) ((Sprite3D)x).Position += new Vector3(0.03f, 0.0f, 0.0f); });
                    layer.Drawables.RemoveAll(x => ((Sprite3D)x).Distance(ship2) < 1.5);
                }
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

                if (keyboardState.IsKeyDown(Keys.Up))
                    camera.Translate(new Vector3(0, 0, -.02f));
                if (keyboardState.IsKeyDown(Keys.Down))
                    camera.Translate(new Vector3(0, 0, .02f));
                if (keyboardState.IsKeyDown(Keys.Left))
                    camera.Position += new Vector3(-.02f, 0, 0);
                if (keyboardState.IsKeyDown(Keys.Right))
                    camera.Position += new Vector3(.02f, 0, 0);

                if (keyboardState.IsKeyDown(Keys.Home))
                    camera.Pitch += .01f;
                if (keyboardState.IsKeyDown(Keys.End))
                    camera.Pitch -= .01f;

                if (keyboardState.IsKeyDown(Keys.PageDown))
                    camera.Translate(new Vector3(0, -.02f, 0));
                if (keyboardState.IsKeyDown(Keys.PageUp))
                    camera.Translate(new Vector3(0, .02f, 0));

                if (keyboardState.IsKeyDown(Keys.W))
                    Lights[0].Position += new Vector3(0, 0, 0.1f);
                if (keyboardState.IsKeyDown(Keys.S))
                    Lights[0].Position += new Vector3(0, 0, -0.1f);
                if (keyboardState.IsKeyDown(Keys.A))
                    Lights[0].Position += new Vector3(-0.1f, 0, 0);
                if (keyboardState.IsKeyDown(Keys.D))
                    Lights[0].Position += new Vector3(0.1f, 0, 0);

                //if (keyboardState.IsKeyDown(Keys.I))
                //    guy.Position += new Vector3(0, 0, 0.1f);
                //if (keyboardState.IsKeyDown(Keys.K))
                //    guy.Position += new Vector3(0, 0, -0.1f);
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
                        guy.gridDimensions = animations[currentAnimation].dimensions;
                        guy.Reset();
                    }
                }

                frameTime += gameTime.ElapsedGameTime.Milliseconds;
                if (frameTime >= timePerFrame)
                {
                    frameTime -= timePerFrame;
                    guy.NextFrame();
                }               

                elapsedTime += gameTime.ElapsedGameTime;
                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }                

                camera.Update();            //czemu dwa razy camera.Update() ?
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
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;           

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, animations[currentAnimation].name, Vector2.Zero, Color.White);
            string info = string.Format("cam: {0}, yaw: {1}", camera.Position.ToString(), camera.Yaw);
            spriteBatch.DrawString(spriteFont, info, new Vector2(0, 20), Color.White);
            info = string.Format("FPS: {0}, guy: {1}", frameRate, guy.Position.ToString());
            spriteBatch.DrawString(spriteFont, info, new Vector2(0, 40), Color.White);

            spriteBatch.End();

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

