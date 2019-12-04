using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace RemGame
{

    enum Animation
    {
        Crouch,
        CrouchWalk,
        Idle,
        Walk,
        JumpStart,
        JumpUp,
        JumpMid,
        JumpDown,
        JumpEnd,
        SlideStart,
        SlideIn,
        SlideEnd,
        MeleeAttack,
        MeleeAttackLeft,
        RangedAttack,
        RangedAttackLeft
    }

    public enum playerMode { Idle, Walking, Attacking }// what mode of behavior the monster AI is using 


    class Kid : Component
    {
        private static ContentManager content;

        private bool GameOver = false;
        private HealthBar health_bar;
        private World world;
        private Map map;
        private Vector2 size;
        private float mass;
        private Point gridLocation;

        private Vector2 followingPlayerPoint;
        private bool isFalling = false;
        private bool firstMove = false;
        private SpriteFont f;


        PhysicsView pv1;
        PhysicsView pv2;
        PhysicsView pv3;

        private PhysicsObject upBody;
        private PhysicsObject midBody;
        private PhysicsObject wheel;

        private Rectangle collisionRec;

        private RevoluteJoint axis1;
        private Joint axis2;

        /// <tmp>
        private PhysicsObject shot;
        private List<PhysicsObject> shotList = new List<PhysicsObject>();
        bool reachedDest;

        private PhysicsObject rangedShot;
        private List<PhysicsObject> rangedShotList = new List<PhysicsObject>();


        Vector2 shootBase;
        Vector2 shootDirection;

        private bool isMeleAttacking = false;
        private bool isRangeAttacking = false;

        Texture2D yoyoTexture;
        Texture2D scissorsTexture;
        Texture2D frozenState;


        private int health = 30;
        private bool isAlive = true;
        private playerMode mode = playerMode.Idle;

        private const float SPEED = 2.0f;
        private float speed = SPEED;
        private float actualMovningSpeed = 0;

        private bool isMoving = false;
        private bool isJumping = false;
        private bool isSliding = false;
        private bool isBending = false;

        private Movement direction = Movement.Right;
        private bool lookRight = true;

        private Vector2 cameraToFollow;


        private bool showText = false;

        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.4f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping
        private float takeOffPoi = 0;
        private float liftOff = 0;
        private float beforeLanding = 0;
        private bool goingDown = false;
        private bool hasLanded = true;
        private bool playLandingSound = false;



        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 1.3f;        // in seconds
        private Vector2 slideForce = new Vector2(7, 0); // applied force when jumping
        private float startPoint = 0;
        private float slideTracker = 0;
        private bool slideOver = true;

        private DateTime previousShoot = DateTime.Now;
        private const float shootInterval = 0.3f;        // in seconds

        private DateTime previousRangedShoot = DateTime.Now;

        private DateTime previousBend = DateTime.Now;
        private const float bendInterval = 0.1f;        // in seconds

        private DateTime previousFreeze = DateTime.Now;   // time at which we previously jumped
        private const float freezeInterval = 2.0f;

        /// <summary>
        /// /////////////////////////////Art Assignment
        /// </summary>
        private AnimatedSprite anim = null;
        private AnimatedSprite[] animations = new AnimatedSprite[16];


        private Camera2D cam;
        Texture2D playerCrouch;
        Texture2D playerCrouchWalk;
        Texture2D playerStand;
        Texture2D playerWalk;
        Texture2D playerMeleAttack;
        Texture2D playerRangedAttack;

        Texture2D[] jumpSetAnim = new Texture2D[5];
        Texture2D[] slideSetAnim = new Texture2D[3];

        SoundEffect footstep;
        SoundEffect jump_Up;
        SoundEffect jump_Down;
        SoundEffect idle;
        SoundEffect crouch;
        SoundEffect slide;

        bool isJumpSoundPlayed = false;

        SoundEffectInstance walkingInstance;
        SoundEffectInstance jumpingUpInstance;
        SoundEffectInstance jumpingDownInstance;
        SoundEffectInstance idleInstance;
        SoundEffectInstance crouchingInstance;
        SoundEffectInstance slidingInstance;


        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        MouseState currentMouseState;
        MouseState previousMouseState = Mouse.GetState();
        private bool isFreezing;

        public AnimatedSprite Anim { get => anim; set => anim = value; }

        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        public Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => upBody.Position; }
        public bool IsJumping { get => isJumping; set => isJumping = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public bool IsBending { get => isBending; set => isBending = value; }
        public int Health { get => health; set => health = value; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public float ActualMovningSpeed { get => actualMovningSpeed; set => actualMovningSpeed = value; }
        public bool HasLanded { get => hasLanded; set => hasLanded = value; }
        public bool IsSliding { get => isSliding; set => isSliding = value; }
        public bool PlayLandingSound { get => playLandingSound; set => playLandingSound = value; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public bool FirstMove { get => firstMove; set => firstMove = value; }
        public Vector2 CameraToFollow { get => cameraToFollow; set => cameraToFollow = value; }
        public HealthBar HealthBar { get => health_bar; set => health_bar = value; }
        public Point GridLocation { get => gridLocation; set => gridLocation = value; }
        public playerMode Mode { get => mode; set => mode = value; }
        public Rectangle CollisionRec { get => collisionRec; set => collisionRec = value; }

        public Kid(Camera2D cam, World world, Vector2 size, float mass, Vector2 startPosition, bool isBent, SpriteFont f)
        {
            this.cam = cam;
            health_bar = new HealthBar(content);
            this.world = world;
            this.map = map;
            this.size = size;
            this.mass = mass / 4.0f;
            this.f = f;

            IsMoving = false;
            isFreezing = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X;


            // Create the upper body
            upBody = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            upBody.Position = startPosition;
            followingPlayerPoint = Position;

            // Create the midlle body
            midBody = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            midBody.Position = upBody.Position + new Vector2(0, size.Y);

            // Create the feet("Engine") of the body
            wheel = new PhysicsObject(world, null, wheelSize, mass / 2.0f);
            wheel.Position = midBody.Position + new Vector2(0, size.Y);

            cameraToFollow = new Vector2(Position.X + 100, Position.Y - 50);

            upBody.Body.Friction = 1.0f;
            midBody.Body.Friction = 1.0f;
            wheel.Body.Friction = 2.0f;


            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, upBody.Body, new Body(world));
            JointFactory.CreateAngleJoint(world, midBody.Body, new Body(world));


            upBody.Body.CollisionCategories = Category.Cat10;
            midBody.Body.CollisionCategories = Category.Cat10;
            wheel.Body.CollisionCategories = Category.Cat11;

            upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
            midBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat30;

            upBody.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);
            midBody.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);

            //direction = Movement.Right;

            axis1 = JointFactory.CreateRevoluteJoint(world, midBody.Body, wheel.Body, Vector2.Zero);
            axis1.CollideConnected = true;
            axis1.MotorEnabled = true;
            axis1.MotorSpeed = 20.0f;
            axis1.MaxMotorTorque = 3.5f;



            axis2 = JointFactory.CreateRevoluteJoint(world, upBody.Body, midBody.Body, Vector2.Zero);

            axis2.CollideConnected = true;

            ////Art Init
            yoyoTexture = Content.Load<Texture2D>("Player/Anim/Yoyo");
            scissorsTexture = Content.Load<Texture2D>("Player/Anim/Scissors_Open");
            frozenState = Content.Load<Texture2D>("Player/Anim/Iceberg");


            playerCrouch = Content.Load<Texture2D>("Player/Anim/Ron_Crouch");
            playerCrouchWalk = Content.Load<Texture2D>("Player/Anim/Ron_Crouch_Walk");
            playerStand = Content.Load<Texture2D>("Player/Anim/Ron_Stand");
            playerWalk = Content.Load<Texture2D>("Player/Anim/Ron_Walk");

            playerMeleAttack = Content.Load<Texture2D>("Player/Anim/Ron_Melee_Yoyo");
            playerRangedAttack = Content.Load<Texture2D>("Player/Anim/Ron_Ranged_Scissors");


            footstep = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Footsteps");
            walkingInstance = footstep.CreateInstance();
            walkingInstance.IsLooped = true;
            walkingInstance.Pitch = 0.2f;


            jump_Up = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Jump_Up");
            jumpingUpInstance = jump_Up.CreateInstance();
            jumpingUpInstance.IsLooped = false;
            jumpingUpInstance.Volume = 0.04f;

            jump_Down = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Jump_Down");
            jumpingDownInstance = jump_Down.CreateInstance();
            jumpingDownInstance.IsLooped = false;
            jumpingDownInstance.Volume = 0.04f;

            idle = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Idle");
            idleInstance = idle.CreateInstance();
            idleInstance.IsLooped = true;
            idleInstance.Volume = 0.35f;
            idleInstance.Pitch = 0.3f;


            crouch = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Crouch");
            crouchingInstance = crouch.CreateInstance();
            crouchingInstance.IsLooped = true;
            crouchingInstance.Volume = 0.05f;
            crouchingInstance.Pitch = 0.25f;


            slide = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Slide");
            slidingInstance = slide.CreateInstance();
            slidingInstance.IsLooped = false;
            slidingInstance.Volume = 0.06f;

            Rectangle anim1 = new Rectangle(-135, -65, 200, 160);
            Rectangle anim2 = new Rectangle(-65, -65, 200, 160);
            Rectangle anim3 = new Rectangle(-100, -65, 200, 160);
            Rectangle anim4 = new Rectangle(0, -50, 140, 130);
            Rectangle anim5 = new Rectangle(-50, -65, 220, 160);
            Rectangle anim6 = new Rectangle(-170, -65, 220, 160);


            Animations[(int)Animation.Crouch] = new AnimatedSprite(playerCrouch, 1, 1, anim3, 0f);
            Animations[(int)Animation.CrouchWalk] = new AnimatedSprite(playerCrouchWalk, 2, 16, anim3, 0.03f);

            Animations[(int)Animation.Idle] = new AnimatedSprite(playerStand, 4, 13, anim3, 0.017f);

            Animations[(int)Animation.Walk] = new AnimatedSprite(playerWalk, 2, 12, anim3, 0.03f);

            jumpSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_01_start");
            jumpSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_02_up");
            jumpSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_03_mid");
            jumpSetAnim[3] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_04_down");
            jumpSetAnim[4] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_05_end");

            Animations[(int)Animation.JumpStart] = new AnimatedSprite(jumpSetAnim[0], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpUp] = new AnimatedSprite(jumpSetAnim[1], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpMid] = new AnimatedSprite(jumpSetAnim[2], 1, 3, anim3, 0.6f);
            Animations[(int)Animation.JumpDown] = new AnimatedSprite(jumpSetAnim[3], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpEnd] = new AnimatedSprite(jumpSetAnim[4], 1, 1, anim3, 0f);

            slideSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_01_start");
            slideSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_02_slide");
            slideSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_03_end");

            Animations[(int)Animation.SlideStart] = new AnimatedSprite(slideSetAnim[0], 1, 4, anim3, 0.4f);
            Animations[(int)Animation.SlideIn] = new AnimatedSprite(slideSetAnim[1], 1, 6, anim3, 0.3f);
            Animations[(int)Animation.SlideEnd] = new AnimatedSprite(slideSetAnim[2], 1, 4, anim3, 0.4f);

            Animations[(int)Animation.MeleeAttack] = new AnimatedSprite(playerMeleAttack, 3, 10, anim2, 0.02f);
            Animations[(int)Animation.MeleeAttackLeft] = new AnimatedSprite(playerMeleAttack, 3, 10, anim1, 0.02f);

            Animations[(int)Animation.RangedAttack] = new AnimatedSprite(playerRangedAttack, 2, 12, anim5, 0.02f);
            Animations[(int)Animation.RangedAttackLeft] = new AnimatedSprite(playerRangedAttack, 2, 12, anim6, 0.02f);



            pv1 = new PhysicsView(upBody.Body, upBody.Position, upBody.Size, f);
            pv2 = new PhysicsView(midBody.Body, midBody.Position, wheel.Size, f);
            pv3 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);
        }

        ///////////////////////////////////////////////////////////Abillities////////////////////////////////////////////////////////
        public void Move(Movement movement)
        {
            axis1.MotorEnabled = true;

            if (!IsBending && !isJumping)
                speed = SPEED;

            if (!IsSliding && !IsJumping)
            {
                switch (movement)
                {
                    case Movement.Left:
                        mode = playerMode.Walking;

                        lookRight = false;
                        axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                        anim = animations[(int)Animation.Walk];
                        break;

                    case Movement.Right:
                        mode = playerMode.Walking;

                        lookRight = true;
                        axis1.MotorSpeed = MathHelper.TwoPi * speed;
                        anim = animations[(int)Animation.Walk];
                        break;

                    case Movement.Stop:
                        mode = playerMode.Idle;

                        axis1.MotorSpeed = 0;
                        //axis1.MotorEnabled = false;                       
                        if (!isFalling)
                        {
                            ResetPlayerDynamics();
                        }

                        if (firstMove)
                            anim = animations[(int)Animation.JumpEnd];
                        else
                            anim = animations[(int)Animation.Idle];
                        break;
                }

            }
        }


        public void Jump()

        {

            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval && hasLanded)
            {
                anim = animations[(int)Animation.JumpStart];
                isJumping = true;
                takeOffPoi = wheel.Position.Y;
                liftOff = wheel.Position.Y;

                wheel.Body.ApplyLinearImpulse(jumpForce * new Vector2(0, 1));

                goingDown = false;
                HasLanded = false;
                previousJump = DateTime.Now;

            }

        }

        public void Slide(Movement dir)
        {
            speed = SPEED;
            if (!isJumping && !isBending)
            {
                if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
                {
                    ResetPlayerDynamics();

                    anim = animations[(int)Animation.SlideStart];
                    slideOver = false;
                    startPoint = wheel.Position.X;
                    slideTracker = startPoint;
                    IsSliding = true;
                    upBody.Body.CollidesWith = Category.None;
                    midBody.Body.CollidesWith = Category.None;
                    if (dir == Movement.Right)
                        wheel.Body.ApplyLinearImpulse(slideForce);
                    else
                    {
                        slideForce.X = slideForce.X * -1;
                        wheel.Body.ApplyLinearImpulse(slideForce);
                        slideForce.X = slideForce.X * -1;
                    }
                    wheel.Body.Friction = 20.0f;
                    previousSlide = DateTime.Now;
                }
            }
        }

        public void bend()
        {
            if (!isJumping && !IsSliding)
            {
                if (IsMoving)
                {
                    upBody.Body.CollidesWith = Category.None;
                    speed = SPEED / 2;
                    anim = animations[(int)Animation.CrouchWalk];
                }
                else
                {
                    upBody.Body.CollidesWith = Category.None;
                    anim = animations[(int)Animation.Crouch];
                }
                //////shot single image crouch
            }

        }

        //////////////////////////////////////// Actions////////////////////////////////////////////////
        public void Shoot()
        {
            if ((DateTime.Now - previousShoot).TotalSeconds >= shootInterval)
            {
                mode = playerMode.Attacking;
                isMeleAttacking = true;
                reachedDest = false;//check for keft direction
                shot = new PhysicsObject(world, yoyoTexture, 15, 1);
                shot.Body.CollisionCategories = Category.Cat28;
                shot.Body.CollidesWith = Category.Cat20 | Category.Cat21 | Category.Cat1;
                shot.Body.Mass = 0.8f;
                shot.Body.IgnoreGravity = true;
                int shootingDirection;

                if (lookRight)
                {
                    shot.Position = new Vector2(upBody.Position.X + upBody.Size.X * 2, upBody.Position.Y + upBody.Size.Y / 4);
                    shootingDirection = 1;
                }
                else
                {
                    shot.Position = new Vector2(upBody.Position.X, upBody.Position.Y + upBody.Size.Y / 4);
                    shootingDirection = -1;

                }

                shot.Body.ApplyLinearImpulse(new Vector2(6 * shootingDirection, 0));
                shot.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);

                previousShoot = DateTime.Now;

            }


        }

        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            PhysicsObject tmp = null;

            if (shot.Body.BodyId == fixtureA.Body.BodyId)
                tmp = shot;


            if (tmp != null)
            {

                tmp.Body.Dispose();
            }

            return true;
        }

        public void rangedShoot(Vector2 shootForce)
        {
            if ((DateTime.Now - previousRangedShoot).TotalSeconds >= shootInterval)
            {
                mode = playerMode.Attacking;
                isRangeAttacking = true;
                rangedShot = new PhysicsObject(world, scissorsTexture, 15, 1);
                rangedShot.Body.CollisionCategories = Category.Cat28;
                rangedShot.Body.CollidesWith = Category.Cat20 | Category.Cat21 | Category.Cat1;
                rangedShot.Body.IgnoreCollisionWith(upBody.Body);
                rangedShot.Body.IgnoreCollisionWith(wheel.Body);

                //Console.WriteLine("end: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);
                rangedShot.Position = new Vector2(upBody.Position.X, upBody.Position.Y - size.Y);
                rangedShot.Body.Mass = 0.8f;
                rangedShot.Body.ApplyForce(shootForce);
                rangedShot.Body.OnCollision += new OnCollisionEventHandler(Shoot_OnCollision);
                rangedShotList.Add(rangedShot);
                previousRangedShoot = DateTime.Now;
            }

        }

        bool Shoot_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            PhysicsObject tmp = null;
            foreach (PhysicsObject p in rangedShotList)
            {
                if (p.Body.BodyId == fixtureA.Body.BodyId)
                    tmp = p;
            }

            if (tmp != null)
            {
                tmp.Body.Dispose();
                rangedShotList.Remove(tmp);
            }

            return true;
        }

        //---IN MAINTAINCE---
        public void Kinesis(Obstacle obj, MouseState currentMouseState)
        {

            if (obj.KinesisOn == true)
                if (obj.Position.Y > 0)
                {
                    if (currentMouseState.RightButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        obj.Body.CollidesWith = Category.None;
                        obj.Body.BodyType = BodyType.Dynamic;
                        //obj.Body.Mass = 0.0f;
                        obj.Body.GravityScale = 0.1f;
                        obj.Body.Rotation = 0.1f;
                        obj.Body.ApplyForce(new Vector2(0, -6.0f));

                    }
                    else
                    {

                        //obj.Body.Mass = 0;
                        //obj.Body.ResetDynamics();
                        //obj.Body.BodyType = BodyType.Static;
                    }
                }
                else
                {
                    obj.Body.ResetDynamics();
                    obj.Body.GravityScale = 1;
                    //obj.Body.Mass = 0.0f;
                    //obj.Body.Mass = 9.8f;
                    if (currentMouseState.RightButton == ButtonState.Pressed)
                    {
                        //obj.Body.BodyType = BodyType.Static;
                        //obj.Body.ApplyForce(new Vector2(4.0f, 0));
                        //obj.Body.BodyType = BodyType.Static;
                    }
                }
            //obj.KinesisOn = false;
        }
        /////

        bool HitByEnemy(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            if (fixtureB.CollisionCategories == Category.Cat30)
            {
                if (HealthBar.getRectangle.Width > 0)
                {
                    HealthBar.decrease(40);
                }
                if (HealthBar.getRectangle.Width <= 0)
                {
                    IsAlive = false;

                }
            }

            return true;
        }

        public override void Update(GameTime gameTime)
        {

            walkingInstance.Volume = 0.1f;
            if ((DateTime.Now - previousFreeze).TotalSeconds > freezeInterval)
            {
                isFreezing = false;
            }

            if (isAlive)
            {

                keyboardState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
                actualMovningSpeed = upBody.Body.AngularVelocity;
                //set the coordinates for camera to follow              
                Vector2 moveTo = Position - cameraToFollow;
                Vector2 fixedPosition;

                collisionRec = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.X * 3);

                if (!IsBending)
                    fixedPosition = new Vector2(moveTo.X + 150, moveTo.Y);
                else
                    fixedPosition = new Vector2(moveTo.X + 150, moveTo.Y + 80);

                cameraToFollow += fixedPosition * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;


                ///////////Check for falling

                if ((followingPlayerPoint.Y + 1.0 < upBody.Position.Y && !isSliding || !firstMove))
                {
                    isFalling = true;

                }
                else
                    isFalling = false;

                if (isFalling == true && upBody.Position.Y > 1300)
                    isAlive = false;

                followingPlayerPoint = upBody.Position;//indicates falling

                //////////////////////Jump Animation ///////////////////
                if (!HasLanded && isJumping)
                {

                    if (wheel.Position.Y <= liftOff)
                    {
                        liftOff = wheel.Position.Y;
                        anim = animations[(int)Animation.JumpStart];
                        if (wheel.Position.Y <= takeOffPoi - 60)
                        {
                            anim = animations[(int)Animation.JumpUp];
                            isFalling = true;

                        }

                    }

                    else if (wheel.Position.Y > liftOff && !goingDown)
                    {
                        goingDown = true;
                        beforeLanding = wheel.Position.Y;
                        anim = animations[(int)Animation.JumpMid];

                    }
                    else if (wheel.Position.Y > beforeLanding)
                    {
                        beforeLanding = wheel.Position.Y;
                        if (wheel.Position.Y > takeOffPoi - 140)
                            anim = animations[(int)Animation.JumpDown];
                        if (wheel.Position.Y > takeOffPoi - 50)
                            anim = animations[(int)Animation.JumpEnd];
                    }
                    else
                    {
                        PlayLandingSound = true;
                        anim = animations[(int)Animation.Idle];
                        goingDown = false;
                        HasLanded = true;
                        isJumping = false;
                        liftOff = 0;
                        beforeLanding = 0;
                    }

                }

                //////////////////////Slide Animation//////////////////////////////
                if (IsSliding)
                {
                    if (wheel.Position.X == slideTracker || wheel.Position.X < slideTracker && direction == Movement.Right || wheel.Position.X > slideTracker && direction == Movement.Left)
                    {
                        slideOver = true;
                        ResetPlayerDynamics();
                    }
                    if (wheel.Position.X > startPoint + 100 || wheel.Position.X < startPoint - 100)
                    {
                        anim = animations[(int)Animation.SlideIn];
                    }

                    if (wheel.Position.X > startPoint + 350 || wheel.Position.X < startPoint - 350)
                    {
                        anim = animations[(int)Animation.SlideEnd];
                    }
                    if (wheel.Position.X > startPoint + 400 || wheel.Position.X < startPoint - 400 || slideOver)
                    {
                        IsSliding = false;
                        slideOver = true;
                        upBody.Body.ResetDynamics();

                        upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                        midBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                    }
                    slideTracker = wheel.Position.X;
                }
                else
                    wheel.Body.Friction = 2.0f;

                IsMoving = false;


                ////////////////////////Key Mangment///////////////////////////////////////////
                /////Movments
                ///
                ///Move Right
                if (!isFreezing)
                {
                    if (!isMeleAttacking)
                    {
                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            if (direction == Movement.Right && !isJumping)
                                ResetPlayerDynamics();

                            Move(Movement.Left);
                            direction = Movement.Left;
                            IsMoving = true;

                        }

                        ///Move Left
                        else if (keyboardState.IsKeyDown(Keys.D))
                        {
                            if (direction == Movement.Left && !isJumping)
                                ResetPlayerDynamics();

                            Move(Movement.Right);
                            direction = Movement.Right;
                            IsMoving = true;
                            if (!firstMove)
                                firstMove = true;

                        }
                        ///No Moving
                        else
                        {
                            IsMoving = false;
                            Move(Movement.Stop);
                        }
                    }
                    if (isFalling)
                        Move(Movement.Stop);

                    ///Jump
                    if (keyboardState.IsKeyDown(Keys.Space) && (!prevKeyboardState.IsKeyDown(Keys.Space)))
                    {
                        if (!IsSliding)
                            Jump();
                    }
                    ///Move While jump
                    ///

                    if (isJumping)
                    {

                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            if (wheel.Body.LinearVelocity.X < 4.0f)
                                wheel.Body.ApplyLinearImpulse(new Vector2(0.07f, 0));
                            if (wheel.Body.LinearVelocity.X < 1.0f)
                            {
                                axis1.MotorEnabled = false;
                                wheel.Body.ApplyForce(new Vector2(0, 1.0f));
                            }
                        }

                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            if (wheel.Body.LinearVelocity.X > -4.0f)
                                wheel.Body.ApplyLinearImpulse(new Vector2(-0.07f, 0));
                            if (wheel.Body.LinearVelocity.X > -1.0f)
                            {
                                axis1.MotorEnabled = false;
                                wheel.Body.ApplyForce(new Vector2(0, 1.0f));
                            }
                        }
                    }




                    ///Slide
                    if (keyboardState.IsKeyDown(Keys.LeftShift) && (!prevKeyboardState.IsKeyDown(Keys.LeftShift)))
                    {

                        if (keyboardState.IsKeyDown(Keys.D))
                            Slide(Movement.Right);

                        else if (keyboardState.IsKeyDown(Keys.A))
                            Slide(Movement.Left);

                    }
                    ///Bend
                    if (keyboardState.IsKeyDown(Keys.S))
                    {
                        bend();
                        IsBending = true;

                    }
                    ///Check for leaving bend pose
                    if (keyboardState.IsKeyUp(Keys.S) && prevKeyboardState.IsKeyDown(Keys.S))
                    {
                        IsBending = false;
                        upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                    }

                    /////////Actions///////////////////////
                    ///Ranged Shot
                    ///Calculate Direction For Shooting
                    if (currentMouseState.RightButton == ButtonState.Pressed && !(previousMouseState.RightButton == ButtonState.Pressed))
                    {
                        shootDirection = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);

                    }
                    ///Calculate Motion Vector For Shooting
                    if (currentMouseState.RightButton == ButtonState.Released && (previousMouseState.RightButton == ButtonState.Pressed) && !isSliding)
                    {
                        shootBase = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);
                        Vector2 shootForce = new Vector2((shootDirection.X - shootBase.X), (shootDirection.Y - shootBase.Y));
                        if (shootForce.X > 5 || shootForce.X < -5 || shootForce.Y > 5 || shootForce.Y < -5)
                            rangedShoot(shootForce);

                    }

                    ///Sraight Shot / might change to Mele
                    if (currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed) && !(currentMouseState.RightButton == ButtonState.Pressed) && !isSliding)

                    {
                        Shoot();

                    }
                }
                ///////////////////////////////////////////////////////////////Sound Effects///////////////////////////////////////////////////////////////////
                if (direction != Movement.Stop && isMoving)
                {
                    walkingInstance.Play();
                }

                else
                {
                    walkingInstance.Stop();
                }

                if (IsJumping && !isJumpSoundPlayed)
                {
                    isJumpSoundPlayed = true;
                    jumpingUpInstance.Play();
                }

                if (!IsJumping)
                    isJumpSoundPlayed = false;

                if (HasLanded && PlayLandingSound)
                {
                    jumpingDownInstance.Play();
                    PlayLandingSound = false;
                }

                if (!IsMoving && !IsJumping)
                    idleInstance.Play();
                else
                    idleInstance.Stop();

                if (IsBending)
                    crouchingInstance.Play();
                else
                    crouchingInstance.Stop();


                if (IsSliding)
                    slidingInstance.Play();


                if (shot != null && !shot.Body.IsDisposed)
                {
                    shot.Body.Rotation += 0.5f;

                    if (shot.Position.X > Position.X + size.X + 300)
                    {
                        shot.Body.ApplyLinearImpulse(new Vector2(15 * -1, 0));
                    }

                    if (shot.Position.X < Position.X - 300)
                    {
                        shot.Body.ApplyLinearImpulse(new Vector2(15 * 1, 0));
                        reachedDest = true;
                    }

                    if ((shot.Position.X < Position.X + 30 && lookRight && !reachedDest) || (shot.Position.X > Position.X && reachedDest && !lookRight))
                    {
                        shot.Body.OnCollision -= new OnCollisionEventHandler(Mele_OnCollision);
                        shot.Body.Dispose();
                    }

                    else
                    {
                        shot.Update(gameTime);
                    }
                }

                foreach (PhysicsObject r in rangedShotList)
                {
                    r.Body.Rotation += 0.2f;
                    r.Update(gameTime);
                }


                if (isMeleAttacking)
                {
                    if (lookRight)
                        anim = animations[(int)Animation.MeleeAttack];
                    else
                        anim = animations[(int)Animation.MeleeAttackLeft];

                    if (anim.IsLooped)
                    {
                        isMeleAttacking = false;
                        anim.IsLooped = false;

                    }
                }

                if (isRangeAttacking)
                {
                    if (lookRight)
                        anim = animations[(int)Animation.RangedAttack];
                    else
                        anim = animations[(int)Animation.RangedAttackLeft];


                    if (anim.IsLooped)
                    {

                        isRangeAttacking = false;
                        anim.IsLooped = false;
                    }

                }

                if (!isMoving && !IsBending && !isJumping && !IsSliding && !isFalling && !isMeleAttacking && !isRangeAttacking)
                    anim = animations[(int)Animation.Idle];

                Anim.Update(gameTime);

                previousMouseState = currentMouseState;
                prevKeyboardState = keyboardState;

            }
            else
                GameOver = true;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!GameOver)
            {
                //upBody.Draw(gameTime, spriteBatch);              
                Rectangle dest = upBody.physicsCircleObjRecToDraw();
                dest.Height = dest.Height + (int)wheel.Size.Y / 2;
                dest.Y = dest.Y + (int)wheel.Size.Y / 2;
                if (Anim != null)
                {
                    if (direction == Movement.Right)
                        Anim.Draw(spriteBatch, dest, upBody.Body, false);
                    else
                        Anim.Draw(spriteBatch, dest, upBody.Body, true);

                }

                if (isFreezing)
                {
                    dest.X -= 70;
                    dest.Y -= 70;
                    dest.Height *= 3;
                    dest.Width *= 3;
                    spriteBatch.Draw(frozenState, dest, Color.White * 0.7f);
                }

                if (shot != null && !shot.Body.IsDisposed)
                {
                    int ropeLength = Math.Abs((int)(shot.Position.X - Position.X - size.X));

                    if (lookRight)
                    {
                        for (int i = 0; i < ropeLength; i++)
                        {
                            spriteBatch.DrawString(f, "*", new Vector2(shot.Position.X - i, shot.Position.Y + shot.Size.X * 4 + 15), Color.White);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < ropeLength / 2; i++)
                        {
                            spriteBatch.DrawString(f, "*", new Vector2(shot.Position.X + i, shot.Position.Y + shot.Size.X * 4 + 15), Color.White);

                        }
                    }

                    shot.Draw(gameTime, spriteBatch);

                }

                foreach (PhysicsObject r in rangedShotList)
                {
                    if (!r.Body.IsDisposed)
                        r.Draw(gameTime, spriteBatch);
                }

                //spriteBatch.DrawString(f, gridLocation.ToString(), new Vector2(Position.X + size.X, Position.Y+30), Color.White);

                //pv1.Draw(gameTime, spriteBatch);
                //pv2.Draw(gameTime, spriteBatch);
                //pv3.Draw(gameTime, spriteBatch);

            }
            else
            {

                //needs to add a while loop for waiting 5 seconds before exiting to StartMenu
                // spriteBatch.DrawString(f, "GAME OVER!!!!!!", new Vector2(Position.X + size.X, Position.Y), Color.White);

            }
            //debbuging
            //spriteBatch.DrawString(f, lookRight.ToString(), new Vector2(Position.X + size.X, Position.Y + 20), Color.White);
            //if (map != null)
            //  spriteBatch.DrawString(f, "tile : " + map.getGridObject(gridLocation.X, gridLocation.Y + 3), new Vector2(Position.X + size.X, Position.Y + 40), Color.White);


        }

        public void ResetPlayerDynamics()
        {
            wheel.Body.ResetDynamics();
            midBody.Body.ResetDynamics();
            upBody.Body.ResetDynamics();
        }

        public void setMap(Map map)
        {
            this.map = map;
        }

        public void checkForFalling()
        {
            if (!isJumping && !isSliding && map.isPassable(gridLocation.X, gridLocation.Y + 1))
            {
                isFalling = true;
            }
            else
                isFalling = false;

        }

        public void freeze()
        {
            if (!isFreezing)
            {
                Move(Movement.Stop);
                isFreezing = true;
                previousFreeze = DateTime.Now;
            }
        }


    }
}