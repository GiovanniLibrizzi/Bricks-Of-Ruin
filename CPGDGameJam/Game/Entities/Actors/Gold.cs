using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class Gold : Actor {
        AnimatedSprite animatedSprite;
        Random r = new Random();
        public int random => r.Next(0, 60);

        public Gold(Texture2D texture, AnimatedSprite animatedSprite, Vector2 position, World world) : base(texture, position, world) {
            this.animatedSprite = animatedSprite;
            RemoveComponent(sprite);
            sprite = new Sprite(animatedSprite);
            AddComponent(sprite);
            
            animatedSprite.tick = random;

            collisionBox = new Rectangle(0, 0, 8, 8);
                

        }

        public override void Update(GameTime gameTime) {
            //animatedSprite.speed = 0;
            //Util.Log(animatedSprite.speed.ToString());
        }


    }
}
