using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class Ladder : SolidTexture {
        private bool crumbling = false;

        public Ladder(Texture2D texture, Vector2 position, World world) : base(texture, position, world) {

        }

        public override void Update(GameTime gameTime) {
            //Util.Log(Actor.Colliding(this.position, this.sprite.texture.Bounds, world.player.position, world.player.collisionBox).ToString());
            if (Actor.Colliding(this.position, this.sprite.texture.Bounds, world.player.position, world.player.collisionBox)) {
                ///Util.Log("ON IT!");
                if (Input.keyDown(Input.Up) || Input.keyDown(Input.Down)) {
                    world.player.state = Player.pState.Ladder;
                }
            } 

            //if (crumbling) {
            //    cTick++;
            //    //position.X = 100;
            //    if (cTick % 2 == 0)
            //        position.X = posX + new Random().Next(-1, 1);
            //    //position.X = new Random().Next(-1, 1);
            //    if (cTick >= breakTick) {
            //        position.Y += Actor.gravityDef * 4;
            //        if (position.Y > world.worldSize.y + 24) {
            //            this.Remove();

            //        }
            //    }
            //    // this.Remove();

            //}
            transform.position = position;

        }


    }
}
