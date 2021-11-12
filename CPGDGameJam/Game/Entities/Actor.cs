using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPGDGameJam.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPGDGameJam.Game.Entities.Solids;

namespace CPGDGameJam.Game.Entities {
    class Actor : Entity {
        public Vector2 position;
        public Texture2D texture;
        public Transform transform;
        public List<AnimatedSprite> spriteList;

        public Vector2 velocity;

        public Sprite sprite;

        protected float friction;
        protected float mspd { get; set; }
        protected float jspd;
        protected float gravity = 0.15f;
        protected const float gravityDef = 0.15f;

        public bool touchingGround;
        protected bool touchingWall;
        protected bool touchingClimbable;
        public Rectangle collisionBox;


        public enum Dir {
            Stop = 0,
            Right = 1,
            Left = -1,
            Up = 2,
            Down = 3,
            Top = 4,
            Bottom = 5

        }

        public Dir direction;




        public Actor(Texture2D texture, Vector2 pos, World scene) : base(scene) {

            position = pos;
            this.texture = texture;


            transform = new Transform(position);
            AddComponent(transform);

            sprite = new Sprite(texture);
            AddComponent(sprite);

        }

        public Actor(Texture2D texture, List<AnimatedSprite> spriteList, Vector2 pos, World scene) : base(scene) {

            position = pos;
            this.texture = texture;

            this.spriteList = spriteList;

            transform = new Transform(position);
            AddComponent(transform);

            sprite = new Sprite(spriteList, 0);
            AddComponent(sprite);

        }

        // Moves actor downwards
        protected void Gravity() {
            if (velocity.Y < 10) {
                velocity.Y += gravity;
            }
        }

        // Stops actor at collision objects 
        protected void CollisionStop() {
            touchingGround = false;
            touchingWall = false;
            touchingClimbable = false;

            foreach (Collision s in world.scene.OfType<Collision>()) {
                if ((velocity.X > 0 && IsTouching(s, Dir.Left)) || (velocity.X < 0 && IsTouching(s, Dir.Right))) {
                    velocity.X = 0;
                    position.X = (float)Math.Floor(position.X);
                    touchingWall = true;
                    //if (s.GetType() == typeof(Climbable)) {
                    //    touchingClimbable = true;

                    //    // Pushes character to latch onto wall (fixes bug where he was a few pixels off)
                    //    if (position.X % 16 != 0) { // HARDCODED TO THE WIDTH, CHANGE THIS IF collisionBox CHANGES!
                    //        position.X += (int)direction;
                    //    }
                    //}
                }

                if (IsTouching(s, Dir.Top)) {
                    touchingGround = true;
                }

                if ((velocity.Y > 0 && IsTouching(s, Dir.Top)) ||
                    (velocity.Y < 0 && IsTouching(s, Dir.Bottom))) velocity.Y = 0;

            }
        }



        // Moves actor the direction specified
        protected void Move (Dir move, float mspd) {
            switch (move) {
                case Dir.Right: MoveRight(mspd); break;
                case Dir.Left: MoveLeft(mspd); break;
                case Dir.Up: MoveUp(mspd); break;
                case Dir.Down: MoveDown(mspd); break;
                case Dir.Stop: StopMoving(); break;
                default: StopMoving(); break;
            }
        }
        private void MoveRight(float mspd) {
            if (velocity.X < mspd - friction) velocity.X += friction;
            else velocity.X = mspd;
        }

        private void MoveLeft(float mspd) {
            if (velocity.X > -mspd + friction) velocity.X -= friction;
            else velocity.X = -mspd;
        }

        private void MoveUp(float mspd) {
            if (velocity.Y > -mspd + friction) velocity.Y -= friction;
            else velocity.Y = -mspd;
        }

        private void MoveDown(float mspd) {
            if (velocity.Y < mspd - friction) velocity.Y += friction;
            else velocity.Y = mspd;
        }

        protected void StopMoving() {
            if (velocity.X >= friction) velocity.X -= friction;
            else if (velocity.X <= -friction) velocity.X += friction;
            else if (velocity.X > -friction && velocity.X < friction) velocity.X = 0;
        }

        protected void Jump(float jspd) {
            velocity.Y = -jspd;
            touchingGround = false;
        }


        // Checks if actor is touching an entity on a certain side of said entity
        protected bool IsTouching(Entity entity, Dir dir) {
            Rectangle entRect;
            Sprite sprite = entity.GetComponent<Sprite>();
            Rectangle myRect = collisionBox;//texture.Bounds;
            Vector2 entPos = entity.GetComponent<Transform>().position;

            if (entity.HasComponent<Sprite>()) {
                entRect = sprite.texture.Bounds;

            } else {
                
                SolidNoTexture s = (SolidNoTexture)entity;
                entRect = new Rectangle(0, 0, s.rectangle.Width, s.rectangle.Height);
            }

            switch (dir) {
                case Dir.Left:
                    return position.X + myRect.Right + velocity.X > entPos.X + entRect.Left &&
                       position.X + myRect.Left < entPos.X + entRect.Left &&
                       position.Y + myRect.Bottom > entPos.Y + entRect.Top &&
                       position.Y + myRect.Top+1 < entPos.Y + entRect.Bottom;
                case Dir.Right:
                    return position.X + (myRect.Left) + velocity.X < entPos.X + entRect.Right &&
                       position.X + myRect.Right > entPos.X + entRect.Right &&
                       position.Y + myRect.Bottom > entPos.Y + entRect.Top &&
                       position.Y + myRect.Top+1 < entPos.Y + entRect.Bottom;

                case Dir.Top:
                    return position.Y + myRect.Bottom + velocity.Y > entPos.Y + entRect.Top &&
                        position.Y + myRect.Top+1 < entPos.Y + entRect.Top &&
                        position.X + myRect.Right > entPos.X + entRect.Left &&
                        position.X + myRect.Left < entPos.X + entRect.Right;
                case Dir.Bottom:
                    return position.Y + (myRect.Top+1) + velocity.Y < entPos.Y + (entRect.Bottom-1) &&
                        position.Y + myRect.Bottom > entPos.Y + entRect.Bottom &&
                        position.X + myRect.Right > entPos.X + entRect.Left &&
                        position.X + myRect.Left < entPos.X + entRect.Right;

                default: return false;

            }
        }
        protected Entity IsTouching(Type entity) {
            //entity.GetType();
            Vector2 posA = position;
            Rectangle boxA = collisionBox;
            foreach (Entity e in world.scene.OfType<Entity>().ToArray()) {
                if (entity.Equals(e.GetType())) {
                    Vector2 posB = e.GetComponent<Transform>().position;
                    Rectangle boxB = e.GetComponent<Sprite>().texture.Bounds;
                    Util.Log(posB.X.ToString() + " | " + posB.Y.ToString());
                    if (posA.X + boxA.Left < posB.X + boxB.Right &&
                           posA.X + boxA.Right > posB.X + boxB.Left &&
                           posA.Y + boxA.Top < posB.Y + boxB.Bottom &&
                           posA.Y + boxA.Bottom > posB.Y + boxB.Top) {
                        return e;
                    }
                }
            }
            return null;
        }
        public static bool Colliding(Vector2 posA, Rectangle boxA, Vector2 posB, Rectangle boxB) {
            return posA.X + boxA.Left < posB.X + boxB.Right &&
                    posA.X + boxA.Right > posB.X + boxB.Left &&
                    posA.Y + boxA.Top < posB.Y + boxB.Bottom &&
                    posA.Y + boxA.Bottom > posB.Y + boxB.Top;
        }

        protected void FlipDirection() {
            if (direction == Dir.Left) direction = Dir.Right; else direction = Dir.Left;
            sprite.Scale(new Vector2((float)direction, 1f));
        }

        public void PutSpriteOnTop() {
            // Draw Sprite on top
            world.scene.Remove(this);
            world.Add(this);
        }

        }
}
