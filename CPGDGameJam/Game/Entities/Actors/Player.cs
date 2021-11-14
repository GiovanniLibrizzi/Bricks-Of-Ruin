using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPGDGameJam.Game.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework.Input.Touch;
using CPGDGameJam.Game.Entities.Solids;
using CPGDGameJam.Game.Entities.Actors;

namespace CPGDGameJam.Game {
    class Player : Actor {

        public enum pState {
            Idle,
            Jump,
            Ladder,
            Launched,
            PreDeath,
            Death,
            Paused
        }

        public int tick = 0;

        public pState state;
        //private pState state = pState.Idle;
        public pState statePrev;// = pState.Idle;

        private Vector2 startingPosition;
        private Dir startingDir;


        private const float mspdClimb = 0.8f;
        private const float mspdBonk = 1f;
        private const float mspdLadder = 0.5f;

        private const float gravLadder = 0f, gravGlide = 0.03f, gravClimb = 0f, gravBonk = 0.3f;
        private float[] grav = { gravityDef, gravityDef, gravLadder, gravityDef, gravityDef, gravityDef, 0f };

        private const float fricDef = 0.75f;
        private const float fricAir = 0.75f;

        private Vector2Int mousePos;
        private Vector2Int mouseTilePos;

        public int goldAmtPrevPrev = 0;
        public int goldAmtPrev = 0;
        public int goldAmt;

        public bool modeSwitched = false;

        public enum pSprite {
            Idle,
            Jump,
        }
        private pSprite spriteCurrent = pSprite.Idle;
        //private const float gravGlide = 0.05f;
        //private const float gravBonk = 0.3f;

        public Player(Texture2D texture, List<AnimatedSprite> spriteList, Vector2 pos, World scene) : base(texture, spriteList, pos, scene) {
            mspd = 1.0f;
            jspd = 3.3f;
            friction = fricDef;
            direction = Dir.Right;

            collisionBox = new Rectangle(0, 0, 16, 16);
        }

        public override void Update(GameTime gameTime) {
            // Initialize on runtime
            if (tick == 0) {
                
                //Util.Log(direction.ToString() + "bruh");
                sprite.Scale(new Vector2((float)direction, 1f));
                startingPosition = position;
                startingDir = direction;
                world.ToMode(World.Mode.Buy);


                //sWalking = new AnimatedSprite(texture, new Vector2Int(8, 8), 4);
            }
            sprite.spriteCurrent = (int)spriteCurrent;


            gravity = grav[(int)state];

            modeSwitched = false;

            // Check if touching gold
            Entity e = IsTouching(typeof(Gold));
            if (e != null) {
                goldAmt++;
                e.Remove();
            }

            // Player States
            #region Player States
            switch (state) {
                #region Idle
                case pState.Idle:
                    spriteCurrent = pSprite.Idle;
                    //Util.Log(Util.TileAt(position, world).ToString());
                    Movement(mspd);


                    //BlockPlacing();
                    
                    //if (Math.Abs(velocity.X) > 0.1) spriteCurrent = pSprite.Land;
                    //if (touchingGround) friction = fricDef; else friction = fricAir;
                    //StopMoving(); // Horizontally


                    // Transitions
                    if ((Input.keyPressed(Input.Jump)) && touchingGround) {
                        Jump(jspd);
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region Jump
                case pState.Jump:
                    //spriteCurrent
                    //if (touchingGround)
                    //    StopMoving(); // Horizontally
                    Movement(mspd);
                    //BlockPlacing();

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
                    
                    if (touchingGround) {
                        StateGoto(pState.Idle);
                    }

                    //if (touchingClimbable) {
                    //    //velocity.X = 0;
                    //    velocity.Y = 0;
                    //    //position.X += (int)direction*2;
                    //    StateGoto(pState.WallClimb);
                    //}
                    break;

                #endregion
                #region Paused/Build Mode
                case pState.Paused:
                    if (Input.keyPressed(Input.ModeSwap) && !modeSwitched) {
                        Util.Log("pressed");
                        modeSwitched = true;
                        world.ToMode(World.Mode.Play);
                    }
                    break;
                #endregion
                #region Ladder
                case pState.Ladder:
                    if (Input.keyDown(Input.Up)) {
                        Move(Dir.Up, mspdLadder);
                    } 
                    if (Input.keyDown(Input.Down)) {
                        Move(Dir.Down, mspdLadder);
                    } 
                    if (!Input.keyDown(Input.Up) && !Input.keyDown(Input.Down)) {
                        StopMovingY();
                    }
                    Movement(mspdLadder);

                    if (Input.keyPressed(Input.Jump)) {
                        Jump(jspd);
                        StateGoto(pState.Jump);
                    }
                    bool touchingLadder = false;
                    foreach (Ladder l in world.scene.OfType<Ladder>().ToArray()) {
                        if (Actor.Colliding(l.position, l.sprite.texture.Bounds, position, collisionBox)) {
                            touchingLadder = true;
                        }
                    }
                    if (!touchingLadder) {
                        StateGoto(pState.Jump);
                    }
                    break;
                #endregion
                #region Launched
                case pState.Launched:
                    Movement(mspd / (float)1.2);
                    if (touchingGround) {
                        StateGoto(pState.Idle);
                    }
                    break;
                #endregion
                #region PreDeath
                case pState.PreDeath:
                    Util.Log("(pre) Death." + world.blockInventoryPrev[0].ToString());
                    world.blockInventory = world.blockInventoryPrev;
                    ResetPlayer(World.Mode.Play);
                    break;
                    #endregion
            }
            #endregion

            // Select block in play mode
            if (world.mode == World.Mode.Play) {
                for (int i = 0; i < world.blockAmt; i++) {
                    if (Input.keyPressed(Input.numKeys[i])) {
                        world.blockCurrent = (World.BlockType)i;
                    }
                }
            }

            // RESET BUTTON
            if (Input.keyPressed(Input.ModeSwap) && !modeSwitched) {
                ResetPlayer(World.Mode.Buy);
            }


            // Check if out of bounds
            if (position.Y > world.worldSize.y + 24) {
                StateGoto(pState.PreDeath);
                
            }

            if (world.mode == World.Mode.Buy) {
                BuyMenu();
            }

            TouchingLevelTrigger();
            Gravity();

            CollisionStop();


            position += velocity;

            transform.position = position;
            tick += 1;
        }

        private void Movement(float mspd) {
            if (Input.keyDown(Input.Right)) {
                Move(Dir.Right, mspd);
            }
            if (Input.keyDown(Input.Left)) {
                Move(Dir.Left, mspd);
            }

            if (!Input.keyDown(Input.Left) && !Input.keyDown(Input.Right)) {
                StopMovingX();
            }
        }


        public void ResetPlayer(World.Mode toMode) {
            if (!modeSwitched) {
                foreach (Block b in world.scene.OfType<Block>().ToArray()) {
                    if (world.placedData.Contains(b.position)) {
                        b.Remove();
                    }
                }
                foreach (Gold g in world.scene.OfType<Gold>().ToArray()) {
                    if (world.placedData.Contains(g.position)) {
                        g.Remove();
                    }
                }
                foreach (Ladder l in world.scene.OfType<Ladder>().ToArray()) {
                    if (world.placedData.Contains(l.position)) {
                        l.Remove();
                    }
                }
                foreach (Spring s in world.scene.OfType<Spring>().ToArray()) {
                    if (world.placedData.Contains(s.position)) {
                        s.Remove();
                    }
                }
                foreach (Vector2 v in world.goldData) {
                    world.scene.Add(new Gold(Game1.sGold, world.animGold, v, world));
                }
                world.placedData.Clear();


                StateGoto(pState.Idle);

                goldAmt = goldAmtPrev;
                position = startingPosition;
                direction = startingDir;
                velocity = new Vector2(0, 0);
                tick = 0;
                modeSwitched = true;
                world.ToMode(toMode);
            }

        }

        private void BuyMenu() {
            if (Input.keyPressed(Input.One)) {

            }
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

        public void StateGoto(pState newState) {
            tick = 0;
            statePrev = state;
            state = newState;
        }

    }
}
