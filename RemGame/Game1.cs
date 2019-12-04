using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using XELibrary;

namespace RemGame
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;


        public SpriteBatch SpriteBatch { get; private set; }

        CelAnimationManager celAnimationManager;
        ScrollingBackgroundManager scrollingBackgroundManager;
        InputHandler inputHandler;
        GameStateManager stateManager;

        public ILoadLevelState LoadLevelState;
        public ITitleIntroState TitleIntroState;
        public IStartMenuState StartMenuState;
        public IPlayingState PlayingState;
        public IMissionOne Mission1;
        public IEscapeState EscapeState;
        public IPausedState PausedState;
        public IOptionsMenuState OptionsMenuState;
        public IGameOverState GameOverState;
        public IMissionCompleteState MissionCompleteState;
        public bool EnableSoundFx { get; set; }
        public bool EnableMusic { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            inputHandler = new InputHandler(this);
            Components.Add(inputHandler);

            celAnimationManager = new CelAnimationManager(this, "Textures\\");
            Components.Add(celAnimationManager);

            scrollingBackgroundManager = new ScrollingBackgroundManager(this, "Textures\\");
            Components.Add(scrollingBackgroundManager);

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            LoadLevelState = new LoadLevelState(this);
            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);
            PlayingState = new PlayingState(this);
            Mission1 = new Mission1(this);
            EscapeState = new EscapeState(this);
            GameOverState = new GameOverState(this);
            MissionCompleteState = new MissionCompleteState(this);
            EnableSoundFx = true;
            EnableMusic = true;

        }
        public void Reset()
        {
            base.Initialize();
            base.BeginRun();
            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);
            PlayingState = new PlayingState(this);
            Mission1 = new Mission1(this);
            EscapeState = new EscapeState(this);
            GameOverState = new GameOverState(this);
            MissionCompleteState = new MissionCompleteState(this);
            LoadLevelState = new LoadLevelState(this);
        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void BeginRun()
        {
            stateManager.ChangeState(StartMenuState.Value);
            IsMouseVisible = false;

            base.Initialize();
            base.BeginRun();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            base.Draw(gameTime);
            SpriteBatch.End();
        }
    }
}