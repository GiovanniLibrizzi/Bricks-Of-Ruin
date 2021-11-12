using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities.Solids {
    class BackgroundTile : SolidNoTexture {
        public BackgroundTile(Vector2 position, Vector2Int size, World world) : base(position, size, world) {

        }
    }
}
