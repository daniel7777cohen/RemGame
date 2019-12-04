using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using LevelDesignerGui;
using System.Linq;
namespace RemGame
{
    static class AssetsDictionary
    {
       static Dictionary<string, Texture2D> assetsDictionary;
        static AssetsDictionary() {
            assetsDictionary = new Dictionary<string, Texture2D>();
        }
        public static Dictionary<string,Texture2D> getDictionary() { return assetsDictionary; }
        public static void setDictionary(Dictionary<string,Texture2D> AD) {  assetsDictionary = AD; setDictionaryUsed(true); }
        public static bool dictionary_used = false;
        public static void setDictionaryUsed(bool flag) { dictionary_used = flag; }
        public static bool getDictionaryUsed() { return dictionary_used; }
    }

    public sealed class LoadLevelState : BaseGameState, ILoadLevelState
    {
        SpriteBatch spriteBatch;
        LDG LevelDesigner;
      
        private Color _backgroundColor = Color.CornflowerBlue;
        private List<Component> _gameComponents;
        World world;
        Kid player;
        private bool isRonAlive = true;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        MouseState currentMouseState;
        MouseState previousMouseState;

        Dictionary<string, string> musics;
        SpriteFont font;
        Camera2D cam;
        Vector2 camLocation;

        //both as an object and as a grid.
        Map map;
        int[,] mapFromXml;

        //level feature - cahsing wall
        Rectangle closingWallRec;
        bool debugStopWall = false;

        //delays end of the level after death
        DateTime deathTimer;
        float deathInterval = 2.0f;

        //delays end of the level after completion
        DateTime missionCompleteTimer;
        bool isMissionComplete = false;
        float missionCompleteInterval = 2.0f;

        List<Texture2D> jpegList;
        SoundEffect hallEffects;
        SoundEffectInstance hallInstance;//has more featurs
        Song backgroundMusic;

        public LoadLevelState(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(ILoadLevelState)) == null)
                game.Services.AddService(typeof(ILoadLevelState), this);
            LevelDesigner = new LDG();
            jpegList = new List<Texture2D>();
            musics = new Dictionary<string, string>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            jpegList = LevelDesigner.LoadMap(GraphicsDevice);//get images from xml
            mapFromXml = LevelDesigner.LoadMapMatrix();//get matrix from xml
            AssetsDictionary.setDictionary(LevelDesigner.LoadAssets(GraphicsDevice));//get Assets from xml
            musics = LevelDesigner.LoadMusics(GraphicsDevice);
            Tile.Content = Content;
            Map.Content = Content;
            Enemy.Content = Content;
            Kid.Content = Content;

            cam = new Camera2D(GraphicsDevice);//intialize camera
            world = new World(new Vector2(0, 9.8f));//setting physics world settings
            Console.WriteLine("output" + " "+musics.First().Value);
                var song = Song.FromUri(musics.First().Value, new Uri(musics.First().Value, UriKind.Relative));
            backgroundMusic = song;
            MediaPlayer.Volume = 2.0f;

            if (OurGame.EnableMusic == true)
            {
                MediaPlayer.Play(backgroundMusic);
                MediaPlayer.IsRepeating = true;
            }
           

            hallEffects = Content.Load<SoundEffect>("Sound/FX/Level1/Hallway_Atmosphere");
            hallInstance = hallEffects.CreateInstance();
            hallInstance.IsLooped = true;
            hallInstance.Play();
            hallInstance.Volume = 0.05f;

            font = Content.Load<SpriteFont>("Fonts/Font");
            player = new Kid(cam, world,
                new Vector2(60, 60),
                100,
            cam.ScreenToWorld(new Vector2(950, 400)), false, font);
            map = new Map(world, player);
            map.Generate(mapFromXml, 64, font);//every digit in the grid represents an object
          
            player.setMap(map);
         
            closingWallRec = new Rectangle((int)cam.Position.X, -50, 400, 1000);

            _gameComponents = new List<Component>()
            {
                player,
            };
        }

        public override void Update(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();

            /////for debbug,needs tobe only one call to handle input with gameTime alone
            handleInput(gameTime, debugStopWall);
            if (keyboardState.IsKeyDown(Keys.O) && !prevKeyboardState.IsKeyDown(Keys.O))
            {
                if (debugStopWall)
                    debugStopWall = false;
                else
                    debugStopWall = true;

            }

            //case closingWall hits player -> player done,level faild.

            if (closingWallRec.X + closingWallRec.Width + 20 > player.Position.X)
            {
                player.HealthBar.decrease(6);
                if (player.IsAlive)
                    deathTimer = DateTime.Now;

                player.IsAlive = false;
            }

            if ((currentMouseState.LeftButton == ButtonState.Pressed) && (currentMouseState.RightButton == ButtonState.Pressed))
            {
                Console.WriteLine("kinesis!!!!!");
                // Console.WriteLine(cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)));
                foreach (Obstacle obj in map.ObstacleTiles)
                {
                    Vector2 mouseToWorld = cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y));
                    if (mouseToWorld.X >= obj.Position.X - 35 && mouseToWorld.X <= obj.Position.X + 35 && mouseToWorld.Y >= obj.Position.Y && mouseToWorld.Y <= obj.Position.Y + 70)
                        player.Kinesis(obj, currentMouseState);


                }
            }
            //

            foreach (var component in _gameComponents)
                component.Update(gameTime);

            camLocation = player.CameraToFollow;//set following camera features by subtracting vectors

            cam.LookAt(camLocation);
            //cam.Rotate(0.0015f);

            map.Update(gameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            prevKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        private void handleInput(GameTime gameTime, bool stopWall)
        {

            if (player.FirstMove == true)
                if (stopWall != true)
                    closingWallRec.X += 2;

            if (map.Enemies_counter == 3)
            {
                if (!isMissionComplete)
                {
                    missionCompleteTimer = DateTime.Now;
                    isMissionComplete = true;
                }

                if ((DateTime.Now - missionCompleteTimer).TotalSeconds >= missionCompleteInterval)
                {
                    StateManager.PopState();
                    StateManager.PushState(OurGame.MissionCompleteState.Value);
                }
            }

            if (player.IsAlive)
            {
                deathTimer = DateTime.Now;
            }
            if ((DateTime.Now - deathTimer).TotalSeconds >= deathInterval)
            {

                StateManager.PopState();
                StateManager.PushState(OurGame.StartMenuState.Value);
                OurGame.Reset();
                player.IsAlive = true;
            }


            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
            {
                StateManager.PushState(OurGame.EscapeState.Value);

            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.P))
            {
                StateManager.PushState(OurGame.PausedState.Value);
                hallInstance.Stop();
                MediaPlayer.Pause();
            }
            if (!(StateManager.State == OurGame.PausedState.Value))
            {
                hallInstance.Play();
                MediaPlayer.Resume();
            }

            previousMouseState = currentMouseState;

            if (OurGame.EnableMusic == false)
                MediaPlayer.Pause();
            else MediaPlayer.Resume();

        }

        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(_backgroundColor);
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix());
           
            for (int i = 0; i < jpegList.Count; i++)
            {
                
                Texture2D element = jpegList.ElementAt(i);
                spriteBatch.Draw(element, new Rectangle(i * element.Width, -700, element.Width, element.Height), Color.White);
            }

            spriteBatch.Draw(AssetsDictionary.getDictionary()["Hands_temp"], closingWallRec, null, Color.White);

            //for debbuging purpose - show x & y coordinates of the objects            
         
            map.DrawEnemies(gameTime, spriteBatch);
            map.DrawHealthBoosters(gameTime, spriteBatch);

            foreach (var component in _gameComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }

            player.HealthBar.Draw(spriteBatch, cam);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool IsRonAlive { get => isRonAlive; set => isRonAlive = value; }

    }
}