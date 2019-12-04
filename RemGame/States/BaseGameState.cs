using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using XELibrary;

namespace RemGame 
{
    public abstract class BaseGameState : GameState
    {
        protected Game1 OurGame;
        protected ContentManager Content;

        public BaseGameState(Game game)
            : base(game)
        {
            Content = game.Content;
            OurGame = (Game1)game;
        }
    }
}
