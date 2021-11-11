using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

using CPGDGameJam.Game;
using System.Diagnostics;
using CPGDGameJam.Game.Entities;
using Microsoft.Xna.Framework.Input.Touch;

namespace CPGDGameJam {
    public class Game1 : Microsoft.Xna.Framework.Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public const int SCREEN_WIDTH = 320;
        public const int SCREEN_HEIGHT = 180;
        public const int FRAME_RATE = 60;
        int backbufferWidth, backbufferHeight;
        private Matrix globalTransformation;

        public const int wres2 = SCREEN_WIDTH * 2;
        public const int hres2 = SCREEN_HEIGHT * 2;
#if !ANDROID
        public static int wScr = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int hScr = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif
        Rectangle CANVAS = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        public static int ScreenWidth = SCREEN_WIDTH * 5;
        public static int ScreenHeight = SCREEN_HEIGHT * 5;
        private RenderTarget2D _renderTarget;


        public static bool levelTransition = false;
        public static string levelFile;

        World world;
        Camera camera;
        Player player;
    

        List<AnimatedSprite> playerSpriteList;


        //SpriteFont font1;
        public enum GameState {
            TitleScreen,
            InGame
        }
        public GameState state = GameState.TitleScreen;


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            /* graphics.PreferredBackBufferWidth = ScreenWidth;  
             graphics.PreferredBackBufferHeight = ScreenHeight;  
             graphics.ApplyChanges();*/

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        public void Resolution(int w, int h, bool b) {
            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
            ScreenWidth = w;
            ScreenHeight = h;
            graphics.IsFullScreen = b;
            graphics.ApplyChanges();
        }
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();

            //world.addCollision(0);
        }

        protected override void LoadContent() {
            _renderTarget = new RenderTarget2D(GraphicsDevice, CANVAS.Width, CANVAS.Height);
            if (world != null)
                camera = new Camera(world.worldSize);
            else
                camera = new Camera(new Vector2Int(int.MaxValue, int.MaxValue));

            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // use this.Content to load your game content here
            //tPlayer = Content.Load<Texture2D>("Player");



            //playerSpriteList = new List<AnimatedSprite> {
            //    new AnimatedSprite(tPlayerIdle, new Vector2Int(16, 16), 1),
            //    new AnimatedSprite(tPlayerGlide, new Vector2Int(16, 16), 10),
            //    new AnimatedSprite(tPlayerClimb, new Vector2Int(16, 16), 7),
            //    new AnimatedSprite(tPlayerLand, new Vector2Int(16, 16), 8)
            //};

            //font1 = Content.Load<SpriteFont>("Font1");

            // TODO: Remove this to add a start menu
            //ToLevel("level1.json");
            //ScalePresentationArea();



            //Texture2D tile_main = Content.Load<Texture2D>("tileset_glide");
            /*world = new World("level1.json", Content);
            player = new Player(tPlayer, new Vector2(120, 20), world);
            world.scene.Add(player);*/

            /*world.scene = new List<Entity> {
                player,
                new NormalTile(tPlayer, new Vector2(190, 80), world)
            };*/

        }

        public void ScalePresentationArea() {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / SCREEN_WIDTH;
            float verScaling = backbufferHeight / SCREEN_HEIGHT;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
        }

        public static void LevelTransition(string level) {
            levelTransition = true;
            levelFile = level;
        }
        public void ToLevel(string level) {
            world = new World(level, Content);
            //player = new Player(tPlayer, playerSpriteList, new Vector2(120, 20), world);
            world.Add(player);
            camera = new Camera(world.worldSize);
            camera.approach.X = -player.position.X - (player.texture.Width / Camera.mod.x);
            camera.approach.Y = -player.position.Y - (player.texture.Height / Camera.mod.y);
        }

        protected override void Update(GameTime gameTime) {
            Input.GetState();
            Input.GetMouseState();
            //Input.GetTouchState();

            if (camera != null)
                camera.Follow(player);
#if !ANDROID
            float framerate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            Window.Title = "Game - " + System.String.Format("{0:0.00}", framerate) + " FPS";

            if (Input.keyPressed(Keys.F)) {
                if (graphics.IsFullScreen) {
                    Resolution(wres2, hres2, false);
                } else if (!graphics.IsFullScreen) {
                    Resolution(wScr, hScr, true);
                }
            }
#endif
            if (Input.keyPressed(Keys.F1)) {
                ToLevel("level1.json");
            }
            if (Input.keyPressed(Keys.F4)) {
                ToLevel("level4.json");
            }
            if (levelTransition) {
                ToLevel(levelFile);
                levelTransition = false;
            }



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (world != null) {
                foreach (Entity entity in world.scene) {
                    entity.Update(gameTime);
                    foreach (Component component in entity.components) {
                        component.Update(gameTime);
                    }
                }
            }

            /*TransformSystem.Update(gameTime);
            SpriteSystem.Update(gameTime);*/
            //ColliderSystem.Update(gameTime);

            //Input.oldMouseState = Input.newMouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Debug.WriteLine("Test");

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);//, transformMatrix: _camera.Transform);

            if (world != null) {
                world.drawLevel(spriteBatch);
            }
            //spriteBatch.Begin();

            if (world != null) {
                foreach (Entity entity in world.scene) {
                    if (entity.HasComponent<Sprite>()) {
                        entity.GetComponent<Sprite>().Draw(spriteBatch);
                        //spriteBatch.Draw(entity.GetComponent<Sprite>().texture, entity.GetComponent<Transform>().position, Color.White);
                    }
                }
            }
            /* string test = "test";
             spriteBatch.DrawString(font1, , new Vector2(0, 0), Color.White, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.5f);*/

            spriteBatch.End();

            // Don't mess with
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
