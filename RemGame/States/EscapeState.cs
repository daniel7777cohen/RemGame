using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RemGame
{
    public sealed class EscapeState : BaseGameState, IEscapeState
    {
        private Texture2D texture;

        private SpriteFont font;
        private int selected;
        private string[] entries =
        {
            "Resume Match",
            "Options",
            "Back To Main Menu"
        };

        

        public EscapeState(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(IEscapeState)) == null)
                game.Services.AddService(typeof(IEscapeState), this);

            selected = 0;
        }

        public override void Update(GameTime gameTime)
        {

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Down))
            {
                selected++;
                if (selected > 2)
                    selected = 2;
            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Up))
            {
                selected--;
                if (selected < 0)
                    selected = 0;
            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Enter))
            {
                if(selected ==0)
                StateManager.PopState();
                else if (selected == 1)
                {
                    StateManager.PushState(OurGame.OptionsMenuState.Value);
                }
                else if(selected == 2)
                {
                    StateManager.PopState();
                    StateManager.PopState();
                    OurGame.Reset();
                    StateManager.PushState(OurGame.StartMenuState.Value);
                }

            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"ScreenDisplay\optionsMenu");
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                          Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(texture.Width / 2,
                                         texture.Height / 2);
            Vector2 optionPos = new Vector2(pos.X - texture.Width / 2 + 20, pos.Y / 2 + 50);
            Vector2 valuePos = new Vector2(pos.X + texture.Width / 2 - 400, optionPos.Y);

            OurGame.SpriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, origin, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
            for (int i = 0; i < entries.Length; i++)
            {
                Color color;
                float scale = 1;

                if (i == selected)
                    color = Color.White;
                else
                    color = Color.Blue;

                Vector2 fontOrigin = new Vector2(0, font.LineSpacing / 2);
                Vector2 optionShadowPos = new Vector2(optionPos.X - 2, optionPos.Y - 2);
                Vector2 valueShadowPos = new Vector2(valuePos.X - 2, valuePos.Y - 2);

                // Draw Shadow
                OurGame.SpriteBatch.DrawString(font, entries[i], optionShadowPos, Color.Black, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                // Draw Text
                OurGame.SpriteBatch.DrawString(font, entries[i], optionPos, color, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                optionPos.Y += font.LineSpacing;
                valuePos.Y += font.LineSpacing;
            }


            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            // Change to visible if not at the top of the stack
            // This way, sub menus will appear on top of this menu
            if (StateManager.State != this.Value)
                Visible = true;
            else
                selected = 0;
        }

    }
}
