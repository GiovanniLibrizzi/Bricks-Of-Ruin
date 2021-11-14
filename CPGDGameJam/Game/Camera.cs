using CPGDGameJam.Game.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPGDGameJam.Game {
    class Camera {

        private Vector2Int worldSize;

        public static Vector2 pos1;

        public static Vector2 mod = new Vector2(2, 2.5f);

        public Matrix Transform;
        public Vector2 approach = new Vector2(0, 0);
        private bool mainCam = false;
        private float speed = 0.1f;
        private float speedMid = 0.02f;
        private float speedSlow = 0.01f;


        private Vector2 minRange = new Vector2((-Game1.SCREEN_WIDTH / mod.X), -Game1.SCREEN_HEIGHT / mod.Y);
        public Actor target;
       
        public Camera(Vector2Int worldSize, Actor target) {
            this.worldSize = worldSize;
            this.target = target;
        }
        //public void Follow(Actor target) {
        //   // this.target = target;
        //    Follow();
        //}

        public void Follow() {
            // Camera moves vertically iff player on ground or wall
            if (target == null) return;
            if (target.GetType() == typeof(MouseBlock)) {
                approach.X = Util.Lerp(approach.X, -target.position.X - (target.texture.Width / mod.X), speedSlow);
                approach.Y = Util.Lerp(approach.Y, -target.position.Y - (target.texture.Height / mod.Y), speedSlow);
            }
            if (target.GetType() == typeof(Player)) {
                Player p = (Player)target;
                if (p.touchingGround || p.state == Player.pState.Ladder) {
                    mainCam = true;
                    if (target.position.Y < (-approach.Y - Game1.SCREEN_HEIGHT) + 128) {
                        approach.Y = Util.Lerp(approach.Y, -target.position.Y - (target.texture.Height / mod.Y), speedMid);
                    } else {
                        approach.Y = Util.Lerp(approach.Y, -target.position.Y - (target.texture.Height / mod.Y), speedMid);
                    }
                } else { mainCam = false; }
                approach.X = Util.Lerp(approach.X, -target.position.X - (target.texture.Width / mod.X), speed);
            }

            // If player is too low or high, speed camera up
            if (!mainCam) {
                float spd;
                if (target.position.Y > -(approach.Y) + 32) {   // below
                    spd = 0.05f;
                } else if (target.position.Y < (-approach.Y - Game1.SCREEN_HEIGHT) + 100) {   // above
                    spd = speedSlow;
                } else {
                    spd = 0f;
                }
                if (spd != 0)
                    approach.Y = Util.Lerp(approach.Y, -target.position.Y - (target.texture.Height / mod.Y), spd);
            }
            //if (target.position.Y < (-approach.Y - Game1.SCREEN_HEIGHT)+128)

            
            //approach.Y = Util.Lerp(approach.Y, -target.position.Y - (target.texture.Height / mod.y), speed);

            // Clamp camera to map bounds
            approach.X = Math.Max(approach.X, -(worldSize.x + minRange.X));
            approach.X = Math.Min(approach.X, minRange.X);

            approach.Y = Math.Max(approach.Y, -worldSize.y - minRange.Y);
            approach.Y = Math.Min(approach.Y, minRange.Y + minRange.Y);


            var position = Matrix.CreateTranslation(
              approach.X,
              approach.Y,
              0);

            var offset = Matrix.CreateTranslation(
                Game1.SCREEN_WIDTH / mod.X,
                Game1.SCREEN_HEIGHT - Game1.SCREEN_HEIGHT / mod.Y,
                0);

            pos1.X = position.M41;
            pos1.X = position.M42;

            Transform = position * offset;
        }
        public Vector2 getMod() {
            return mod;
        }

        public Vector2 getPosition() {
            Vector2 pos;
            pos.X = (Math.Abs(approach.X) - Game1.SCREEN_WIDTH) + (Game1.SCREEN_WIDTH / mod.X);
            pos.Y = (Math.Abs(approach.Y) - Game1.SCREEN_HEIGHT) + (Game1.SCREEN_HEIGHT / mod.Y);
            return pos;
        }
    }
}
