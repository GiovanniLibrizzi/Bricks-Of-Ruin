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
        public static int GridSize = 16;
        public static int ScreenWidth = SCREEN_WIDTH * 5;
        public static int ScreenHeight = SCREEN_HEIGHT * 5;
        private RenderTarget2D _renderTarget;


        public static bool levelTransition = false;
        public static string levelFile;

        public int tick;

        //
        World world;
        Camera camera;
        Player player;
        BuyMenu buyMenu;

        // Texture Initializing
        Texture2D sPlayer;
        public static Texture2D sLadder;
        public static Texture2D sBlock;
        public static Texture2D sCrumblingBlock;
        public static Texture2D sSpring;

        Texture2D sBackground;
        public static Texture2D sOutline;
        public static Texture2D sOutlineX;
        public static Texture2D sNumbers;
        public static Texture2D sNumbersSmall;

        public static Texture2D sGold;
        public static Texture2D sGoldAnim;
        Texture2D textBuyMode;
        public static Texture2D sHudBuyMenu;
        public static Texture2D sHudBlock;
        public static Texture2D sHudSpring;
        public static Texture2D sHudLadder;
        public static Texture2D sHudCrumblingBlock;
        public static Texture2D sHudSelector;
        public static Texture2D sHudContainer;
        public static Texture2D sHudSlash;

        // Saved variables 
        public int goldAmt = 0;
        public List<int> blockInventoryPrev = new List<int>();
        //public 
        
        int[] scale = { 1, 3, 4, 5, 6, -1};
        int scaleInc = 0;
        List<AnimatedSprite> playerSpriteList;


        //SpriteFont font1;
        public enum GameState {
            TitleScreen,
            InGame
        }
        public GameState state = GameState.InGame;


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
            if (world != null) {
                camera = new Camera(world.worldSize, player);
            } else {
                camera = new Camera(new Vector2Int(int.MaxValue, int.MaxValue), player);
                
            }

            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // use this.Content to load your game content here
            sPlayer = Content.Load<Texture2D>("sPlayer");
            sLadder = Content.Load<Texture2D>("s_ladder");
            sBlock = Content.Load<Texture2D>("s_block");
            sSpring = Content.Load<Texture2D>("s_spring");
            sCrumblingBlock = Content.Load<Texture2D>("s_crumblingblock");
            sOutline = Content.Load<Texture2D>("s_outline");
            sOutlineX = Content.Load<Texture2D>("s_outline_x");
            sNumbers = Content.Load<Texture2D>("s_numbers");
            sNumbersSmall = Content.Load<Texture2D>("s_numbers_small");
            sBackground = Content.Load<Texture2D>("bkg");
            sGold = Content.Load<Texture2D>("s_gold");
            sGoldAnim = Content.Load<Texture2D>("s_gold_anim");
            textBuyMode = Content.Load<Texture2D>("text_buymode");
            sHudBuyMenu = Content.Load<Texture2D>("s_hud_buymenu");
            sHudBlock = Content.Load<Texture2D>("s_hud_block");
            sHudSpring = Content.Load<Texture2D>("s_hud_spring");
            sHudLadder = Content.Load<Texture2D>("s_hud_ladder");
            sHudCrumblingBlock = Content.Load<Texture2D>("s_hud_crumblingblock");
            sHudSelector = Content.Load<Texture2D>("s_hud_selector");
            sHudContainer = Content.Load <Texture2D>("s_hud_container");
            sHudSlash = Content.Load<Texture2D>("s_hud_slash");



            playerSpriteList = new List<AnimatedSprite> {
                new AnimatedSprite(sPlayer, new Vector2Int(16, 16), 0),
                new AnimatedSprite(sPlayer, new Vector2Int(16, 16), 1)
            };

            //font1 = Content.Load<SpriteFont>("Font1");

            // TODO: Remove this to add a start menu
            ToLevel("level1.json");
            ScalePresentationArea();



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
            //player.goldAmt = goldAmt;
            world = new World(level, Content);

            player = new Player(sPlayer, playerSpriteList, new Vector2(120, 20), world);
            player.StateGoto(Player.pState.Paused);
            player.goldAmt = goldAmt;
            player.goldAmtPrev = goldAmt;
            player.goldAmtPrevPrev = player.goldAmtPrev;
            world.player = player;
            world.Add(player);

            MouseBlock mb = new MouseBlock(sOutline, new Vector2(100f, 100f), world);
            world.Add(mb);
            world.mouseBlock = mb;

            buyMenu = new BuyMenu(world);
            world.Add(buyMenu);
            world.buyMenu = buyMenu;

            camera = new Camera(world.worldSize, player);
            camera.approach.X = -player.position.X - (player.texture.Width / Camera.mod.X);
            camera.approach.Y = -player.position.Y - (player.texture.Height / Camera.mod.Y);
            world.camera = camera;

            world.mode = World.Mode.Buy;
            //player.ResetPlayer(World.Mode.Buy);

        }


        public void DrawHud() {
           // spriteBatch.Draw();
            switch (world.mode) {
                case World.Mode.Buy:
                    spriteBatch.Draw(textBuyMode, new Vector2(123f, 3f)+camera.getPosition(), null, Color.White);
                    // Draw Gold
                    spriteBatch.Draw(sGold, new Vector2(64, 7) + camera.getPosition(), null, Color.White);
                    Sprite.DrawNumber(spriteBatch, sNumbers, goldAmt, new Vector2(74, 6), camera.getPosition(), Color.White);
                    break;
                case World.Mode.Play:

                    // Draw Gold
                    spriteBatch.Draw(sGold, new Vector2(8, 7) + camera.getPosition(), null, Color.White);
                    Sprite.DrawNumber(spriteBatch, sNumbers, goldAmt, new Vector2(18, 6), camera.getPosition(), Color.White);

                    // ---- Draw Selector Hud

                    //Draw outline selector
                    spriteBatch.Draw(sHudContainer, new Vector2(114, 157) + world.camera.getPosition(), null, Color.White); 
                    float gap = 22;
                    spriteBatch.Draw(sHudSelector, new Vector2(115 + ((int)world.blockCurrent * gap), 158) + world.camera.getPosition(), null, Color.White);
                    for (int i = 0; i < world.blockAmt; i++) {


                        //Draw blocks
                        spriteBatch.Draw(world.blockHudSprites[i], new Vector2(117 + (i * gap), 160)+world.camera.getPosition(), null, Color.White);

                        // Draw block amt
                        float minus = 0;
                        if (world.blockInventory[i] >= 10) minus = 5;
                        Sprite.DrawNumber(spriteBatch, sNumbers, world.blockInventory[i], new Vector2(127 + (i*gap) - minus, 169), world.camera.getPosition(), Color.White);

                    }
                    break;
            }
        }


        protected override void Update(GameTime gameTime) {

            Input.GetState();
            //Input.GetTouchState();

            switch (state) {
                case GameState.TitleScreen:
                    
                    break;

                case GameState.InGame:
            

                    if (camera != null)
                        camera.Follow();
        #if !ANDROID
                    float framerate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Window.Title = "Game - " + System.String.Format("{0:0.00}", framerate) + " FPS";

                    if (Input.keyPressed(Keys.F)) {
                        //if (graphics.IsFullScreen) {
                        //    Resolution(wres2, hres2, false);
                        //} else if (!graphics.IsFullScreen) {
                        //    graphics.HardwareModeSwitch = false;
                        //    Resolution(wScr, hScr, true);
                        //}
                        if (scaleInc < scale.Length - 1) {
                            scaleInc++;
                        } else {
                            scaleInc = 0;
                        }
                        int scaleCur = scale[scaleInc];
                        if (scaleCur > 0) {
                            Resolution(SCREEN_WIDTH * scaleCur, SCREEN_HEIGHT * scaleCur, false);
                        } else {
                            graphics.HardwareModeSwitch = false;
                            Resolution(wScr, hScr, true);
                        }
                    }

        #endif
                    if (Input.keyPressed(Keys.F1)) {
                        ToLevel("level1.json");
                    }
                    //if (Input.keyPressed(Keys.F4)) {
                    //    ToLevel("level4.json");
                    //}
                    goldAmt = player.goldAmt;

                    if (levelTransition) {
                        ToLevel(levelFile);
                        levelTransition = false;
                    }



                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();


                    if (world != null) {
                        foreach (Entity entity in world.scene.ToArray()) {
                            entity.Update(gameTime);
                            foreach (Component component in entity.components) {
                                component.Update(gameTime);
                            }
                        }
                    }
                    break;

            }

            /*TransformSystem.Update(gameTime);
            SpriteSystem.Update(gameTime);*/
            //ColliderSystem.Update(gameTime);

            //Input.oldMouseState = Input.newMouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.Black);

            //Debug.WriteLine("Test");

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);//, transformMatrix: _camera.Transform);
            

            switch (state) {
                case GameState.TitleScreen:


                case GameState.InGame:
                    // Background
                    spriteBatch.Draw(sBackground, camera.getPosition(), Color.White);
                    if (world != null) {
                        world.drawLevel(spriteBatch, world.worldDataBkg, world.tilesetBkg);
                        world.drawLevel(spriteBatch, world.worldData, world.tileset);
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

                    DrawHud();
                    /* string test = "test";
                     spriteBatch.DrawString(font1, , new Vector2(0, 0), Color.White, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.5f);*/
                    world.buyMenu.DrawMenu(spriteBatch);
                    break;

            }


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
