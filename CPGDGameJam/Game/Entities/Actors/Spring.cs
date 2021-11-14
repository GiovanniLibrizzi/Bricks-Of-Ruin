using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
            }

            Gravity();

            CollisionStop();

            position += velocity;
            transform.position = position;
        }

    }
}
