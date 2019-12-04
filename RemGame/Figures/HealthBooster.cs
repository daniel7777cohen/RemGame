using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RemGame
{
    class HealthBooster
    {
        private World world;
        private Body body;
        private int value;
        private bool activated;
        private Kid player;
        private Texture2D texture;
        private Rectangle collisionRec;
        private Vector2 startingLocation;
        private float floatingDirection;


        public HealthBooster (World world,ContentManager Content,Kid player,int value,Vector2 position,Vector2 size)
        {
            this.Value = value;
            this.player = player;
            this.world = world;
            Activated = false;
            if(AssetsDictionary.getDictionaryUsed())
            texture = AssetsDictionary.getDictionary()["HealthBooster"];
            else texture = Content.Load<Texture2D>("misc/HealthBooster");
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            body.BodyType = BodyType.Dynamic;
            body.IgnoreGravity = true;
            body.CollisionCategories = Category.Cat5;
            collisionRec = new Rectangle((int)position.X, (int)position.Y,(int)size.X*2,(int)size.X*2);
            startingLocation = position;
            floatingDirection = -1;
        }

        public int Value { get => value; set => this.value = value; }
        public bool Activated { get => activated; set => activated = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }

        public void Update(GameTime gameTime)
        {
            if (collisionRec.Intersects(player.CollisionRec))
            {
                if (!activated)
                {
                    player.Health += value;
                    player.HealthBar.increase(value);
                    activated = true;
                }
            }

            if (activated)
            {
                body.ApplyForce(new Vector2(-0.1f, -0.2f));
                body.ApplyTorque(0.001f);
            }

            else
            {
                body.ApplyForce(new Vector2(0, floatingDirection));

                if (Position.Y <= startingLocation.Y - 15)
                    floatingDirection = 0.3f;

                else if (Position.Y + 15 >= startingLocation.Y)
                    floatingDirection = -0.3f;

            }

            if (this.Position.Y < -300)
            {
                this.body.Enabled = false;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Rectangle destination = new Rectangle
           (
               (int)Position.X,
               (int)Position.Y,
               55,
               55
           );

            if (body.Enabled)
            {
                spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width, texture.Height), SpriteEffects.None, 0);
            }
        }

    }
}
