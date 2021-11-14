using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPGDGameJam.Game.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework.Input.Touch;
using CPGDGameJam.Game.Entities.Solids;
using CPGDGameJam.Game.Entities.Actors;

namespace CPGDGameJam.Game {
    class MouseBlock : Actor {
        int range = 5;
        public MouseBlock(Texture2D texture, Vector2 pos, World scene) : base(texture, pos, scene) {
            sprite.color = Color.Red;
        }

        public override void Update(GameTime gameTime) {

            Vector2Int posInt = new Vector2Int((int)position.X, (int)position.Y);

            //if (world.backgroundData.Contains(posInt)) {
            //    sprite.color = Color.White;
            //} else {
            //    sprite.color = Color.Red;
            //}
            //if (world.mode == World.Mode.Build) {
            //Util.Log(Util.TileAt(world.player.position, world).ToString() + " ||| " + Util.TileAt(posInt, world).ToString());
            ///if (Util.TileAt(world.player.position, world).Equals(Util.TileAt(posInt, world))){// && world.backgroundData.Contains(Util.TileAt(posInt, world))) {
            if (Util.InDistance(Util.Vector2toInt(world.player.position), posInt, range) && world.blockInventory[(int)world.blockCurrent] > 0 && !world.placedData.Contains(new Vector2(posInt.x, posInt.y)) && world.backgroundData.Contains(posInt) && !world.foregroundData.Contains(posInt) && world.mode != World.Mode.Buy) {
                BlockPlacing();
                sprite.color = Color.White;
            } else {
                sprite.color = Color.Red;
            }
            if (!world.backgroundData.Contains(posInt) || (world.backgroundData.Contains(posInt) && (world.foregroundData.Contains(posInt)) || world.placedData.Contains(new Vector2(posInt.x, posInt.y)))) {
                sprite.texture = Game1.sOutlineX;
            } else {
                sprite.texture = Game1.sOutline;
            }
            //    sprite.visible = true;
            //} else {
            //    sprite.visible = false;
            //}

            position = Input.getMouseTile(world.camera.getPosition());


            transform.position = position;
        }

        public void BlockPlacing() {
            if (Input.Click(0)) {

                foreach (BackgroundTile t in world.scene.OfType<BackgroundTile>().ToArray()) {
                    float a = t.position.X / Game1.GridSize;
                    Vector2 mouseTile = Input.getMouseTile(world.camera.getPosition());
                    int mX = (int)(mouseTile.X - (mouseTile.X % Game1.GridSize)) / Game1.GridSize;
                    int mY = (int)(mouseTile.Y - (mouseTile.Y % Game1.GridSize)) / Game1.GridSize;

                    if ((t.position / Game1.GridSize) == new Vector2(mX, mY)) {
                        Util.Log("Click at: " + a.ToString() + " " + mX.ToString());

                        //world.scene.Add(new Block(Game1.sBlock, mouseTile, this.world));
                        //world.scene.Add(new Collision(mouseTile, new Vector2Int(16, 16), this.world));

                        switch (world.blockCurrent) {
                            case World.BlockType.Block:
                                world.scene.Add(new Block(Game1.sBlock, mouseTile, this.world));
                                world.placedData.Add(mouseTile);
                                break;
                            case World.BlockType.CrumblingBlock:
                                world.scene.Add(new CrumblingBlock(Game1.sCrumblingBlock, mouseTile, this.world));
                                //world.scene.
                                world.placedData.Add(mouseTile);
                                break;
                            case World.BlockType.Ladder:
                                world.scene.Add(new Ladder(Game1.sLadder, mouseTile, this.world));
                                world.placedData.Add(mouseTile);
                                break;
                            case World.BlockType.Spring:
                                world.scene.Add(new Spring(Game1.sSpring, mouseTile, this.world));
                                world.placedData.Add(mouseTile);
                                break;
                        }
                        
                        //MouseBlock mb = this;

                        PutSpriteOnTop();
                    }
                }

                world.blockInventory[(int)world.blockCurrent]--;

            }
        }


    }
}
