using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    class HealthBar
    {
        private Texture2D texture;
        private Rectangle rectangle;
        public Rectangle getRectangle { get => rectangle; set => rectangle = value; }
        
        public HealthBar(ContentManager Content)
        {
            if(AssetsDictionary.getDictionaryUsed())
                texture = AssetsDictionary.getDictionary()["HealthBar"];
            else texture = Content.Load<Texture2D>("misc/HealthBar");
            rectangle = new Rectangle(0, 0, texture.Width*3, texture.Height);
        }
        
        public void decrease(int damage)
        {
            this.rectangle.Width -= damage;
        }

        public void increase(int value)
        {
            this.rectangle.Width += value;
        }

        public void Draw(SpriteBatch spriteBatch,Camera2D cam)
        {
 
            spriteBatch.Draw(texture, new Vector2(cam.Position.X, cam.Position.Y),rectangle,Color.White); 
           
        }

        public void Update()
        {

        }
    }

}
