using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using System;

namespace RemGame
{
    enum Movement
    {
        Left,
        Right,
        Jump,
        Stop
    }

    class Enemy
    {
        public enum Mode { Idle, Patrol, WalkToPlayer, Attack, Evade }// what mode of behavior the monster AI is using 

        protected static int x = 1;

        protected static ContentManager content;
        protected Point startLocationGrid;

        protected bool Ghost = false;//***

        protected World world;
        protected Map map;
        protected Kid player;

        protected String decision;

        protected int startinghealth;
        protected int health;
        protected Vector2 size;
        protected float mass;
        protected float speed;

        protected int inspectionSightRange;
        protected float idleInterval;

        private bool playerInAttackRange = false;

        protected Vector2 position;
        protected Vector2 previousPosition;
        protected Point gridLocation;


        private bool isPlayerAlive;
        protected bool isMoving;
        protected bool isAttacking;


        protected Vector2[] patrolGridPath;
        protected Vector2[] playerGridPath;

        protected List<Vector2> path;///protection type needed

        protected Vector2[] selectedPath;

        Texture2D shootTexture;

        protected Texture2D gridColor;




        public Enemy(World world, Map map, Kid player, int health, Vector2 size, float mass, float speed, Point startLocationGrid, SpriteFont f)
        {
            
            this.world = world;
            this.map = map;
            this.Player = player;

            this.startinghealth = health;
            this.Health = health;
            this.size = size;
            this.mass = mass / 2.0f;
            this.speed = speed;

            this.startLocationGrid = startLocationGrid;

            isMoving = false;
            IsAttacking = false;

        }

        public int Health { get => health; set => health = value; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public Point GridLocation { get => gridLocation; set => gridLocation = value; }
        public virtual Vector2 Position { get => position; }
        public bool IsPlayerAlive { get => Player.IsAlive; }
        public int Startinghealth { get => startinghealth;}
        protected string Decision { get => decision; set => decision = value; }
        public Vector2[] PatrolGridPath { get => patrolGridPath; set => patrolGridPath = value; }
        public Vector2[] PlayerGridPath { get => playerGridPath; set => playerGridPath = value; }
        public Random Luck { get; private set; }
        public Kid Player { get => player; set => player = value; }
        public bool PlayerInAttackRange { get => playerInAttackRange; set => playerInAttackRange = value; }

        public virtual void Update(GameTime gameTime, Vector2 playerPosition, bool PlayerAlive, int patrolbound)
        {
            if (gridLocation == startLocationGrid)
                PathFinder.SetMap(map);

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            /*
                //DRAWS A* PATH
                for (int i = 0; i < patrolGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)patrolGridPath[i].X * 64, (int)patrolGridPath[i].Y * 64, 64, 64);
                    if (gridLocation.ToVector2() != patrolGridPath[i])
                        spriteBatch.Draw(gridColor, gridloc, c);
                    else
                        spriteBatch.Draw(gridColor, gridloc, Color.Green);
                }
                
            }

            //dRAWS PATH TO PLAYER
            if (playerGridPath != null)
            {

                for (int i = 0; i < playerGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)playerGridPath[i].X * 64, (int)playerGridPath[i].Y * 64, 40, 40);
                    if (gridLocation.ToVector2() != playerGridPath[i])
                        spriteBatch.Draw(gridColor, gridloc, Color.Green);
                    else
                        spriteBatch.Draw(gridColor, gridloc, Color.GreenYellow);
                }
            }
            */
            //torso.Draw(gameTime,spriteBatch);
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;


            //wheel.Draw(gameTime,spriteBatch);
            /*
           spriteBatch.DrawString(font, this.GridLocation.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 - 20), Color.White);
            if (selectedPath != null)
                spriteBatch.DrawString(font, selectedPath[selectedPath.Length - 1].ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);
            
            */
            //spriteBatch.DrawString(font, itrator.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);

            //spriteBatch.DrawString(font, "IM HERE", new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 40), Color.White);

        }


        public virtual Vector2[] findPathToPlayer()
        {
            Vector2[] arr;

            path = PathFinder.FindPath(gridLocation.ToVector2(), Player.GridLocation.ToVector2(), "Manhattan");
            if (path == null)
                arr = new Vector2[] { gridLocation.ToVector2() };
            else
                arr = path.ToArray();

            return arr;
        }

        public virtual void setAstarsquare(Texture2D t)
        {
            gridColor = t;
        }

        protected bool checkIfStuck()
        {
            Console.WriteLine("pre" + previousPosition);
            Console.WriteLine(Position);

            if (Math.Abs(previousPosition.X - Position.X) < 0.1)
                return true;
            else
                return false;

        }

        public virtual bool isSpecialAbbilityLuck()
        {
            Luck = new Random();
            float chance = Luck.Next(1000);
            Console.WriteLine("luck:" + chance);
            if (chance == 1)
            {
                return true;
            }

            else return false;
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


