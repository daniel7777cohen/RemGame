using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RemGame
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture;
        private SpriteFont font;
        private Song generalMusic;

        private int selected;
        private string[] entries =
        {
            "Tutorial",
            "Mission1",
            "Load Level",
            "Options",
            "Exit Game"
        };

        public StartMenuState(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(IStartMenuState)) == null)
                game.Services.AddService(typeof(IStartMenuState), this);
            selected = 0;

        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"ScreenDisplay\forest_shot");
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
            generalMusic = Content.Load<Song>(@"Sound/Music/General_Music_1");
            MediaPlayer.Play(generalMusic);

        }

        public override void Update(GameTime gameTime)
        {
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
            {
                // Go back to title screen
                StateManager.ChangeState(OurGame.TitleIntroState.Value);
            }


            if (Input.KeyboardHandler.WasKeyPressed(Keys.Up))
                selected--;
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Down))
                selected++;

            if (selected < 0)
                selected = entries.Length - 1;
            if (selected > entries.Length - 1)
                selected = 0;

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Enter))
            {
                switch (selected)
                {
                    case 0:
                        // Got back here from playing the game. So just pop myself off the stack
                        if (StateManager.ContainsState(OurGame.PlayingState.Value))
                            StateManager.PopState();
                        else // Starting a new game.
                            MediaPlayer.Stop();
                        StateManager.ChangeState(OurGame.PlayingState.Value);

                        break;
                    case 1:
                        // Got back here from playing the game. So just pop myself off the stack
                        if (StateManager.ContainsState(OurGame.Mission1.Value))
                            StateManager.PopState();
                        else // Starting a new game.
                        {
                            StateManager.ChangeState(OurGame.Mission1.Value);
                        }
                        break;
                    case 2:
                        StateManager.ChangeState(OurGame.LoadLevelState.Value);
                        break;
                    case 3:
                        StateManager.PushState(OurGame.OptionsMenuState.Value);
                        break;
                    case 4:
                        StateManager.ChangeState(OurGame.TitleIntroState.Value);
                        break;
                }
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                          Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(texture.Width / 2,
                                         texture.Height / 2);
            Vector2 currPos = new Vector2(100, pos.Y / 2);

            OurGame.SpriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, origin, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
            for (int i = 0; i < entries.Length; i++)
            {
                Color color;
                float scale;

                if (i == selected)
                {
                    double time = gameTime.TotalGameTime.TotalSeconds;
                    float pulsate = (float)Math.Sin(time * 12) + 1;
                    color = Color.YellowGreen;
                    scale = 1 + pulsate * 0.05f;
                }
                else
                {
                    color = Color.LightGoldenrodYellow;
                    scale = 1;
                }

                Vector2 fontOrigin = new Vector2(0, font.LineSpacing / 2);
                Vector2 shadowPos = new Vector2(currPos.X - 2, currPos.Y - 2);

                // Draw Shadow
                OurGame.SpriteBatch.DrawString(font, entries[i], shadowPos, Color.Black, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                // Draw Text
                OurGame.SpriteBatch.DrawString(font, entries[i], currPos, color, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                currPos.Y += font.LineSpacing;
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
        }

    }
}