using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RemGame
{
    public class Obstacle : Component
    {

        private Body body;
        private Texture2D texture;
        private Vector2 size;
        private bool kinesisOn = false;
        private bool inAir = false;
        private PhysicsView p;
        private SpriteFont f;
        private bool passable;


        private Point gridLocation;


        public Obstacle(World world, Texture2D texture, Vector2 size,SpriteFont font,bool passable)
        {
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            this.size = size;
            this.texture = texture;
            p = new PhysicsView(body,body.Position,size,font);
            body.CollisionCategories = Category.Cat1;
            f = font;
            this.passable = passable;
        }

        public Body Body { get => body; set => body = value; }
        public Texture2D Texture { get => texture; set => texture = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }
        public bool KinesisOn { get => kinesisOn; set => kinesisOn = value; }
        public bool InAir { get => inAir; set => inAir = value; }
        public Point GridLocation { get => gridLocation; set => gridLocation = value; }
        public bool Passable { get => passable;}

        public Rectangle physicsObjRecToDraw()
        {
            Rectangle destination = new Rectangle
            (
                (int)Position.X ,
                (int)Position.Y,
                (int)Size.X,
                (int)Size.Y
            );
            return destination;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Position.Y < -300)
            {

                this.Body.Enabled = false;

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Rectangle destination = new Rectangle
           (
               (int)Position.X,
               (int)Position.Y + (int)Size.Y / 2,
               (int)Size.X,
               (int)Size.Y
           );
            //spriteBatch.Draw(texture, destination, null, Color.White);
            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
            //p.Draw(gameTime,spriteBatch);

        }
    }
}


