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

namespace SecondGame
{
    struct AnimationData
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

    public class TestGame : Microsoft.Xna.Framework.Game
    {
        private static Dictionary<string, SpriteSheetDimensions> animationParams = new Dictionary<string, SpriteSheetDimensions>()
        {
            { "attack",     new SpriteSheetDimensions(6, 4, 22) },
            { "die",        new SpriteSheetDimensions(5, 3, 13) },
            { "idle",       new SpriteSheetDimensions(3, 2,  6) },
            { "postwalk",   new SpriteSheetDimensions(4, 2,  8) },
            { "prewalk",    new SpriteSheetDimensions(4, 2,  7) },
            { "walk",       new SpriteSheetDimensions(4, 2,  8) }            
        };

        private List<AnimationData> animations;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SimpleCamera camera;
        BasicEffect effect;

        Vector3 spriteInitialPosition = new Vector3(0, 0, -5);

        Matrix world = Matrix.Identity;        
        
        float timePerFrame = 1000 / 16;
        float frameTime = 0;

        int timeFromAnimationChange = 0;
        int animationChangeDelay = 200;

        int currentAnimation = 0;
        
        Texture2D texture;
        FrameSprite3D sprite;
        Sprite3D background;
        Sprite3D hills;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new SimpleCamera(GraphicsDevice, new Vector3(-3.7f, 0, 1.3f));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts/sample");

            effect = new BasicEffect(GraphicsDevice);

            animations = new List<AnimationData>();
            foreach (KeyValuePair<String, SpriteSheetDimensions> entry in animationParams)
            {
                texture = Content.Load<Texture2D>(String.Format("Images/nuk/{0}", entry.Key));
                animations.Add(new AnimationData(texture, entry.Value, entry.Key));                
            }

            if (animations.Count == 0)
                this.Exit();

            sprite = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition, animations[currentAnimation].dimensions);
            
            texture = Content.Load<Texture2D>("Images/others/sample");
            background = new Sprite3D(texture, new Vector3(0, 0, -10.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(20.0f, 20.0f, 1.0f));

            texture = Content.Load<Texture2D>("Images/others/hills");
            hills = new Sprite3D(texture, new Vector3(-5.0f, 0, -1.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(2.0f, 1.0f, 1.0f));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyboardState.IsKeyDown(Keys.Up))
                camera.Translate(new Vector3(0, 0, -.02f));
            if (keyboardState.IsKeyDown(Keys.Down))
                camera.Translate(new Vector3(0, 0, .02f));
            if (keyboardState.IsKeyDown(Keys.Left))
                camera.Translate(new Vector3(-.02f, 0, 0));
            if (keyboardState.IsKeyDown(Keys.Right))
                camera.Translate(new Vector3(.02f, 0, 0));

            if (keyboardState.IsKeyDown(Keys.Z))
                camera.Yaw += .01f;                
            if (keyboardState.IsKeyDown(Keys.X))
                camera.Yaw -= .01f;                

            timeFromAnimationChange += gameTime.ElapsedGameTime.Milliseconds;
            if (timeFromAnimationChange >= animationChangeDelay)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    timeFromAnimationChange = 0;
                    currentAnimation++;
                    if (currentAnimation >= animations.Count)
                        currentAnimation = 0;

                    sprite.texture = animations[currentAnimation].texture;
                    sprite.gridDimensions = animations[currentAnimation].dimensions;
                    sprite.Reset();                        
                }
            }

            frameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (frameTime >= timePerFrame)
            {
                frameTime -= timePerFrame;
                sprite.NextFrame();
            }
            
            camera.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;            
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // draw the background
            background.Draw(effect, camera);

            // draw the character
            sprite.Draw(effect, camera);
            hills.Draw(effect, camera);
            
            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, animations[currentAnimation].name, Vector2.Zero, Color.Black);
            string info = string.Format("Camera pos: ({0}, {1}, {2}), yaw = {3}", camera.Position.X, camera.Position.Y, camera.Position.Z, camera.Yaw);
            spriteBatch.DrawString(spriteFont, info, new Vector2(0, 20), Color.Black);
            spriteBatch.End();
        }
    }
}
