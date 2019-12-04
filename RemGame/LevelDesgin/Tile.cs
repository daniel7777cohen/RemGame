using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    class Tile
    {
        protected Texture2D texture;

        private Rectangle rectangle;
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private Body body;
        World world;
        private Vector2 size;
        private PhysicsView p;
        public SpriteFont font;

        private bool kinesisOn = false;

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private static ContentManager content;

        public Tile(int i, Rectangle newRectangle, World world, Vector2 size)
        {
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            this.size = size;
            texture = Content.Load<Texture2D>("Tiles/tile" + i);
            p = new PhysicsView(body, body.Position, size, font);
        }

        public Rectangle Rectangle { get => rectangle; protected set => rectangle = value; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => Body.Position * CoordinateHelper.unitToPixel; set => Body.Position = value * CoordinateHelper.pixelToUnit; }
        public World World { get => world; set => world = value; }
        public Body Body { get => body; set => body = value; }
        public PhysicsView P { get => p; set => p = value; }
        

        /*
public void Draw(SpriteBatch spriteBatch)
{
spriteBatch.Draw(texture, rectangle, Color.White);
}
*/
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, Rectangle, null, Color.White);
            //spriteBatch.Draw(texture, Rectangle, null, Color.White, Body.Rotation, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
            //Console.WriteLine(Body.Position);
            p.Draw(gameTime, spriteBatch);
        }

    }
    /*
    class CollisionTiles : Tile
    {
        public CollisionTiles(int i, Rectangle newRectangle ,World world,Vector2 size,SpriteFont font)
        {

            texture = Content.Load<Texture2D>("Tiles/tile" + i);
            this.Rectangle = newRectangle;
            this.Size = size;
            this.World = world;
            this.Body = BodyFactory.CreateRectangle(World, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            this.Body.Position = new Vector2(newRectangle.X,newRectangle.Y);
            this.P = new PhysicsView(Body, new Vector2(newRectangle.X, newRectangle.Y), size, font);
            this.font = font;


        }
    }
    */
}
