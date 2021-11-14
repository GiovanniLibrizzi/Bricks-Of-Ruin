using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class CrumblingBlock : Block {
        private bool crumbling = false;
        private int cTick = 0;
        private int breakTick = 55;
        private float posX;
        public CrumblingBlock(Texture2D texture, Vector2 position, World world) : base(texture, position, world) {
            posX = position.X;
        }

        public override void Update(GameTime gameTime) {
            if (Actor.IsTouchingPlayer(this, world.player, Actor.Dir.Top)) {

                crumbling = true;
            }

            if (crumbling) {
                cTick++;
                //position.X = 100;
                if (cTick % 2 == 0)
                    position.X = posX + new Random().Next(-1, 1);
                //position.X = new Random().Next(-1, 1);
                if (cTick >= breakTick) {
                    position.Y += Actor.gravityDef*4;
                    if (position.Y > world.worldSize.y + 24) {
                        this.Remove();

                    }
                }
                   // this.Remove();
                
            }
            transform.position = position;
            
        }         
        

    }
}
