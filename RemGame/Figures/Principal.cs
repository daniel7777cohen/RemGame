using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{



    class Principal : Enemy
    {

        public enum Mode { Idle, Patrol, WalkToPlayer, Attack, Evade }// what mode of behavior the monster AI is using 
        private int itrator = 0;
        private bool colorPicked = false;
        private bool isAlive = true;
        Random random;
        private Random freezeLuck;

        private int playerDistanceToAttack;

        private int distance;
        private int oldDistance;

        private PhysicsObject torso;
        private PhysicsObject wheel;

        /////////////////////////
        private PhysicsObject midBody;
        private PhysicsObject tail1;
        private PhysicsObject tail2;

        /////////////////////////

        private PhysicsObject mele;


        private DateTime previousWalk = DateTime.Now;   // time at which we previously jumped
        private const float walkInterval = 3.0f;        // in seconds

        private static Random r = new Random();

        private PhysicsView pv1;
        private PhysicsView pv2;
        private PhysicsView pv3;
        private PhysicsView pv4;
        private PhysicsView pv5;


        private bool playerDetected = false;
        private bool isMeleAttacking = false;

        private RevoluteJoint axis1;
        private RevoluteJoint axis2;
        private RevoluteJoint axis3;
        private RevoluteJoint axis4;

        private int attackingrange;

        private Movement direction = Movement.Right;
        private bool lookingRight = true;


        private Mode mode;
        private Mode previuosMode;

        private int patrolRange;

        private int patrolDirection = 1;

        private DateTime previousWander = DateTime.Now;   // time at which we previously jumped
        private const float wanderInterval = 3.0f;

        private DateTime evaded = DateTime.Now;   // time at which we previously jumped
        private bool dissapear = false;

        private bool endOfPatrol = false;

        private bool wandered = true;


        /// //////////////////////////////Working on new behavoir - Ai//////////////////////////////////////////////////////////////////////////////


        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -6); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(4, 0); // applied force when jumping

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 3.0f;        // in seconds

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[6];


        Texture2D shootTexture;
        private bool evadeRight;
        private bool evadeLeft;

        public Principal(World world, Map map, Kid player, int health, Vector2 size, float mass, float speed, Vector2 startPosition, Point startLocationGrid, SpriteFont f, int inspectionSightRange, float idleInterval, float evasionLuck, int patrolRange, int newDistance, int playerDistanceToAttack) : base(world, map, player, health, size, mass, speed, startLocationGrid, f)
        {
            ////
            this.patrolRange = patrolRange;
            this.playerDistanceToAttack = playerDistanceToAttack;
            mode = Mode.Idle;
            ////
            Vector2 torsoSize = new Vector2(size.X, size.Y);

            // Create the torso
            torso = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;


            int rInt = r.Next(192, 320);
            distance = rInt;
            oldDistance = distance;

            ///////////////////////
            midBody = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            midBody.Position = torso.Position + new Vector2(0, size.Y);
            ///////////////////////


            // Create the feet of the body
            wheel = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            wheel.Position = midBody.Position + new Vector2(0, size.Y);

            ///////////////////////
            tail1 = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            tail1.Position = midBody.Position + new Vector2(size.X * 1.5f, size.Y);


            tail2 = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            tail2.Position = midBody.Position + new Vector2(-size.X * 1.5f, size.Y);
            ///////////////////////

            wheel.Body.Friction = 16.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body, new Body(world));
            JointFactory.CreateAngleJoint(world, midBody.Body, new Body(world));
            JointFactory.CreateAngleJoint(world, tail1.Body, new Body(world));
            JointFactory.CreateAngleJoint(world, tail2.Body, new Body(world));



            axis1 = JointFactory.CreateRevoluteJoint(world, torso.Body, midBody.Body, Vector2.Zero);
            axis1.MotorEnabled = false;


            // Connect the feet to the torso
            axis2 = JointFactory.CreateRevoluteJoint(world, midBody.Body, wheel.Body, Vector2.Zero);
            axis2.CollideConnected = false;
            axis2.MotorEnabled = true;
            axis2.MotorSpeed = 0;
            axis2.MaxMotorTorque = 2;

            axis3 = JointFactory.CreateRevoluteJoint(world, midBody.Body, tail1.Body, Vector2.Zero);
            axis4 = JointFactory.CreateRevoluteJoint(world, midBody.Body, tail2.Body, Vector2.Zero);


            torso.Body.CollisionCategories = Category.Cat20;
            midBody.Body.CollisionCategories = Category.Cat20;
            wheel.Body.CollisionCategories = Category.Cat21;
            tail1.Body.CollisionCategories = Category.Cat21;
            tail2.Body.CollisionCategories = Category.Cat21;


            torso.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            midBody.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            tail1.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            tail2.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;



            torso.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            midBody.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);


            pv1 = new PhysicsView(torso.Body, torso.Position, torso.Size, f);
            pv2 = new PhysicsView(midBody.Body, midBody.Position, torso.Size, f);
            pv3 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);
            pv4 = new PhysicsView(tail1.Body, tail1.Position, wheel.Size, f);
            pv5 = new PhysicsView(tail2.Body, tail2.Position, wheel.Size, f);

            if (AssetsDictionary.getDictionaryUsed())
            {

                Animations[0] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Walk"], 2, 8, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[1] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Walk"], 2, 8, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[2] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Ranged_Chalk"], 4, 8, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[3] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Ranged_Chalk"], 4, 8, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[4] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Stand"], 2, 17, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[5] = new AnimatedSprite(AssetsDictionary.getDictionary()["Principal_Stand"], 2, 17, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                shootTexture = shootTexture = AssetsDictionary.getDictionary()["Chalk"];
            }
            else
            {
                Animations[0] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Walk"), 2, 8, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[1] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Walk"), 2, 8, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[2] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Ranged_Chalk"), 4, 8, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[3] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Ranged_Chalk"), 4, 8, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[4] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Stand"), 2, 17, new Rectangle((int)-size.X * 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                Animations[5] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Stand"), 2, 17, new Rectangle((int)-size.X / 2, (int)(-size.Y * 1.5), 250, 250), 0.05f);
                shootTexture = shootTexture = Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Chalk");

            }

            shootTexture = shootTexture = Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Chalk");

        }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        public Movement Direction { get => direction; set => direction = value; }
        public int Distance { get => distance; set => distance = value; }
        public override Vector2 Position { get => torso.Position; }


        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    lookingRight = false;
                    axis2.MotorSpeed = -MathHelper.TwoPi * speed;
                    anim = animations[0];
                    break;

                case Movement.Right:
                    lookingRight = true;
                    axis2.MotorSpeed = MathHelper.TwoPi * speed;
                    anim = animations[1];

                    break;

                case Movement.Stop:
                    axis2.MotorSpeed = 0;
                    if (!isMeleAttacking)
                    {
                        if (lookingRight)
                            anim = animations[4];
                        else
                            anim = animations[5];
                    }
                    break;
            }
        }

        /*
                public void Jump()
                {
                    if (motion == Act.jump) return;
                    motion = Act.jump;
                    wheel.Body.ApplyLinearImpulse(jumpForce * new Vector2(0, 1));
                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                        torso.Body.ApplyLinearImpulse(jumpForce);
                        previousJump = DateTime.Now;
                    }
                }
         */
        //should create variables for funciton

        public void meleAttack()
        {
            random = new Random();
            double randomInterval = (random.NextDouble() * 10 + 1);

            if ((DateTime.Now - previousShoot).TotalSeconds >= randomInterval)
            {
                isMeleAttacking = true;

                if (lookingRight && anim != Animations[2])
                    anim = Animations[2];
                else if (anim != animations[3])
                    anim = animations[3];

                mele = new PhysicsObject(world, shootTexture, 5, 1);
                mele.Body.CollisionCategories = Category.Cat30;
                mele.Body.CollidesWith = Category.Cat10 | Category.Cat11 | Category.Cat1;

                mele.Body.Mass = 0.4f;
                mele.Body.IgnoreGravity = true;
                mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y);
                int dir;

                if (lookingRight)
                    dir = 1;
                else
                    dir = -1;

                mele.Body.ApplyLinearImpulse(new Vector2(10 * dir, 0));

                mele.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
                previousShoot = DateTime.Now;

            }
            else
                isAttacking = false;

        }


        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            mele.Body.Dispose();
            return true;

        }

        bool HitByPlayer(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat28)
            {
                if (Health > 0)
                {
                    Health--;
                }
                else
                {
                    isAlive = false;
                    torso.Body.Dispose();
                    midBody.Body.Dispose();
                    wheel.Body.Dispose();
                    tail1.Body.Dispose();
                    tail2.Body.Dispose();

                }
                return true;
            }

            return true;

        }


        public void bent()
        {
            torso.Body.IgnoreCollisionWith(wheel.Body);
            torso.Position = wheel.Position;
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition, bool PlayerAlive, int patrolbound)
        {
            if (isAlive)
            {
                bool reached = false;

                if (gridLocation == startLocationGrid)
                    PathFinder.SetMap(map);

                if (anim != null)
                {
                    if (anim.IsLooped)
                    {
                        isMeleAttacking = false;
                        anim.IsLooped = false;
                    }
                }

                if (!isMeleAttacking)
                    anim = Animations[(int)direction];



                UpdateAI();


                // if (isMoving || direction == Movement.Stop) // apply animation
                Anim.Update(gameTime);
                // else //player will appear as standing with frame [1] from the atlas.
                //   Anim.CurrentFrame = 1;

                if (direction == Movement.Right)
                {
                    tail2.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
                    tail1.Body.CollidesWith = Category.None;
                }

                else
                {
                    tail1.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
                    tail2.Body.CollidesWith = Category.None;
                }

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {

            if (patrolGridPath != null)
            {
                Color c = Color.Red;

                if (!colorPicked)
                {
                    switch (x)
                    {
                        case 1:
                            c = Color.Red;
                            break;
                        case 2:
                            c = Color.Purple;
                            break;
                        case 3:
                            c = Color.Pink;
                            break;
                        case 4:
                            c = Color.Plum;
                            break;
                        case 5:
                            c = Color.RosyBrown;
                            break;
                    }
                    x++;
                    if (x == 5)
                        x = 1;
                }
                //DRAWS A* PATH
                /*
                for (int i = 0; i < patrolGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)patrolGridPath[i].X * 64, (int)patrolGridPath[i].Y * 64, 64, 64);
                    if (gridLocation.ToVector2() != patrolGridPath[i])
                        spriteBatch.Draw(shootTexture, gridloc, c);
                    else
                        spriteBatch.Draw(shootTexture, gridloc, Color.Green);
                }
               */
            }

            //dRAWS PATH TO PLAYER

            if (playerGridPath != null)
            {

                for (int i = 0; i < playerGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)playerGridPath[i].X * 64, (int)playerGridPath[i].Y * 64, 40, 40);
                    if (gridLocation.ToVector2() != playerGridPath[i])
                        spriteBatch.Draw(shootTexture, gridloc, Color.Green);
                    else
                        spriteBatch.Draw(shootTexture, gridloc, Color.GreenYellow);
                }

            }

            //torso.Draw(gameTime,spriteBatch);
            Rectangle dest = torso.physicsRectnagleObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;
            if (!torso.Body.IsDisposed && anim != null && lookingRight && !dissapear)
                anim.Draw(spriteBatch, dest, torso.Body, true);
            if (!torso.Body.IsDisposed && anim != null && !lookingRight && !dissapear)
                anim.Draw(spriteBatch, dest, torso.Body, false);



            //pv1.Draw(gameTime, spriteBatch);
            //pv2.Draw(gameTime, spriteBatch);
            //pv3.Draw(gameTime, spriteBatch);
            //pv4.Draw(gameTime, spriteBatch);
            //pv5.Draw(gameTime, spriteBatch);



            if (mele != null && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);

            //wheel.Draw(gameTime,spriteBatch);

            //spriteBatch.DrawString(font, this.GridLocation.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 - 20), Color.White);
            //if (selectedPath != null)
            //  spriteBatch.DrawString(font, selectedPath[selectedPath.Length - 1].ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);


            //spriteBatch.DrawString(font, itrator.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);
            //spriteBatch.DrawString(font, this.position.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);

            spriteBatch.DrawString(font, this.mode.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 40), Color.White);
        }

        public void UpdateAI()
        {

            if (selectedPath == null)
                selectedPath = new Vector2[] { Vector2.Zero };

            //Borders for chcking path to the player,to reduce calculations
            if ((Player.GridLocation.X < GridLocation.X + 50 && Player.GridLocation.X > GridLocation.X - 50) && (Player.GridLocation.Y > 0) && Player.GridLocation != null)
            {
                playerGridPath = findPathToPlayer();
            }

            if (playerGridPath == null)
                playerGridPath = new Vector2[] { gridLocation.ToVector2() };

            if (mode != Mode.Evade)
            {
                if ((playerGridPath.Length < 10 && playerGridPath.Length > 1))//sight range
                {
                    PlayerInAttackRange = true;
                    var trunk = MainDecisionTree();
                    decision = trunk.Evaluate(this);
                    Console.WriteLine(decision);
                    Console.WriteLine(Health);

                    if (decision == "Attack" || decision == "Freeze") //attack range                
                        mode = Mode.Attack;

                    else if (decision == "Evade")
                    {
                        //////maybe needs to be deleted and keep old evade////////////////
                        random = new Random();
                        double randomInterval = (random.NextDouble() * 10 + 1);
                        if (randomInterval < 4 && (DateTime.Now - evaded).TotalSeconds > 8)
                        {
                            mode = Mode.Evade;
                            evaded = DateTime.Now;

                        }
                        else
                        {
                            mode = Mode.Attack;

                        }
                        //////////////////////////////////////////////////////////
                        //mode = Mode.Evade;

                    }

                    else if (mode != Mode.WalkToPlayer)
                        mode = Mode.WalkToPlayer;

                }

                else
                {
                    PlayerInAttackRange = false;

                    if (endOfPatrol)
                    {
                        if (mode != Mode.Idle)
                            mode = Mode.Idle;

                        if ((DateTime.Now - previousWander).TotalSeconds >= wanderInterval)
                        {
                            itrator = 0;
                            endOfPatrol = false;
                            wandered = true;
                        }
                    }

                    else if (mode != Mode.Patrol)
                    {
                        if (mode == Mode.WalkToPlayer)
                            itrator = 0;
                        mode = Mode.Patrol;

                    }

                }

                patrolDirection *= -1;
            }
            //condition needts to change by enemies abilities like attack up or down etc...           
            //if (playerGridPath.Length > 2)//if player not in range do: patrol idle,else do : attack,walktoplayer,evade
            //{            //}

            switch (mode)
            {
                case Mode.Idle:
                    Move(Movement.Stop);
                    direction = Movement.Stop;
                    break;

                case Mode.Patrol:

                    if (itrator == 0 && wandered)
                    {
                        patrolGridPath = findPathToPatrol(patrolDirection * 20);
                        selectedPath = patrolGridPath;
                        endOfPatrol = false;

                    }

                    else if (itrator == selectedPath.Length - 1 && selectedPath[selectedPath.Length - 1] == gridLocation.ToVector2())
                    {
                        endOfPatrol = true;
                        previousWander = DateTime.Now;
                        wandered = false;
                    }
                    break;

                case Mode.WalkToPlayer:
                    itrator = 0;
                    selectedPath = playerGridPath;

                    if (itrator == selectedPath.Length - 1 && selectedPath[selectedPath.Length - 1] == gridLocation.ToVector2())
                    {

                        if (!wandered)
                        {
                            if ((map.isPassable(gridLocation.X + 1, gridLocation.Y) || map.isPassable(gridLocation.X - 1, gridLocation.Y)))
                            {
                                wandered = true;
                                previousWander = DateTime.Now;
                            }
                        }
                        if ((DateTime.Now - previousWander).TotalSeconds >= wanderInterval)
                        {
                            itrator = 0;
                            wandered = false;
                        }

                    }

                    break;

                case Mode.Attack:
                    if (Player.Position.X > Position.X && !lookingRight)
                        Move(Movement.Right);

                    else if (Player.Position.X < Position.X && lookingRight)
                        Move(Movement.Left);

                    Move(Movement.Stop);

                    meleAttack();

                    if (decision == "Freeze")
                        FreezePlayer();

                    break;

                case Mode.Evade:

                    dissapear = true;

                    if (!(evadeRight || evadeLeft))
                    {
                        if (Player.Position.X > Position.X)
                            evadeRight = true;
                        else if (Player.Position.X < Position.X)
                            evadeLeft = true;
                    }

                    if (evadeRight)
                    {
                        if (Position.X < Player.Position.X + 200)
                            Move(Movement.Right);
                        else
                            evadeRight = false;
                    }

                    if (evadeLeft)
                    {
                        if (Position.X > Player.Position.X - 200)
                            Move(Movement.Left);
                        else
                            evadeLeft = false;
                    }
                    /*
                    else if (player.Position.X < Position.X + 200)
                        Move(Movement.Left);
                        */

                    if ((DateTime.Now - evaded).TotalSeconds > 4)
                    {
                        dissapear = false;
                        mode = Mode.WalkToPlayer;
                    }

                    break;

                default:
                    selectedPath = new Vector2[] { gridLocation.ToVector2() };
                    break;

            }

            if (gridLocation.ToVector2() != selectedPath[selectedPath.Length - 1] && (mode == Mode.Patrol || mode == Mode.WalkToPlayer))
            {
                isMoving = true;

                if (gridLocation.ToVector2() == selectedPath[itrator])
                {

                    if (selectedPath[itrator + 1].X > gridLocation.X)
                    {
                        direction = Movement.Right;
                        Move(Movement.Right);
                    }


                    if (selectedPath[itrator + 1].X < gridLocation.X)
                    {
                        direction = Movement.Left;
                        Move(Movement.Left);
                    }

                    if (selectedPath[itrator + 1].Y < gridLocation.Y)
                    {
                        mode = Mode.Idle;
                    }

                    itrator++;

                }

            }
            else if (mode != Mode.Evade)
            {
                Move(Movement.Stop);
                isMoving = false;

            }

        }

        public Vector2[] findPathToPatrol(int dest)
        {
            int maxDestanationValue;
            if (dest < 0)
            {
                maxDestanationValue = r.Next(5, dest * -1);
                maxDestanationValue *= -1;
            }
            else
                maxDestanationValue = r.Next(5, dest);


            Vector2[] arr;
            path = PathFinder.FindPath(gridLocation.ToVector2(), new Vector2(gridLocation.X + maxDestanationValue, gridLocation.Y), "Euclidain");
            if (path == null)
                arr = new Vector2[] { gridLocation.ToVector2() };
            else
                arr = path.ToArray();

            return arr;
        }

        public override Vector2[] findPathToPlayer()
        {
            Vector2[] arr;

            path = PathFinder.FindPath(gridLocation.ToVector2(), Player.GridLocation.ToVector2(), "Manhattan");
            if (path == null)
                arr = new Vector2[] { gridLocation.ToVector2() };
            else
                arr = path.ToArray();

            return arr;
        }

        private void swtichLookingDirection()
        {
            wheel.Body.ApplyLinearImpulse(new Vector2(0, -0.2f));
        }

        private static Decision MainDecisionTree()
        {
            //Decision 4
            var evadeBranch = new DecisionQuery
            {
                Title = "Evade",
                Test = (en) => en.PlayerInAttackRange,
                Positive = new DecisionResult { Result = true, Action = "Evade" },
                Negative = new DecisionResult { Result = false }
            };
            //Decision 3
            var meleeBranch = new DecisionQuery
            {
                Title = "Attack",
                Test = (en) => en.PlayerGridPath.Length < 6,
                Positive = new DecisionResult { Result = true, Action = "Attack" },
                Negative = new DecisionResult { Result = false }
            };
            //Decision 3
            var SpecialAbillity = new DecisionQuery
            {
                Title = "Freeze",
                Test = (en) => en.PlayerGridPath.Length < 8,
                Positive = new DecisionResult { Result = true, Action = "Freeze" },
                Negative = new DecisionResult { Result = false }
            };
            //Decision 2
            var attackBranch = new DecisionQuery
            {
                Title = "Choose Attack Strg",
                Test = (en) => en.isSpecialAbbilityLuck(),
                Positive = SpecialAbillity,
                Negative = meleeBranch
            };

            //Decision 1
            var HealthBranch = new DecisionQuery
            {
                Title = "Have enough health",
                Test = (en) => en.Health > en.Startinghealth / 2,
                Positive = attackBranch,
                Negative = evadeBranch
            };

            //Decision 0
            var trunk = new DecisionQuery
            {
                Title = "Want to attack",
                Test = (en) => en.IsPlayerAlive,
                Positive = HealthBranch,
                Negative = new DecisionResult { Result = false }
            };

            return trunk;
        }
        /*
        private float GetRandomSpeed()
        {
            float x = Game1.rnd.Next(12) - 6;
            if (x > 0) if (x < 4) x = 4;
            if (x < 0) if (x > -4) x = -4;
            return x;
        }
        */

        private void FreezePlayer()
        {
            Player.freeze();
        }
    }
}