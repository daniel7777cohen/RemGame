using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace RemGame
{
    class Chair : Obstacle
    {
        public Chair(World world, Texture2D texture, Vector2 size, SpriteFont font,bool passable) : base(world, texture, size, font, passable)
        {

        }
    }
}
