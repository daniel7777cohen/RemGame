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
    class groundedEnemy : Enemy
    {
        private int itrator = 0;
        private bool colorPicked = false;
        private bool isAlive = true;
        Random random;
        private int playerDistanceToAttack;
        private int distance;
        private int oldDistance;
        private PhysicsObject torso;
        private PhysicsObject wheel;

        /////////////////////////
        private PhysicsObject midBody;

        /////////////////////////

        private PhysicsObject mele;

        private DateTime previousWalk = DateTime.Now;   // time at which we previously jumped
        private const float walkInterval = 3.0f;        // in seconds

        private static Random r = new Random();

        private PhysicsView pv1;
        private PhysicsView pv2;
        private PhysicsView pv3;

        private bool playerDetected = false;
        private bool playerInAttackRange = false;
        private bool isMeleAttacking = false;

        private RevoluteJoint axis1;
        private RevoluteJoint axis2;
        private RevoluteJoint axis3;
        private RevoluteJoint axis4;

        private int attackingrange;

        private Movement direction = Movement.Right;
        private bool lookingRight = true;
        private bool isInAir;

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
        private const float jumpInterval = 0.8f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -6); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(4, 0); // applied force when jumping

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 3.0f;        // in seconds

        private DateTime previousStuckCheck = DateTime.Now;   // time at which we previously jumped
        private const float stuckCheckInterval = 1.5f;

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[6];


        Texture2D shootTexture;
        private bool isJumping;

        public groundedEnemy(World world, Map map, Kid player, int health, Vector2 size, float mass, float speed, Vector2 startPosition, Point startLocationGrid, SpriteFont f, int inspectionSightRange, float idleInterval, float evasionLuck, int patrolRange, int newDistance, int playerDistanceToAttack) : base(world, map, player, health, size, mass, speed, startLocationGrid, f)
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

            wheel.Body.Friction = 16.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body, new Body(world));
            JointFactory.CreateAngleJoint(world, midBody.Body, new Body(world));

            axis1 = JointFactory.CreateRevoluteJoint(world, torso.Body, midBody.Body, Vector2.Zero);
            axis1.MotorEnabled = false;


            // Connect the feet to the torso
            axis2 = JointFactory.CreateRevoluteJoint(world, midBody.Body, wheel.Body, Vector2.Zero);
            //axis2.CollideConnected = false;
            axis2.MotorEnabled = true;
            axis2.MotorSpeed = 0;
            axis2.MaxMotorTorque = 20;

            torso.Body.CollisionCategories = Category.Cat20;
            midBody.Body.CollisionCategories = Category.Cat20;
            wheel.Body.CollisionCategories = Category.Cat21;

            torso.Body.CollidesWith = Category.Cat28 | Category.Cat7;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            midBody.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;


            torso.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            midBody.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);


            pv1 = new PhysicsView(torso.Body, torso.Position, torso.Size, f);
            pv2 = new PhysicsView(midBody.Body, midBody.Position, torso.Size, f);
            pv3 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);


            Animations[0] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Other/playerLeft"), 1, 4, new Rectangle((int)-size.X / 2, (int)(-size.Y / 2), 150, 200), 0.25f);
            Animations[1] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Other/playerRight"), 1, 4, new Rectangle((int)-size.X / 2, (int)(-size.Y / 2), 150, 200), 0.25f);

            shootTexture = shootTexture = Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Chalk");
            isInAir = false;

        }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        public Movement Direction { get => direction; set => direction = value; }
        public int Distance { get => distance; set => distance = value; }
        public override Vector2 Position { get => torso.Position; }
        public bool IsInAir { get => isInAir; set => isInAir = value; }



        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    if (!IsInAir)
                    {
                        lookingRight = false;
                        axis2.MotorSpeed = -MathHelper.TwoPi * speed;
                        anim = animations[0];
                    }
                    break;

                case Movement.Right:
                    if (!IsInAir)
                    {
                        lookingRight = true;
                        axis2.MotorSpeed = MathHelper.TwoPi * speed;
                        anim = animations[1];
                    }
                    break;

                case Movement.Stop:
                    axis2.MotorSpeed = 0;
                    break;
            }
        }

        public void Jump()
        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval && !isInAir)
            {
                isJumping = true;
                if (direction == Movement.Right)
                {
                    wheel.Body.ApplyLinearImpulse(jumpForce * new Vector2(0, -15));
                }
                else
                {
                    wheel.Body.ApplyLinearImpulse(jumpForce * new Vector2(0, -15));
                }

                previousJump = DateTime.Now;
            }
        }

        //should create variables for funciton

        public void meleAttack()
        {
            random = new Random();
            double randomInterval = (random.NextDouble() * 10 + 1);

            if ((DateTime.Now - previousShoot).TotalSeconds >= randomInterval)
            {
                isMeleAttacking = true;

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
                if (isJumping)
                {
                    //if(map.isPassable(gridLocation.X,gridLocation.Y))
                    if (direction == Movement.Right)
                        torso.Body.ApplyForce(new Vector2(34, 0));
                    if (direction == Movement.Left)
                        torso.Body.ApplyForce(new Vector2(-34, 0));
                }

                if (map.isPassable(GridLocation.X, GridLocation.Y + 1))
                    isInAir = true;
                else
                {
                    isInAir = false;
                    if (isJumping)
                        isJumping = false;
                }

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
                if (lookingRight)
                    anim = animations[1];
                else
                    anim = animations[0];

                if (isMoving) // apply animation
                    Anim.Update(gameTime);
                else //player will appear as standing with frame [1] from the atlas.
                    Anim.CurrentFrame = 1;

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {

            if (PatrolGridPath != null)
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

                for (int i = 0; i < PatrolGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)PatrolGridPath[i].X * 64, (int)PatrolGridPath[i].Y * 64, 64, 64);
                    if (gridLocation.ToVector2() != PatrolGridPath[i])
                        spriteBatch.Draw(shootTexture, gridloc, c);
                    else
                        spriteBatch.Draw(shootTexture, gridloc, Color.Green);
                }

            }

            //dRAWS PATH TO PLAYER

            if (PlayerGridPath != null)
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
            Rectangle dest = torso.physicsGroundedEnemyRectnagleObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;
            if (!torso.Body.IsDisposed && anim != null && !dissapear)
                anim.Draw(spriteBatch, dest, torso.Body, false);

            //pv1.Draw(gameTime, spriteBatch);
            //pv2.Draw(gameTime, spriteBatch);
            //pv3.Draw(gameTime, spriteBatch);

            if (mele != null && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);

            //wheel.Draw(gameTime,spriteBatch);
            /*
            spriteBatch.DrawString(font, this.GridLocation.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 - 20), Color.White);
            if(selectedPath != null)
              spriteBatch.DrawString(font, selectedPath[selectedPath.Length - 1].ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);
            spriteBatch.DrawString(font, IsInAir.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64), Color.White);
            spriteBatch.DrawString(font, this.mode.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 40), Color.White);
        */
        }

        public void UpdateAI()
        {
            /*
            if (isJumping)
            {
                if (map.isPassable(gridLocation.X, gridLocation.Y + 1))
                {
                    if(direction == Movement.Right)
                    {
                        wheel.Body.ApplyForce(new Vector2(3,0));
                    }
                    else
                        wheel.Body.ApplyForce(new Vector2(-3, 0));
                }
            }
            */
            if (selectedPath == null)
                selectedPath = new Vector2[] { Vector2.Zero };

            //Borders for chcking path to the player,to reduce calculations
            if ((Player.GridLocation.X < GridLocation.X + 20 && Player.GridLocation.X > GridLocation.X - 20) && (Player.GridLocation.Y > 0) && Player.GridLocation != null && !isInAir)
            {
                playerGridPath = findPathToPlayer();

            }

            if (PlayerGridPath == null)
                PlayerGridPath = new Vector2[] { gridLocation.ToVector2() };

            if ((PlayerGridPath.Length < 14 && PlayerGridPath.Length > 1))//sight range
            {

                var trunk = MainDecisionTree();
                decision = trunk.Evaluate(this);

                if (decision == "Attack") //attack range                
                    mode = Mode.Attack;

                else if (decision == "Evade")
                    mode = Mode.Evade;

                else if (mode != Mode.WalkToPlayer)
                    mode = Mode.WalkToPlayer;

            }

            else
            {
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

                    if (PatrolGridPath != null)
                    {
                        if (PatrolGridPath.Length == 1)
                        {
                            itrator = 0;
                            wandered = true;
                        }
                        /*
                        if ((DateTime.Now - previousStuckCheck).TotalSeconds >= stuckCheckInterval && mode != Mode.Idle)
                        {
                            if (checkIfStuck())
                            {
                                itrator = 0;
                                wandered = true;
                                previousStuckCheck = DateTime.Now;
                            }
                        }
                        */
                    }

                    if ((itrator == 0 && wandered && !isInAir))
                    {
                        //Console.WriteLine("LOOKING FOR PATH");
                        PatrolGridPath = findPathToPatrol(patrolDirection * 20);
                        selectedPath = PatrolGridPath;
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
                    selectedPath = PlayerGridPath;

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

                    if (IsPlayerAlive)
                        meleAttack();
                    /*
                    random = new Random();
                    double randomInterval = (random.NextDouble() * 10 + 1);
                    if (randomInterval < 4 && (DateTime.Now - evaded).TotalSeconds > 8)
                    {
                        mode = Mode.Evade;
                        evaded = DateTime.Now;
                    }
                    */
                    break;

                case Mode.Evade:
                    speed *= 1.5f;
                    if (Player.Position.X > Position.X)
                        Move(Movement.Left);
                    else if (Player.Position.X < Position.X)
                        Move(Movement.Right);

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
                    if (selectedPath[itrator + 1].Y < gridLocation.Y)
                    {
                        Move(Movement.Stop);
                        Jump();

                    }

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

                    itrator++;
                }
                previousPosition = Position;
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
            path = PathFinder.FindPath(gridLocation.ToVector2(), new Vector2(gridLocation.X + maxDestanationValue, gridLocation.Y), "Manhattan");
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
            //Decision 3
            var evadeBranch = new DecisionQuery
            {
                Title = "Evade",
                Test = (en) => en.Health > 0,
                Positive = new DecisionResult { Result = true, Action = "Evade" },
                Negative = new DecisionResult { Result = false }
            };


            //Decision 2
            var attackBranch = new DecisionQuery
            {
                Title = "Attack",
                Test = (en) => en.PlayerGridPath.Length < 6,
                Positive = new DecisionResult { Result = true, Action = "Attack" },
                Negative = new DecisionResult { Result = false }
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
    }
}