using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class NumberSprite {

        private Texture2D texture;
        private int number;
        public NumberSprite(Texture2D texture, int number) {
            this.texture = texture;
            this.number = number;
        }
    }
}
