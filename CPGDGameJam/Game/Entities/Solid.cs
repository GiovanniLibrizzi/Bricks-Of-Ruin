using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities {
    class Solid : Entity {
        public Vector2 position;
        //public Vector2Int size;
        //public Texture2D texture { get; set; }
        public Transform transform;
        public Rectangle rectangle = new Rectangle();

        public Solid(Vector2 position, World world) : base(world) {
            this.position = position;
            //this.size = size;
            //rectangle = new Rectangle((int)position.X, (int)position.Y, size.x, size.y);

            transform = new Transform(position);
            AddComponent(transform);

        }




    }
}
