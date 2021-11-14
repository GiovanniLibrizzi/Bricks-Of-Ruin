using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class Block : Collision {
        public Vector2 initialPosition;
        public Block(Texture2D texture, Vector2 position, World world) : base(texture, position, world) {
            initialPosition = new Vector2(position.X, position.Y);
        }
        
    }
}
