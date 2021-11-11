using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPGDGameJam.Game.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework.Input.Touch;

namespace CPGDGameJam.Game {
    class Player : Actor {

        public enum pState {
            Idle,
            Jump,
            Glide,
            WallClimb,
            Land,
            Bonk
        }

        private int tick = 0;

        public pState state { get; private set; }
        //private pState state = pState.Idle;
        private pState statePrev = pState.Idle;

        private Vector2 startingPosition;
        private Dir startingDir;


        private const float mspdClimb = 0.8f;
        private const float mspdBonk = 1f;


        private const float gravGlide = 0.03f, gravClimb = 0f, gravBonk = 0.3f;
        private float[] grav = { gravityDef, gravityDef, gravGlide, gravClimb, gravityDef, gravBonk };

        private const float fricDef = 0.11f;
        private const float fricAir = 0.06f;

        public enum pSprite {
            Idle,
            Glide,
            Climb,
            Land
        }
        private pSprite spriteCurrent = pSprite.Idle;
        //private const float gravGlide = 0.05f;
        //private const float gravBonk = 0.3f;

        public Player(Texture2D texture, List<AnimatedSprite> spriteList, Vector2 pos, World scene) : base(texture, spriteList, pos, scene) {
            mspd = 2.0f;
            jspd = 6.6f;
            friction = fricDef;
            direction = Dir.Right;
            startingPosition = position;
            startingDir = direction;

            collisionBox = new Rectangle(0, 0, 16, 16);
        }

        public override void Update(GameTime gameTime) {

            // Initialize on runtime
            if (tick == 0) {
                //Util.Log(direction.ToString() + "bruh");
                sprite.Scale(new Vector2((float)direction, 1f));
                //sWalking = new AnimatedSprite(texture, new Vector2Int(8, 8), 4);
            }
            sprite.spriteCurrent = (int)spriteCurrent;
          
            // debug reset
            if (Input.keyPressed(Keys.R)) {
                position = startingPosition;
                direction = startingDir;
                velocity = new Vector2(0, 0);
                tick = 0;
            }

            gravity = grav[(int)state];
            //if (touchingGround) friction = fricDef; else friction = fricAir;

            // Player States
            #region Player States
            switch (state) {
                #region Idle
                case pState.Idle:
                    spriteCurrent = pSprite.Idle;
                    if (Math.Abs(velocity.X) > 0.1) spriteCurrent = pSprite.Land;
                    if (touchingGround) friction = fricDef; else friction = fricAir;
                    StopMoving(); // Horizontally
                    // Transitions
                    if ((Input.keyPressed(Input.Jump)) && touchingGround) {
                        Jump(jspd);
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region Jump
                case pState.Jump:
                    spriteCurrent = pSprite.Idle;
                    if (touchingGround)
                        StopMoving(); // Horizontally

                    // Variable jump height
                    if (velocity.Y < 0) {
                        if (!Input.keyPressed(Input.Jump)) {
                            velocity.Y = Util.Lerp(velocity.Y, 0, 0.05f);
                        }
                        if (Input.keyReleased(Input.Jump)) {
                            velocity.Y = Util.Lerp(velocity.Y, 0, 0.3f);
                        }
                    }

                    //Transitions
                    if (Input.keyPressed(Input.Jump) && !touchingGround) {
                        velocity.Y = 0;
                        StateGoto(pState.Glide);
                    }

                    if (touchingGround) {
                        StateGoto(pState.Idle);
                    }

                    if (touchingClimbable) {
                        //velocity.X = 0;
                        velocity.Y = 0;
                        //position.X += (int)direction*2;
                        StateGoto(pState.WallClimb);
                    }
                    break;
                #endregion
                #region Glide
                case pState.Glide:
                    spriteCurrent = pSprite.Glide;
                    //spriteList[(int)spriteCurrent].speed
                    Move(direction, mspd);

                    velocity.Y = Math.Max(velocity.Y, -0.1f);
                    velocity.Y = Math.Min(velocity.Y, 0.95f);
                    // Transitions
                        // Bonk
                    if (touchingWall && !touchingClimbable && Math.Abs(velocity.X) > 0f) {
                        velocity.X = -(int)direction * mspdBonk;
                        velocity.Y = -2f;
                        FlipDirection();
                        StateGoto(pState.Bonk);
                    }

                    if (touchingClimbable) {
                        //velocity.X = 0;
                        velocity.Y = 0;
                        //position.X += (int)direction*2;
                        StateGoto(pState.WallClimb);
                    }

                    if (touchingGround) {
                        if (touchingWall && !touchingClimbable) {
                            FlipDirection();
                        }
                        StateGoto(pState.Land);
                    }

                    //if (Input.keyPressed(Input.Jump) && !touchingGround) {
                    //    StateGoto(pState.Glide);
                    //}

                    if (Input.keyPressed(Input.Jump)) {
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region WallClimb
                case pState.WallClimb:
                    spriteCurrent = pSprite.Climb;
                    velocity.X = (int)direction*3f;

                    //if (Input.keyDown(Input.Up)) {
                        Move(Dir.Up, mspdClimb/1.15f);
                    //}
                    //if (Input.keyDown(Input.Down)) {
                    //    Move(Dir.Down, mspdClimb);
                    //    //Util.Log("DOWN!");
                    //}

                    sprite.animatedSprite.speed = (int)(velocity.Y*8);

                    // Transitions
                    if (tick > 4 && !touchingClimbable) {
                        //if (!touchingWall) {
                        Launch((int)direction * 2f, -1f);
                        //velocity.X = (int)direction * 1.5f;
                        //velocity.Y = 2f;
                        StateGoto(pState.Idle);
                    }

                    if (Input.keyPressed(Input.Jump)) {
                        FlipDirection();
                        velocity.X = 2 * (int)direction;
                        Jump(jspd/1.5f);
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region Land
                case pState.Land:
                    spriteCurrent = pSprite.Land;
                    //velocity.X = Util.Lerp(velocity.X, 0, 0.1f);
                    StopMoving();
                    if (Math.Abs(velocity.X) < 0.1) {
                        velocity.X = 0;
                        StateGoto(pState.Idle);
                    }
                    if ((Input.keyPressed(Input.Jump)) && touchingGround) {
                        Jump(jspd);
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region Bonk
                case pState.Bonk:
                    if (touchingGround)
                        StateGoto(pState.Idle);
                    break;
                #endregion
            }
            #endregion



            //Util.Log("state: " + state + " || touchingClimbable: " + touchingClimbable);
            // Movement Controls
            /*if (Input.keyDown(Input.Right)) {
                Move(Dir.Right);
            }
            if (Input.keyDown(Input.Left)) {
                Move(Dir.Left);
            }
            if (!Input.keyDown(Input.Right) && !Input.keyDown(Input.Left)) {
                Move(Dir.Stop);
            }*/

            // Jump
            /*if (Input.keyPressed(Input.Jump) && onGround) {
                System.Diagnostics.Debug.WriteLine("JUMP!");
                Jump();
            }*/

            // Jump height variation
            /*if (velocity.Y < 0) {
                if (!Input.keyPressed(Input.Jump)) {
                    velocity.Y = Util.Lerp(velocity.Y, 0, 0.05f);
                }
                if (Input.keyReleased(Input.Jump)) {
                    velocity.Y = Util.Lerp(velocity.Y, 0, 0.5f);
                }
            }*/
            TouchingLevelTrigger();
            Gravity();

            CollisionStop();


            position += velocity;

            transform.position = position;
            tick += 1;
        }

        protected void TouchingLevelTrigger() {
            foreach (LevelTrigger s in world.scene.OfType<LevelTrigger>()) {
                if (Colliding(this.position, this.texture.Bounds, s.position, Util.RemoveRectPos(s.rectangle))) {
                    Game1.LevelTransition(s.worldNew);
                    //Game1.levelTransition = true;
                }
            }
        }
        
        private void Launch(Vector2 v) {
            velocity.X = v.X;
            velocity.Y = v.Y;
        }
        private void Launch(float h, float v) {
            velocity.X = h;
            velocity.Y = v;
        }

        private void StateGoto(pState newState) {
            tick = 0;
            statePrev = state;
            state = newState;
        }

    }
}
