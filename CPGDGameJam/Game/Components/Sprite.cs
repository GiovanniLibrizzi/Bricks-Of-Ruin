using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class Sprite : Component {
        public Texture2D texture { get; set; }
        public Vector2 scale = new Vector2(1f, 1f);
        public List<AnimatedSprite> spriteList;
        public int spritePrevious = -1;
        public int spriteCurrent;
        public AnimatedSprite animatedSprite;
        public SpriteEffects spriteEffect;
        public int tick = 0;


        public Sprite(Texture2D texture) {
            this.texture = texture;
            SpriteSystem.Register(this);
        }

        public Sprite(AnimatedSprite animatedSprite) {
            this.texture = animatedSprite.texture;
            this.animatedSprite = animatedSprite;
            SpriteSystem.Register(this);
        }

        public Sprite(List<AnimatedSprite> spriteList, int spriteCurrent) {
            this.spriteList = spriteList;
            this.spriteCurrent = spriteCurrent;
            animatedSprite = spriteList[spriteCurrent];
            texture = animatedSprite.texture;

            SpriteSystem.Register(this);
        }


        public void Scale(Vector2 sc) {
            scale = new Vector2(Math.Abs(sc.X), Math.Abs(sc.Y));

            // Flip if scale is less than 0
            if (sc.X < 0) {
                spriteEffect = SpriteEffects.FlipHorizontally;
            } else {
                spriteEffect = SpriteEffects.None;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            Transform t = entity.GetComponent<Transform>();

            // Resets frame count if changing sprites
            if (spriteList != null) {
                if (spritePrevious != spriteCurrent) {
                    animatedSprite = spriteList[spriteCurrent];
                    animatedSprite.speed = spriteList[spriteCurrent].speedInit;
                    animatedSprite.frameCurrent = 0;
                    //animatedSprite.
                }
            }

            if (animatedSprite != null) {
                tick++;
                spriteBatch.Draw(animatedSprite.texture, t.position, animatedSprite.GetRect(tick), Color.White, 0f, Vector2.Zero, scale, spriteEffect, 0f);
            } else if (texture != null) {
                spriteBatch.Draw(texture, t.position, null, Color.White, 0f, Vector2.Zero, scale, spriteEffect, 0f);
            }
            //Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale
            spritePrevious = spriteCurrent;
        }

    }
}
