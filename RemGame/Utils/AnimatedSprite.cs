using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace RemGame
{
    public class AnimatedSprite
    {
        private int currentFrame;
        private int totalFrames;
        private double timer;
        private double speed;
        private Rectangle scale;
        private bool isLooped;
        SpriteEffects flip = SpriteEffects.FlipHorizontally;

        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int CurrentFrame { get => currentFrame; set => currentFrame = value; }
        public bool IsLooped { get => isLooped; set => isLooped = value; }

        public AnimatedSprite(Texture2D texture, int rows, int columns,Rectangle scale, float rate)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            CurrentFrame = 0;
            totalFrames = Rows * Columns;
            speed = rate;
            timer = speed;
            this.scale = scale;
            IsLooped = false;

        }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                CurrentFrame++;
                timer = speed;
            }
            if (CurrentFrame == totalFrames)
            {
                CurrentFrame = 0;
                IsLooped = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destination,Body body,bool toLeft)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)CurrentFrame / (float)Columns);
            int column = CurrentFrame % Columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);


            Rectangle destinationRectangle = new Rectangle (destination.X+scale.X,destination.Y+scale.Y,destination.Width+scale.Width,destination.Height+scale.Height);
            //case figures tun left - > flip animation side.
            if(toLeft)
                spriteBatch.Draw(Texture, null, destinationRectangle, sourceRectangle, null, 0.0f, null, Color.White, flip, 0.0f);
            else
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);





        }
        
    }
}