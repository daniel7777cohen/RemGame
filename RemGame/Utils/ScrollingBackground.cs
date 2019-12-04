using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace RemGame
{

    class Backgrounds
    {
        protected Texture2D texture;
        protected Rectangle rectangle;
        protected int scrollingSpeed;
        protected Scrollingbackground twinSc;
        protected float scrollingDelta = 0;


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }

    class Scrollingbackground : Backgrounds
    {
        public Scrollingbackground(Texture2D newTexture, Rectangle newRectangle, int newScrollingSpeed, Scrollingbackground sC)
        {
            texture = newTexture;
            rectangle = newRectangle;
            scrollingSpeed = newScrollingSpeed;
            twinSc = sC;
        }

        public void setRighttwinSc(Scrollingbackground sC)
        {
            this.twinSc = sC;
        }
        //Attach 2 exact background pictures and scroll them vertically on screen.
        //switch theire places by movement direction
        public void Update(Camera2D cam, int direction)
        {
           
                if (direction == -1)
                {
                    if (rectangle.X + texture.Width < cam.Position.X)
                        rectangle.X = twinSc.rectangle.X + texture.Width - 5;

                }

                else
                {
                    if (rectangle.X > cam.Position.X + texture.Width)
                        rectangle.X = twinSc.rectangle.X - texture.Width + 5;

                }
                if (scrollingSpeed != 0)
                {

                    rectangle.X += 1 * direction;

                }
            }
        
        }

}