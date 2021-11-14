using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class BuyMenu : Entity {
        public bool enabled = true;

        private int gap = 21;

        public BuyMenu(World world) : base(world) {
            this.world = world;
        }


        public override void Update(GameTime gameTime) {
            //Util.Log("BlockInv: " + world.blockInventory[0]);
            if (world.mode == World.Mode.Buy) {
                for (int i = 0; i < world.blockAmt; i++) {
                    if (Input.keyPressed(Input.numKeys[i])) {
                        BuyBlock((World.BlockType)i);
                        
                    }
                }
            }
        }

        private void BuyBlock(World.BlockType block) {
            if (world.player.goldAmt - world.blockPrice[(int)block] >= 0 && world.blockInventory[(int)block] < world.blockLimit[(int)block]) {
                world.blockInventory[(int)block]++;
                world.player.goldAmt -= world.blockPrice[(int)block];
                Game1.PlaySound(Game1.sfx.menuClick, 1f, 0.3f, world.noAudio);
                world.blockCurrent = block;
                Util.Log("Purchased: " + block.ToString() + " | curAmt: " + world.blockInventory[(int)block].ToString());
            }
        }
        public void DrawMenu(SpriteBatch spriteBatch) {
            if (world.mode == World.Mode.Buy) {
                // Draw main pre-made menu
                spriteBatch.Draw(Game1.sHudBuyMenu, new Vector2(0, 0) + world.camera.getPosition(), null, Color.White);

                //Draw the block to be purchased
                for (int i = 0; i < world.blockAmt; i++) {
                    Color c = Color.White;
                    if (world.blockInventory[i] == world.blockLimit[i]) {
                        c = Color.Red;
                    } else if (world.blockInventory[i] == world.blockLimit[i]-1) {
                        c = Color.Orange;
                    }


                    spriteBatch.Draw(world.blockHudSprites[i], new Vector2(39, 1 + (i*gap))+world.camera.getPosition(), null, Color.White);
                //}

                    // Draw the block's price
                    Sprite.DrawNumber(spriteBatch, Game1.sNumbers, world.blockPrice[i], new Vector2(23, 5 + (i * gap)), world.camera.getPosition(), Color.White);
                 
                    float xt;
                    if (world.blockInventory[i] < 10) {
                        xt = 49;
                    } else {
                        xt = 43;
                    }
                    Sprite.DrawNumber(spriteBatch, Game1.sNumbers, world.blockInventory[i], new Vector2(xt, 10 + (i * gap)), world.camera.getPosition(), c);

                    // Draw slash
                    int slashX = 55;
                    if (world.blockLimit[i] >= 10) {
                        slashX += 5;
                    }
                    spriteBatch.Draw(Game1.sHudSlash, new Vector2(slashX, 13 + (i*gap)) + world.camera.getPosition(), null, c);

                    // Draw block limit
                    Sprite.DrawNumber(spriteBatch, Game1.sNumbersSmall, world.blockLimit[i], new Vector2(slashX + 5, 14 + (i * gap)), world.camera.getPosition(), c);

                }
            }
        }
    }
}
