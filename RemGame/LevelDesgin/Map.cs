using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    /// <summary>
    /// an map object holds information on the current level as each level holds map object.
    /// the map class aimed to read the matrix represnts objects location in a level and genrate those objects into the level.
    /// moreover the map calss is used as an input to the A* algorithem to calculate npc's behavoir.
    /// </summary>
    class Map
    {

        private int[,] grid;//the matrix which represnts objects location in the level.
        //private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        private List<Tile> collisionTiles = new List<Tile>();
        private List<Obstacle> obstacleTiles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<HealthBooster> HealthBoostersList = new List<HealthBooster>();

        private Kid player;
        private int enemies_counter = 2;
        private int width, height;
        private World world;
        private Texture2D texture;

        //holdes eace value as {object value in matrix,is it passable as bool}
        public Dictionary<int, bool> passableDict = new Dictionary<int, bool>();

        private static ContentManager content;

        Random r;

        private bool finished_tutorial = false;

        /// delete font
        private SpriteFont font;
        /// 

        public Map(World world,Kid player)
        {
            this.world = world;
            this.player = player;
        }

        public List<Tile> CollisionTiles { get => collisionTiles; }
        public int Width { get => width; }
        public int Height { get => height; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public List<Obstacle> ObstacleTiles { get => obstacleTiles; set => obstacleTiles = value; }
        public List<Enemy> Enemies { get => enemies; set => enemies = value; }
        public bool Finished_tutorial { get => finished_tutorial; set => finished_tutorial = value; }

        //public List<CollisionTiles> CollisionTiles { get => collisionTiles; }
        public int Enemies_counter { get => enemies_counter; set => enemies_counter = value; }
        public int[,] Grid { get => grid;}

        //Construct each object as reprasented in the matrix in the location which calculated by : position in physics world = grid[x][y]*64.
        public void Generate(int[,] map, int size, SpriteFont font)
        {
            /////delete font//////
            this.font = font;

            int number = 0;
            passableDict.Add(0, true);
            passableDict.Add(8, true);

            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    number = map[y, x];
                    texture = Content.Load<Texture2D>("Tiles/HUD");

                    //if (number > 0 && number <= 2)
                    //{
                    //obs.Position = new Vector2(x * size, y * size);
                    /*
                    if (number == 2)
                        obs.Body.CollisionCategories = Category.Cat30;
                        */
                    //ObstacleTiles.Add(obs);

                    //collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size),world,new Vector2(64,64),font));
                    //collisionTiles.Add(new Tile(number, new Rectangle(x * size, y * size, size, size), world, new Vector2(64, 64)));
                    //    }


                    if (number == 1)//ground
                    {
                        Ground ground = new Ground(world, texture, new Vector2(64, 64), font,false);
                        ground.Position = new Vector2(x * size+(ground.Size.X / 2), y * size);
                        ObstacleTiles.Add(ground);
                        if (!passableDict.ContainsKey(1)) 
                        passableDict.Add(1, ground.Passable);
                    }
                    else if (number == 2)//locker
                    {
                        Locker locker = new Locker(world, texture, new Vector2(64, 64), font,false);
                        locker.Position = new Vector2(x * size+(locker.Size.X/2), y * size);
                        ObstacleTiles.Add(locker);
                        if (!passableDict.ContainsKey(2))         
                            passableDict.Add(2, locker.Passable);


                    }
                    else if (number == 3)//door
                    {
                        Door door = new Door(world, texture, new Vector2(64, 64), font,false);
                        door.Position = new Vector2(x * size + (door.Size.X / 2), y * size);
                        ObstacleTiles.Add(door);
                        if (!passableDict.ContainsKey(3))
                            passableDict.Add(3, door.Passable);

                    }
                    else if (number == 4)//bag
                    {
                        Bag bag = new Bag(world, texture, new Vector2(64, 64), font,false);
                        bag.Position = new Vector2(x * size + (bag.Size.X / 2), y * size);
                        ObstacleTiles.Add(bag);
                        if (!passableDict.ContainsKey(4))
                            passableDict.Add(4, bag.Passable);

                    }
                    else if (number == 5)//table
                    {
                        Table table = new Table(world, texture, new Vector2(64, 64), font,false);
                        table.Position = new Vector2(x * size + (table.Size.X / 2), y * size);
                        ObstacleTiles.Add(table);
                        if (!passableDict.ContainsKey(5))
                            passableDict.Add(5, table.Passable);


                    }
                    else if (number == 6)//chair
                    {
                        Chair chair = new Chair(world, texture, new Vector2(64, 64), font,false);
                        chair.Position = new Vector2(x * size + (chair.Size.X / 2), y * size);
                        ObstacleTiles.Add(chair);
                        if (!passableDict.ContainsKey(6))
                            passableDict.Add(6, chair.Passable);


                    }
                    else if (number == 7)//platfomr which is not floor
                    {
                        Obstacle obs = new Obstacle(world, texture, new Vector2(64, 64), font,false);
                        obs.Position = new Vector2(x * size + (obs.Size.X / 2), y * size);
                        ObstacleTiles.Add(obs);
                        obs.KinesisOn = false;
                        obs.Body.CollisionCategories = Category.Cat7;//////needs to be deleted 
                        if (!passableDict.ContainsKey(7))
                            passableDict.Add(7, obs.Passable);


                    }

                    else if (number == 8)
                    {
                        Obstacle obs2 = new Obstacle(world, texture, new Vector2(64, 64), font, false);
                        obs2.Position = new Vector2(x * size + (obs2.Size.X / 2), y * size);
                        ObstacleTiles.Add(obs2);
                        obs2.KinesisOn = true;
                        if (!passableDict.ContainsKey(9))
                            passableDict.Add(9, obs2.Passable);

                    }
                    else if (number == 9)//Principal
                    {
                        Point startLocationGrid = new Point(x, y+2);
                        r = new Random();
                        int rInt = r.Next(192, 320);
                        Enemy en = new Principal(world,this,player,5,
                        new Vector2(60, 60),
                        100,0.5f,
                        new Vector2(x * 64 + size/2, y * 64),startLocationGrid,font,10, rInt, 5,5,5,5);
                        en.setAstarsquare(texture);
                        en.GridLocation = startLocationGrid;                       
                        Enemies.Add(en);
                    }
                    

                    else if (number == 10)//groundedEnemey
                    {
                        Point startLocationGrid = new Point(x, y);
                        r = new Random();
                        int rInt = r.Next(192, 320);
                        Enemy en = new groundedEnemy(world, this, player, 5,
                        new Vector2(60, 60),
                        100, 0.5f,
                        new Vector2(x * 64 + size / 2, y * 64), startLocationGrid, font, 10, rInt, 5, 5, 5, 5);
                        en.setAstarsquare(texture);
                        en.GridLocation = startLocationGrid;
                        Enemies.Add(en);

                    }

                    else if (number == 11)//HealthBooster
                    {
                        HealthBooster hb = new HealthBooster(world, content, player, 3, new Vector2(x * 64, y * 64),new Vector2(15,15));
                        hb.Position = new Vector2(x * size , y * size);
                        HealthBoostersList.Add(hb);
                    }

                    width = (x + 1) * size;
                    height = (y + 1) * size;

                }
            }
            grid = map;

        }

        public void Update(GameTime gameTime)
        {
                       
            player.GridLocation = new Point(scaleToGrid(player.Position.X / 64), ((int)player.Position.Y / 64)+2);

            foreach (Enemy en in Enemies)
            {

                en.Update(gameTime, player.Position, player.IsAlive,20);
                Point enemeyGridLocation = new Point(scaleToGrid(en.Position.X / 64), ((int)en.Position.Y / 64) +2);
                en.GridLocation = enemeyGridLocation;
                if (en.Health == 0)
                {
                    enemies_counter--;
                }
            }

            Enemies.RemoveAll(Enemy => Enemy.Health == 0);

            foreach (Obstacle ob in obstacleTiles)
            {
                Point obstacleGridLocation = new Point((int)ob.Position.X / 64, (int)ob.Position.Y / 64);

                ob.GridLocation = obstacleGridLocation;

            }

            foreach (HealthBooster hb in HealthBoostersList)
            {
                hb.Update(gameTime);
            }

            
        }
        //for debbbuging
        public void DrawGrid(GameTime gameTime, int[,] gameMap, SpriteBatch spriteBatch, SpriteFont f)
        {
            for (int x = 0; x < gameMap.GetLength(1); x++)
            {
                for (int y = 0; y < gameMap.GetLength(0); y++)
                {
                    spriteBatch.DrawString(f, x + " / " + y, new Vector2(x * 64, y * 64), Color.White);
                }

            }
        }

        public void DrawObstacle(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Obstacle ob in ObstacleTiles)
                ob.Draw(gameTime, spriteBatch);
             
        }

        public void DrawEnemies(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (Enemy en in Enemies)
                en.Draw(gameTime, spriteBatch,font);
        }

        public void DrawHealthBoosters(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (HealthBooster hb in HealthBoostersList)
            {
                hb.Draw(gameTime,spriteBatch);
            }
        }

    public int getGridObject(int x,int y)
        {
            if (x > 0 && y > 0)
                return Grid[y, x];
            else
                return 100;
        }

        public bool isPassable(int x, int y)
        {
            if (x > 1 && y > 1 && x<width/64 && y<height/64)
            {
                if (passableDict.TryGetValue(Grid[y, x], out bool check))
                {
                    if (check)
                        return true;
                    else
                        return false;
                }
            }
 
            return false;
        }

        private int scaleToGrid(float x)//scale the difrrences between grid(int) and physics world(float)
        {
            float tmp = x;
            int scale = (int)tmp;
            if (tmp > (float)scale)
                return scale;
            else
                return --scale;
            
        }
    }
}