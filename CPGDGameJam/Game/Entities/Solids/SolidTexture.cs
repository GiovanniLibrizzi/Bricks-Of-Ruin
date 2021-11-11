using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game.Entities {
    class SolidTexture : Solid {
        Texture2D texture;
        public SolidTexture(Texture2D texture, Vector2 position, World world) : base(position, world) {
            //rectangle.Width = this.texture.Bounds.Width;
            //rectangle.Height = this.texture.Bounds.Height;
            this.texture = texture;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            //this.position = position;

            transform = new Transform(position);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture);
            AddComponent(sprite);
        }
    }


}
