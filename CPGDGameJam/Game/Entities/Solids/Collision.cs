using CPGDGameJam.Game;
using CPGDGameJam.Game.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities {
    class Collision : SolidNoTexture {
        Texture2D texture;
        Transform transform;
        protected Sprite sprite;
        public Collision(Vector2 position, Vector2Int size, World world) : base(position, size, world) {
            
        }

        public Collision(Texture2D texture, Vector2 position, World world) : base(position, world) {
            this.texture = texture;
            //size = new Vector2Int(texture.Width, texture.Height);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            //this.position = position;

            transform = new Transform(position);
            AddComponent(transform);

            sprite = new Sprite(texture);
            AddComponent(sprite);
        }
    }
}
