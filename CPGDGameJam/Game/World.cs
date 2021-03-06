using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
//using static Glide.Content.Util;
using Newtonsoft.Json;
using static Microsoft.Xna.Framework.Content.ContentManager;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using CPGDGameJam.Game.Entities;
using CPGDGameJam.Game.Entities.Solids;
using CPGDGameJam.Game.Entities.Actors;
//using Android.Content.Res;

namespace CPGDGameJam.Game {
    class World {
        public List<Entity> scene = new List<Entity>();

        public enum Mode {
            Buy,
            Play
        }

        public Mode mode = Mode.Buy;

        public int tick = 0;

        public Player player;
        public Camera camera;
        public Actor mouseBlock;
        public BuyMenu buyMenu;


        private string levelFile;

        public Texture2D tileset;
        public Texture2D tilesetBkg;
        public Vector2Int worldSize;
        private Vector2Int tileSize, worldGridSize, tilesetSize;
        private Vector2Int tileSizeBkg, worldGridSizeBkg, tilesetSizeBkg;

        public List<Vector2Int> foregroundData = new List<Vector2Int>();
        public List<Vector2Int> backgroundData = new List<Vector2Int>();
        public List<Vector2> goldData = new List<Vector2>();
        public List<Vector2> placedData = new List<Vector2>();


        public List<int> worldData = new List<int>();
        public List<int> worldDataBkg = new List<int>();

        private List<int> collisionData = new List<int>() { };//1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
        private List<int> climbableData = new List<int>() { };//5, 6, 7, 14, 15, 22, 23, 30, 31, 38, 39 };

        private int levelTriggerData = 37;

        private Vector2Int playerSpawn;
        private Actor.Dir dir;

        public ContentManager content;
        public AnimatedSprite animGold;
        public bool noAudio;
        Texture2D sGoldAnim;

        enum Layer {
            Tiles,
            TilesBkg,
            Entities,
            Gold,
        }

        // Block Info
        public enum BlockType {
            Block,
            CrumblingBlock,
            Ladder,
            Spring,
        }
        public List<Texture2D> blockHudSprites = new List<Texture2D>() { Game1.sHudBlock, Game1.sHudCrumblingBlock, Game1.sHudLadder, Game1.sHudSpring };

        public int blockAmt = 4;
        public List<int> blockPrice = new List<int>() { 2, 1, 4, 8 };
        public List<int> blockInventory = new List<int>() { 0, 0, 0, 0 };
        public List<int> blockInventoryPrev = new List<int>() { 0, 0, 0, 0 };
        public List<int> blockLimit = new List<int>() { 1, 0, 0, 0 };

        public BlockType blockCurrent = BlockType.Block;

        public World (string levelFile, ContentManager content) {
            this.levelFile = levelFile;
            this.content = content;


            // Load
            sGoldAnim = content.Load<Texture2D>("s_gold_anim");
            animGold = new AnimatedSprite(Game1.sGoldAnim, new Vector2Int(8, 8), 10f);


            loadFromJson();

            tilesetSize.x = tileset.Width / tileSize.x;
            tilesetSize.y = tileset.Height / tileSize.y;

            //tilesetSizeBkg.x = tilesetBkg.Width / tileSizeBkg.x;
            //tilesetSizeBkg.y = tilesetBkg.Height / tileSizeBkg.y;


            addCollision();
            addBkg();

            tick = 0;


        }

        public void Add(Player player) {
            scene.Add(player);
            player.position.X = playerSpawn.x;
            player.position.Y = playerSpawn.y;
            player.direction = dir;
        }
        public void Add(Actor actor) {
            scene.Add(actor);
        }
        public void Add(Entity entity) {
            scene.Add(entity);
        }

        private void loadFromJson() {

            string levelPath = string.Format("Content/{0}", levelFile);

            // Android loading works :D
            using (Stream stream = TitleContainer.OpenStream(levelPath))
            using (StreamReader r = new StreamReader(stream)) {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                // Get World Size
                worldSize.x = array.width;
                worldSize.y = array.height;

                blockLimit[0] = array.values.blockLimitBlock;
                blockLimit[1] = array.values.blockLimitCrumblingBlock;
                blockLimit[2] = array.values.blockLimitLadder;
                blockLimit[3] = array.values.blockLimitSpring;

                // -- -- Tile Layer
                dynamic layers = array.layers[(int)Layer.Tiles];

                // Get tileset 
                string name = layers.tileset;
                tileset = content.Load<Texture2D>(name);

                // Get Tile Size
                tileSize.x = layers.gridCellWidth;
                tileSize.y = layers.gridCellHeight;

                // Get world grid size
                worldGridSize.x = layers.gridCellsX;
                worldGridSize.y = layers.gridCellsY;

                // Get world tile data
                foreach (int d in layers.data) {
                    worldData.Add(d);
                }


                layers = array.layers[(int)Layer.TilesBkg];
                // Get tileset 
                string name2 = layers.tileset;
                Util.Log(name2);
                tilesetBkg = content.Load<Texture2D>(name2);

                // Get Tile Size
                tileSizeBkg.x = layers.gridCellWidth;
                tileSizeBkg.y = layers.gridCellHeight;


                // Get world tile data
                foreach (int d in layers.data) {
                    worldDataBkg.Add(d);
                }

                // -- -- Entity Layer
                layers = array.layers[(int)Layer.Entities];

                foreach (dynamic entity in layers.entities) {
                    switch ((string)entity.name) {
                        case "LevelTrigger":
                            scene.Add(new LevelTrigger((string)entity.values.levelNew, new Vector2((float)entity.x, (float)entity.y), new Vector2Int(tileSize.x, tileSize.y), this));
                            break;
                        case "Player":
                            playerSpawn = new Vector2Int((int)entity.x, (int)entity.y);
                            Util.Log(playerSpawn.x.ToString() + "|" + playerSpawn.y.ToString());
                            dir = (Actor.Dir)entity.values.direction;
                            break;
                        case "Ladder":
                            scene.Add(new Ladder(Game1.sLadder, new Vector2((float)entity.x, (float)entity.y), this));
                            break;
                        case "Spring":
                            scene.Add(new Spring(Game1.sSpring, new Vector2((float)entity.x, (float)entity.y), this));
                            break;
                        case "Laser":
                            scene.Add(new Laser(Game1.sLaser, new Vector2((float)entity.x, (float)entity.y), this));
                            break;
                        case "CrumblingBlock":
                            scene.Add(new CrumblingBlock(Game1.sCrumblingBlock, new Vector2((float)entity.x, (float)entity.y), this));
                            break;
                        default: break;
                    }

                }

                layers = array.layers[(int)Layer.Gold];

                foreach (dynamic entity in layers.entities) {
                    switch ((string)entity.name) {
                        case "Gold":
                            Vector2 pos = new Vector2((float)entity.x, (float)entity.y);
                            goldData.Add(pos);
                            scene.Add(new Gold(Game1.sGold, animGold, pos, this));
                            break;
                        default: break;
                    }
                }



            }


        }
        

        public void drawLevel(SpriteBatch spriteBatch, List<int> worldData, Texture2D tileset) {
            
            for (int i = 0; i < worldData.Count; i++) {
                int tileID = worldData[i];

                if (tileID != -1) {
                    int column = tileID % tilesetSize.x;
                    int row = tileID / tilesetSize.x;

                    float x = (i % worldGridSize.x) * tileSize.x;
                    float y = (float)Math.Floor(i / (double)worldGridSize.x) * tileSize.y;
                    //System.Diagnostics.Debug.WriteLine(i + ": tileID: " + tileID + " | column: " + column + " | row: " + row + " | x: " + x + " | y: " + y + " | worldGridSize.x: " + worldGridSize.x);

                    Rectangle tilesetRect = new Rectangle(tileSize.x * column, tileSize.y * row, tileSize.x, tileSize.y);

                    Rectangle tilePosition = new Rectangle((int)x, (int)y, tileSize.x, tileSize.y);
                    spriteBatch.Draw(tileset, tilePosition, tilesetRect, Color.White);
                    

                }
            }
        }

        public void addCollision() {
            for (int i = 0; i < worldData.Count; i++) {
                int tileID = worldData[i];

                if (tileID != -1) {
                    int column = tileID % tilesetSize.x;
                    int row = tileID / tilesetSize.x;

                    float x = (i % worldGridSize.x) * tileSize.x;
                    float y = (float)Math.Floor(i / (double)worldGridSize.x) * tileSize.y;
                    //System.Diagnostics.Debug.WriteLine(i + ": tileID: " + tileID + " | column: " + column + " | row: " + row + " | x: " + x + " | y: " + y + " | worldGridSize.x: " + worldGridSize.x);


                    if (climbableData.Contains(tileID)) {
                        //scene.Add(new Climbable(new Vector2(x, y), new Vector2Int(tileSize.x, tileSize.y), this));

                    } else if (tileID == levelTriggerData) {

                    } else {
                        foregroundData.Add(new Vector2Int((int)x, (int)y));
                        scene.Add(new Collision(new Vector2(x, y), new Vector2Int(tileSize.x, tileSize.y), this));

                    }

                }
            }
        }

        public void addBkg() {
            //Texture2D col = content.Load<Texture2D>("Collision");
            for (int i = 0; i < worldDataBkg.Count; i++) {
                int tileID = worldDataBkg[i];

                if (tileID != -1) {
                    int column = tileID % tilesetSize.x;
                    int row = tileID / tilesetSize.x;

                    float x = (i % worldGridSize.x) * tileSize.x;
                    float y = (float)Math.Floor(i / (double)worldGridSize.x) * tileSize.y;
                    //System.Diagnostics.Debug.WriteLine(i + ": tileID: " + tileID + " | column: " + column + " | row: " + row + " | x: " + x + " | y: " + y + " | worldGridSize.x: " + worldGridSize.x);


                    if (climbableData.Contains(tileID)) {
                        //scene.Add(new Climbable(new Vector2(x, y), new Vector2Int(tileSize.x, tileSize.y), this));

                    } else if (tileID == levelTriggerData) {

                    } else {
                        scene.Add(new BackgroundTile(new Vector2(x, y), new Vector2Int(tileSize.x, tileSize.y), this));
                        backgroundData.Add(new Vector2Int((int)x, (int)y));
                        //scene.Add(new SolidTexture(col, new Vector2(x, y), this));
                    }

                }
            }
        }



        public void ToMode(Mode mode) {
            Mode fromMode = this.mode;
            Util.Log(mode.ToString());
            if (fromMode != mode) {
                Game1.PlaySound(Game1.sfx.menuClick, 1f, 0f, this.noAudio);
            }
            switch (mode) {
                case Mode.Play:
                    
                    if (fromMode == Mode.Buy) {
                        Util.Log("from buy");
                        player.goldAmtPrev = player.goldAmt;
                        blockInventoryPrev = new List<int>(blockInventory);
                        //blockInventoryPrev = blockInventory;
                    } else {
                        Util.Log("from play");
                        Util.Log("inv: " + blockInventoryPrev[0].ToString());
                        blockInventory = new List<int>(blockInventoryPrev);
                    }
                    player.state = Player.pState.Idle;
                    camera.target = player;
                    break;
                case Mode.Buy:
                    if (player.modeSwitched) {
                        player.goldAmt = player.goldAmtPrevPrev;
                    }
                    blockInventory = new List<int>() { 0, 0, 0, 0 };
                    player.state = Player.pState.Paused;
                    camera.target = mouseBlock;
                    break;
            }
            this.mode = mode;
            
        }
    }
}
