using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities {
    class SolidNoTexture : Solid {

        public Vector2Int size;

        public SolidNoTexture(Vector2 position, Vector2Int size, World world) : base(position, world) {
            this.size = size;
            rectangle = new Rectangle((int)position.X, (int)position.Y, size.x, size.y);
        }

        public SolidNoTexture(Vector2 position, World world) : base(position, world) {
            this.size = new Vector2Int(Game1.GridSize, Game1.GridSize);
            rectangle = new Rectangle((int) position.X, (int) position.Y, size.x, size.y);
    }
}
}
