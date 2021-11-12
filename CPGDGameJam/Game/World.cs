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
//using Android.Content.Res;

namespace CPGDGameJam.Game {
    class World {
        public List<Entity> scene = new List<Entity>();

        public enum Mode {
            Build,
            Play
        }

        public Mode mode = Mode.Build;

        public Player player;
        public Camera camera;
        public Actor mouseBlock;
        

        private string levelFile;

        public Texture2D tileset;
        public Texture2D tilesetBkg;
        public Vector2Int worldSize;
        private Vector2Int tileSize, worldGridSize, tilesetSize;
        private Vector2Int tileSizeBkg, worldGridSizeBkg, tilesetSizeBkg;

        public List<Vector2Int> foregroundData = new List<Vector2Int>(); 
        public List<Vector2Int> backgroundData = new List<Vector2Int>();
        public List<Vector2> goldData = new List<Vector2>();
        public List<Vector2Int> placedData = new List<Vector2Int>();


        public List<int> worldData = new List<int>();
        public List<int> worldDataBkg = new List<int>();

        private List<int> collisionData = new List<int>() { };//1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
        private List<int> climbableData = new List<int>() { };//5, 6, 7, 14, 15, 22, 23, 30, 31, 38, 39 };

        private int levelTriggerData = 37;

        private Vector2Int playerSpawn;
        private Actor.Dir dir;

        public ContentManager content;

        enum Layer {  
            Tiles,
            TilesBkg,
            Entities,
            Gold,
        }

        public World (string levelFile, ContentManager content) {
            this.levelFile = levelFile;
            this.content = content;


            loadFromJson();

            tilesetSize.x = tileset.Width / tileSize.x;
            tilesetSize.y = tileset.Height / tileSize.y;

            //tilesetSizeBkg.x = tilesetBkg.Width / tileSizeBkg.x;
            //tilesetSizeBkg.y = tilesetBkg.Height / tileSizeBkg.y;


            addCollision();
            addBkg();

        }

        public void Add(Player player) {
            scene.Add(player);
            player.position.X = playerSpawn.x;
            player.position.Y = playerSpawn.y;
            player.direction = dir;
        }
        public void Add(Actor actor) {
            scene.Add(actor);
            //actor.position.X = playerSpawn.x;
            //player.position.Y = playerSpawn.y;
            //player.direction = dir;
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
                        default: break;
                    }

                }

                layers = array.layers[(int)Layer.Gold];

                foreach (dynamic entity in layers.entities) {
                    switch ((string)entity.name) {
                        case "Gold":
                            Vector2 pos = new Vector2((float)entity.x, (float)entity.y);
                            goldData.Add(pos);
                            scene.Add(new Gold(Game1.sGold, pos, this));
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
            switch (mode) {
                case Mode.Play:
                    player.state = Player.pState.Idle;
                    break;
                case Mode.Build:
                    player.state = Player.pState.Paused;
                    break;
            }
            this.mode = mode;
        }
    }
}
