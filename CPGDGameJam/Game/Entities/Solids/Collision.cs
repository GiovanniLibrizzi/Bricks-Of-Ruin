using CPGDGameJam.Game;
using CPGDGameJam.Game.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities {
    class Collision : SolidNoTexture {
        public Collision(Vector2 position, Vector2Int size, World world) : base(position, size, world) {
            
        }
    }
}
