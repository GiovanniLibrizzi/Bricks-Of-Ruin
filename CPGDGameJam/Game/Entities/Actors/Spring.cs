using CPGDGameJam.Game.Entities.Solids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPGDGameJam.Game.Entities.Actors {
    class Spring : Actor {
        public Spring(Texture2D texture, Vector2 pos, World scene) : base(texture, pos, scene) {
            collisionBox = new Rectangle(0, 8, 16, 8);
        }

        public override void Update(GameTime gameTime) {

            if (Colliding(world.player.position, world.player.collisionBox, position, texture.Bounds)) {
                world.player.state = Player.pState.Launched;
                world.player.velocity.Y = -4f;
                Game1.PlaySound(Game1.sfx.spring, 1f, 0f, world.noAudio);
                foreach (CrumblingBlock c in world.scene.OfType<CrumblingBlock>().ToArray()) {
                    if (c.position.X == position.X && !c.crumbling) {
                        c.crumbling = true;
                    }
                }
            
            }

            Gravity();

            CollisionStop();

            position += velocity;
            transform.position = position;
        }

    }
}
