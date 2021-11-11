using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class AnimatedSprite {
        public Texture2D texture;
        public Vector2Int frameSize;
        public int frameAmt;
        public int frameCurrent = 0;
        public int speedInit;
        public int speed;

        public AnimatedSprite(Texture2D texture, Vector2Int frameSize, int speedInit) {
            this.texture = texture;
            this.frameSize = frameSize;
            this.speedInit = speedInit;
            speed = speedInit;
            frameAmt = texture.Width / frameSize.x;
        }

        public Rectangle GetRect(int tick) {
            if (speed != 0) {
                // Sprite Speed Management
                int tickspd = (int)(tick % (speed+Game1.FRAME_RATE/speed));
                // Update current frame
                if (tickspd == 0) frameCurrent++;
                if (frameCurrent == frameAmt) frameCurrent = 0;
            }
            
            // This will be the drawn portion of the sprite
            return new Rectangle((int)(frameSize.x * frameCurrent), 0, frameSize.x, frameSize.y);
        }

    }
}
