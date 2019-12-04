using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;


namespace RemGame
{
    /// <summary>
    /// this calss draw the bounderies (4 points at top,bottom,left,right) of a physic object.
    /// </summary>
    class PhysicsView:Component
    {
        SpriteFont font;
        Body body;
        Vector2 position;
        Vector2 textureSize;
       

        public PhysicsView(Body body,Vector2 position,Vector2 size, SpriteFont f)
        {
            this.body = body;
            this.position = position;
            this.textureSize = size;
            this.font = f;

        }

        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }
        public SpriteFont Font { get => font;}

        
    

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (font != null)
            {
                
                spriteBatch.DrawString(font, "*" + (int)Position.X + "/" + (int)Position.Y, new Vector2(Position.X, Position.Y), Color.White);
                spriteBatch.DrawString(font, "*" + (int)(Position.Y + textureSize.Y), new Vector2(Position.X, (Position.Y + textureSize.Y)), Color.White);
                spriteBatch.DrawString(font, "*" + ((int)Position.X - (int)textureSize.X / 2) + "/" + (int)(Position.Y + textureSize.X / 2), new Vector2(Position.X - textureSize.X / 2, Position.Y + textureSize.X / 2), Color.White);
                spriteBatch.DrawString(font, "*" + ((int)Position.X + (int)textureSize.X / 2), new Vector2(Position.X + textureSize.X / 2, Position.Y + textureSize.Y / 2), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
