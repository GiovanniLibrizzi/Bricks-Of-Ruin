using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class Laser : SolidTexture {
        public Laser(Texture2D texture, Vector2 position, World world) : base (texture, position, world) {

        }

        public override void Update(GameTime gameTime) {
            if (Actor.Colliding(this.position, this.sprite.texture.Bounds, world.player.position, world.player.collisionBox)) {
                world.player.state = Player.pState.PreDeath;
            }
        }
    }
}
