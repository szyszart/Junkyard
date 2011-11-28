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

using GameStateManagement;
using Junkyard.Screens;

namespace Junkyard
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        //#region Structures
        //private struct AnimationData
        //{
        //    public Texture2D texture;
        //    public SpriteSheetDimensions dimensions;
        //    public string name;
        //    public AnimationData(Texture2D texture, SpriteSheetDimensions dims, string name)
        //    {
        //        this.texture = texture;
        //        this.dimensions = dims;
        //        this.name = name;
        //    }
        //}
        //#endregion

        #region Fields
        //private static Dictionary<string, SpriteSheetDimensions> animationParams = new Dictionary<string, SpriteSheetDimensions>()
        //{
        //    { "attack",     new SpriteSheetDimensions(6, 4, 22) },
        //    { "die",        new SpriteSheetDimensions(5, 3, 13) },
        //    { "idle",       new SpriteSheetDimensions(3, 2,  6) },
        //    { "postwalk",   new SpriteSheetDimensions(4, 2,  8) },
        //    { "prewalk",    new SpriteSheetDimensions(4, 2,  7) },
        //    { "walk",       new SpriteSheetDimensions(4, 2,  8) }            
        //};

        //private List<Light> Lights = new List<Light>()
        //{
        //    new Light(LightType.Point, Color.Red,   new Vector3(-2.3f, -0.7f, -0.6f), Vector3.Zero, 0.8f),            
        //    new Light(LightType.Point, Color.Green, new Vector3(-1.0f, -0.1f, -0.6f), Vector3.Zero, 0.8f),
        //    new Light(LightType.Point, Color.Pink,  new Vector3(-2.0f, -1.2f, -0.6f), Vector3.Zero, 0.9f),
        //    new Light(LightType.Point, Color.Pink,  new Vector3( 1.0f, -0.1f, -0.2f), Vector3.Zero, 0.8f),
        //};

        //private List<AnimationData> animations;

        private GraphicsDeviceManager graphics;
        //private SpriteBatch spriteBatch;        

        ScreenManager screenManager;
        ScreenFactory screenFactory;

        //private Scene scene;
        //private SimpleSceneRenderer sceneRenderer;
        //private FreeCamera camera;
        //private FrameSprite3D guy;
        //private Sprite3D ship;
        //private Sprite3D ship2;
        //private Vector3 P1SpriteInitialPosition = new Vector3(-1f, -0.2f, -1.25f);

        //private int currentAnimation = 0;

        //private float timePerFrame = 1000 / 16;
        //private float frameTime = 0;
        //private int frameRate = 0;
        //private int frameCounter = 0;
        //private TimeSpan elapsedTime = TimeSpan.Zero;
        //private int timeFromAnimationChange = 0;
        //private int animationChangeDelay = 200;       

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024; // 640;
            graphics.PreferredBackBufferHeight = 768; // 480;
            
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            AddInitialScreens();            
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

            screenManager.AddScreen(new MainMenuScreen(), null);           
        }

        //protected override void Initialize()
        //{
        //    input.GetGamePad(ExtendedPlayerIndex.Five).ButtonPressed += gamePadButtonPressed;            
        //    base.Initialize();
        //}

        // gamepad buttonpressed event
        //private void gamePadButtonPressed(Buttons buttons)
        //{            
        //    if (buttons == Buttons.A)
        //    {                
        //        FrameSprite3D sprite = new FrameSprite3D(animations[currentAnimation].texture, P1SpriteInitialPosition + new Vector3(0.5f, 0f, 0.0f), animations[currentAnimation].dimensions);
        //        var layer = scene.Layers.ElementAt(0);
        //        layer.Drawables.Add(sprite);
        //    }
            
        //    if (buttons == Buttons.B)
        //    {
        //        var layer = scene.Layers.ElementAt(0);
        //        layer.Drawables.RemoveAt(1);
        //    }
        //    if (buttons == Buttons.Start)
        //        Exit();
        //}

        //protected void InitializeScene()
        //{
        //    Layer layer = new Layer(-1.25f);

        //    Vector3 spriteInitialPosition = new Vector3(1.0f, -0.2f, -1.25f);
        //    guy = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(-2.5f, 0f, 0f), animations[currentAnimation].dimensions);
        //    layer.Drawables.Add(guy);
        //    FrameSprite3D sprite = new FrameSprite3D(animations[currentAnimation].texture, spriteInitialPosition + new Vector3(0.5f, 0f, 0.0f), animations[currentAnimation].dimensions);
        //    layer.Drawables.Add(sprite);
        //    scene.Layers.Add(layer);            
        //    Texture2D texture;
        //    texture = Content.Load<Texture2D>("Images/others/powietrze_male");
        //    Sprite3D background = new Sprite3D(texture, null, new Vector3(0, 5.0f, -20.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(20.02f, 11.71f, 1.0f));
        //    scene.Unlayered.Add(background);

        //    Sprite3D floor = new Sprite3D(texture, null, new Vector3(0, -1.2f, -21.0f), Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver2, 0), new Vector3(20.0f, 20.0f, 1.0f));
        //    scene.Unlayered.Add(floor);
            
        //    texture = Content.Load<Texture2D>("Images/others/lightbulb");
        //    Sprite3D bulb = new Sprite3D(texture, null, new Vector3(0, 0, 0), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(0.1f, 0.1f, 1.0f));
        //    scene.Unlayered.Add(bulb);

        //    texture = Content.Load<Texture2D>("Images/others/blisko");
        //    Sprite3D hills = new Sprite3D(texture, null, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(10.0f, 2.0f, 1.0f));
        //    texture = Content.Load<Texture2D>("Images/others/blisko_norm");
        //    hills.NormalMap = texture;
        //    scene.Unlayered.Add(hills);

        //    texture = Content.Load<Texture2D>("Images/others/oddalenie");
        //    Sprite3D distantHills = new Sprite3D(texture, null, new Vector3(0.0f, -1.0f, -10.0f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(20.0f, 6.0f, 1.0f));
        //    scene.Unlayered.Add(distantHills);

        //    texture = Content.Load<Texture2D>("Images/others/kapitan");
        //    ship = new Sprite3D(texture, null, new Vector3(-4.0f, 0.0f, -1.2f), Quaternion.CreateFromYawPitchRoll(0, 0, 0), new Vector3(2.0f, 2.0f, 1.0f));
        //    texture = Content.Load<Texture2D>("Images/others/kapitan_norm");
        //    ship.NormalMap = texture;
        //    scene.Unlayered.Add(ship);

        //    texture = Content.Load<Texture2D>("Images/others/kapitan");
        //    ship2 = new Sprite3D(texture, null, new Vector3(6.0f, 0.5f, -1.2f), Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(180), MathHelper.ToRadians(5), 0), new Vector3(2.0f, 2.0f, 1.0f));
        //    texture = Content.Load<Texture2D>("Images/others/kapitan_norm");
        //    ship2.NormalMap = texture;
        //    scene.Unlayered.Add(ship2);

        //    scene.SimpleLights.InsertRange(0, Lights);            
        //}

        //protected override void LoadContent()
        //{
        //    spriteBatch = new SpriteBatch(GraphicsDevice);
        //    sceneRenderer = new SimpleSceneRenderer(GraphicsDevice, Content);
        //    sceneRenderer.RenderShadows = true;
        //    spriteFont = Content.Load<SpriteFont>("Fonts/sample");
            
        //    PresentationParameters pp = GraphicsDevice.PresentationParameters;
        //    float aspectRatio = (float)pp.BackBufferWidth / (float)pp.BackBufferHeight;
        //    camera = new FreeCamera(new Vector3(0f, 0.95f, 5.6f), MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000.0f);
            
        //    scene = new Scene();
        //    scene.Camera = camera;
        //    scene.Ambient = new Color(0.7f, 0.7f, 0.7f);
        //    // load some sprite sheets
        //    animations = new List<AnimationData>();
        //    foreach (KeyValuePair<String, SpriteSheetDimensions> entry in animationParams)
        //    {
        //        Texture2D texture = Content.Load<Texture2D>(String.Format("Images/units/nuk/{0}", entry.Key));
        //        animations.Add(new AnimationData(texture, entry.Value, entry.Key));
        //    }

        //    Light dirLight = new Light(LightType.Directional, Color.Red);
        //    dirLight.Direction = new Vector3(0.45f, -0.15f, 0.875f); 
        //    dirLight.Position = new Vector3(5.6f, 7.6f, 12.0f);
        //    scene.ShadowCastingLights.Add(dirLight);

        //    animations = new List<AnimationData>();
        //    foreach (KeyValuePair<String, SpriteSheetDimensions> entry in animationParams)
        //    {
        //        Texture2D texture = Content.Load<Texture2D>(String.Format("Images/units/nuk/{0}", entry.Key));
        //        animations.Add(new AnimationData(texture, entry.Value, entry.Key));
        //    }

        //    InitializeScene();            
        //}

        protected override void UnloadContent()
        {
        }

        #region Draw and update
        //protected override void Update(GameTime gameTime)
        //{
        //    #region temporary keybord controls
        //    //KeyboardState keyboardState = Keyboard.GetState();            
        //    //if (keyboardState.IsKeyDown(Keys.Escape))
        //    //    this.Exit();

        //    //if (keyboardState.IsKeyDown(Keys.Up))
        //    //    camera.Translate(new Vector3(0, 0, -.02f));
        //    //if (keyboardState.IsKeyDown(Keys.Down))
        //    //    camera.Translate(new Vector3(0, 0, .02f));
        //    //if (keyboardState.IsKeyDown(Keys.Left))
        //    //    camera.Position += new Vector3(-.02f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.Right))
        //    //    camera.Position += new Vector3(.02f, 0, 0);

        //    //if (keyboardState.IsKeyDown(Keys.Home))
        //    //    camera.Pitch += .01f;
        //    //if (keyboardState.IsKeyDown(Keys.End))
        //    //    camera.Pitch -= .01f;

        //    //if (keyboardState.IsKeyDown(Keys.PageDown))
        //    //    camera.Translate(new Vector3(0, -.02f, 0));
        //    //if (keyboardState.IsKeyDown(Keys.PageUp))
        //    //    camera.Translate(new Vector3(0, .02f, 0));

        //    //if (keyboardState.IsKeyDown(Keys.W))
        //    //    Lights[0].Position += new Vector3(0, 0, 0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.S))
        //    //    Lights[0].Position += new Vector3(0, 0, -0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.A))
        //    //    Lights[0].Position += new Vector3(-0.1f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.D))
        //    //    Lights[0].Position += new Vector3(0.1f, 0, 0);

        //    ////if (keyboardState.IsKeyDown(Keys.I))
        //    ////    guy.Position += new Vector3(0, 0, 0.1f);
        //    ////if (keyboardState.IsKeyDown(Keys.K))
        //    ////    guy.Position += new Vector3(0, 0, -0.1f);
        //    //if (keyboardState.IsKeyDown(Keys.J))
        //    //    guy.Position += new Vector3(-0.1f, 0, 0);
        //    //if (keyboardState.IsKeyDown(Keys.L))
        //    //    guy.Position += new Vector3(0.1f, 0, 0);

        //    //if (keyboardState.IsKeyDown(Keys.Z))
        //    //    camera.Yaw += .01f;
        //    //if (keyboardState.IsKeyDown(Keys.X))
        //    //    camera.Yaw -= .01f;

        //    //if (keyboardState.IsKeyDown(Keys.Delete))
        //    //    sceneRenderer.RenderShadows = !sceneRenderer.RenderShadows;
        //    #endregion

        //    //temporary gamepad controls
        //    var gPadState = input.GetGamePad(ExtendedPlayerIndex.Five).GetState();            
        //    var thumbSticks = gPadState.ThumbSticks;
        //    if (Math.Abs(thumbSticks.Left.Y) > 0.1f)
        //        camera.Translate(new Vector3(0, 0, -thumbSticks.Left.Y/3));
        //    if (Math.Abs(thumbSticks.Left.X) > 0.1f)
        //        camera.Position += new Vector3(thumbSticks.Left.X/3, 0, 0);
            

        //    timeFromAnimationChange += gameTime.ElapsedGameTime.Milliseconds;
        //    if (timeFromAnimationChange >= animationChangeDelay)
        //    {
        //        if (keyboardState.IsKeyDown(Keys.Space) || gPadState.IsButtonDown(Buttons.X))
        //        {
        //            timeFromAnimationChange = 0;
        //            currentAnimation++;
        //            if (currentAnimation >= animations.Count)
        //                currentAnimation = 0;

        //            guy.Texture = animations[currentAnimation].texture;
        //            guy.gridDimensions = animations[currentAnimation].dimensions;
        //            guy.Reset();
        //        }
        //    }

        //    frameTime += gameTime.ElapsedGameTime.Milliseconds;
        //    if (frameTime >= timePerFrame)
        //    {
        //        frameTime -= timePerFrame;
        //        guy.NextFrame();
        //    }

        //    camera.Update();

        //    elapsedTime += gameTime.ElapsedGameTime;
        //    if (elapsedTime > TimeSpan.FromSeconds(1))
        //    {
        //        elapsedTime -= TimeSpan.FromSeconds(1);
        //        frameRate = frameCounter;
        //        frameCounter = 0;
        //    }

        //    #region temporary stupid logic
        //    foreach (Layer layer in scene.Layers)
        //    {
        //        layer.Drawables.ForEach(x => { if (x != guy) ((Sprite3D)x).Position += new Vector3(0.03f, 0.0f, 0.0f); });
        //        layer.Drawables.RemoveAll(x => ((Sprite3D)x).Distance(ship2) < 1.5);                
        //    }
        //    #endregion

        //    camera.Update();            //czemu dwa razy camera.Update() ?
        //    base.Update(gameTime);
        //}

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            //sceneRenderer.Render(scene);

            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, animations[currentAnimation].name, Vector2.Zero, Color.White);
            //string info = string.Format("cam: {0}, yaw: {1}", camera.Position.ToString(), camera.Yaw);
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 20), Color.White);
            //info = string.Format("FPS: {0}, guy: {1}", frameRate, guy.Position.ToString());
            //spriteBatch.DrawString(spriteFont, info, new Vector2(0, 40), Color.White);

            //spriteBatch.End();

            base.Draw(gameTime);
            //frameCounter++;
        }
        #endregion
    }
}
