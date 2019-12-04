using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace LevelDesignerGui
{
    class Game1 : Game
    {
        const string FILENAME_Map = "levelMap.xml";
        const string FILENAME_Assets = "levelAssets.xml";
        const string FILENAME_Sound = "levelSound.xml";
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        int[,] map;
        int totalWidth = 0;

        Point offset = new Point();//Offset for scrolling 
        int scrollSpeed = 7;
        Texture2D onePixelTex;
        
        // Flags to hide/show menu and grid
        bool isMenuActive = true;
        bool isGridActive = true;
   
        bool isDone = false;
        bool add_sound_flag = false;
        bool add_assets_flag = false;
        bool load_map = false;
        // Menu
        Texture2D[] tileStatusTex;
        Rectangle[] tileRects = new Rectangle[5];

        Color[] tileStatusColor = new Color[] { Color.Red, Color.Green, Color.Blue,Color.Black ,Color.DimGray};
        int selectedTileStatus = 0;

        Point menuPos = new Point(100, 100);
        Point btnOffset = new Point();
        
        //buttons
        Rectangle btnSaveImgRect, btnAddImgRect, btnAddMusicRect, btnAddAssetsRect, btnDoneAddImgRect;
        Texture2D btnAddImgTex, btnAddMusicTex, btnDoneAddImgTex, btnAddAssetsTex, btnSaveImgTex;
        struct AssetInfo
        {
            public string name;
            public string path;
        }

        List<AssetInfo> imagesToAdd = new List<AssetInfo>();
        List<AssetInfo> assetsToAdd= new List<AssetInfo>();
        List<AssetInfo> musicsToAdd = new List<AssetInfo>();
        List<Texture2D> jpegList = new List<Texture2D>();
        List<Texture2D> jpegAssetsList = new List<Texture2D>();

        // Keyboard state to be able to detect key presses
        KeyboardState prevKeyState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile =GraphicsProfile.HiDef;
        }

        public void DrawGrid(int[,] gameMap, SpriteBatch spriteBatch, SpriteFont f)
        {
            
            for (int x = 0; x < gameMap.GetLength(1); x++)
            {
                for (int y = 0; y < gameMap.GetLength(0); y++)
                {
                    Color color;
                    Texture2D img;
                    if (map[y, x] == 9)
                    {
                        color = tileStatusColor[2];
                        img = tileStatusTex[2];
                    }
                    else if (map[y, x] == 10)
                    {
                        color = tileStatusColor[3];
                        img = tileStatusTex[3];
                        
                    }
                    else if (map[y, x] == 11)
                    {
                        color = tileStatusColor[4];
                        img = tileStatusTex[4];
         
                    }

                    else
                    {
                        color = tileStatusColor[map[y, x]];
                        img = tileStatusTex[0];
                    }
                    if (isGridActive)
                    {
                        
                        DrawBorder(new Rectangle(y * 64 + offset.X, x * 64, 64, 64), 2, color);
                        if (!img.Equals(tileStatusTex[0]))
                        spriteBatch.Draw(img,new Rectangle(y*64+offset.X,x*64,64,64), Color.White);

                    }
                }
            }
        }

        private void DrawBorder(Rectangle rect, int thicknessOfBorder, Color borderColor)
        {
            var pixel = onePixelTex;

            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thicknessOfBorder, rect.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel,
                new Rectangle(
                    (rect.X + rect.Width - thicknessOfBorder),
                    rect.Y,
                    thicknessOfBorder,
                    rect.Height),
                borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel,
                new Rectangle(
                    rect.X,
                    rect.Y + rect.Height - thicknessOfBorder,
                    rect.Width,
                    thicknessOfBorder),
                borderColor);
        }
        private void DrawMenu()
        {
            // background
            spriteBatch.Draw(onePixelTex, new Rectangle(menuPos.X, menuPos.Y, 400, 200), Color.Brown);

            // state buttons
            int x = menuPos.X + btnOffset.X;
            var buttonSize = new Point(50, 50);

            for (int i = 0; i < tileStatusTex.Length; i++)
            {
                tileRects[i] = new Rectangle(x, menuPos.Y + btnOffset.Y, buttonSize.X, buttonSize.Y);
                spriteBatch.Draw(tileStatusTex[i], tileRects[i], Color.White);
                x += btnOffset.X + buttonSize.X;
            }
      
            DrawBorder(tileRects[selectedTileStatus], 2, Color.Aqua);
            // add image button
            int y = menuPos.Y + btnOffset.Y + buttonSize.Y + 30;
            btnAddImgRect = new Rectangle(menuPos.X + btnOffset.X, y, buttonSize.X, buttonSize.Y);
            spriteBatch.Draw(btnAddImgTex, btnAddImgRect, Color.White);

            // add music button
            btnAddMusicRect = new Rectangle(menuPos.X + btnOffset.X * 2 + buttonSize.X, y, buttonSize.X, buttonSize.Y);
            spriteBatch.Draw(btnAddMusicTex, btnAddMusicRect, Color.White);
            
            // add Assets button
            btnAddAssetsRect = new Rectangle(menuPos.X + btnOffset.X + 50 + buttonSize.X, y, buttonSize.X, buttonSize.Y);
            spriteBatch.Draw(btnAddAssetsTex, btnAddAssetsRect, Color.White);
            
            // add Done button
            btnDoneAddImgRect = new Rectangle(menuPos.X + btnOffset.X + 110 + buttonSize.X, y, buttonSize.X, buttonSize.Y);
            spriteBatch.Draw(btnDoneAddImgTex, btnDoneAddImgRect, Color.White);
            btnSaveImgRect = new Rectangle(menuPos.X + btnOffset.X + 170 + buttonSize.X, y, buttonSize.X, buttonSize.Y);
            spriteBatch.Draw(btnSaveImgTex, btnSaveImgRect, Color.White);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }
        protected override void LoadContent()
        {
            this.IsMouseVisible = true;
            map = new int[0, 0];
            font = Content.Load<SpriteFont>("Fonts/Font");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            prevKeyState = Keyboard.GetState();

            tileStatusTex = new Texture2D[5];
            tileStatusTex[0] = Content.Load<Texture2D>("Menu/Empty");
            tileStatusTex[1] = Content.Load<Texture2D>("Menu/Collision");
            tileStatusTex[2] = Content.Load<Texture2D>("Menu/Principal");
            tileStatusTex[3] = Content.Load<Texture2D>("Menu/Kid");
            tileStatusTex[4] = Content.Load<Texture2D>("Menu/HealthBooster");

            btnAddImgTex = Content.Load<Texture2D>("Menu/AddMap");
            btnAddAssetsTex = Content.Load<Texture2D>("Menu/AddAsset");
            btnAddMusicTex = Content.Load<Texture2D>("Menu/AddSound");
            btnDoneAddImgTex = Content.Load<Texture2D>("Menu/DoneAdd");
            btnSaveImgTex = Content.Load<Texture2D>("Menu/Save");

            onePixelTex = new Texture2D(GraphicsDevice, 1, 1);
            onePixelTex.SetData(new Color[] { Color.White });         
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
     
        protected override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                Exit();
            if (IsKeyPressed(keyState, Keys.L))
            {
                map = LoadMap();
                load_map = true;
            }

            // Toggle menu
            if (IsKeyPressed(keyState, Keys.I))
            {
                isMenuActive = !isMenuActive;
            }

            // Toggle grid
            if (IsKeyPressed(keyState, Keys.G))
            {
                isGridActive = !isGridActive;
            }

            if (IsKeyPressed(keyState, Keys.D1))
            {
                selectedTileStatus = 0;//empty
            }
            else if (IsKeyPressed(keyState, Keys.D2))
            {
                selectedTileStatus = 1;//collision
            }
            else if (IsKeyPressed(keyState, Keys.D3))
            {
                selectedTileStatus = 2;//enemy
            }

            // Save
            if (IsKeyPressed(keyState, Keys.S))
            {
                Save();
            }
            if (IsKeyPressed(keyState, Keys.C))
            {
                CopyFiles();
            }

            // Scroll offset X
            if (keyState.IsKeyDown(Keys.Right))
            {
                offset.X -= scrollSpeed;
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                offset.X += scrollSpeed;
            }


            MouseState ms = Mouse.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                
                var xIndex = (ms.X - offset.X) / 64;
                var yIndex = (ms.Y ) / 64;
                bool handled = false;
                //for the menu to be selected
                for (int i = 0; i < tileRects.Length; i++)
                {
                    if (tileRects[i].Contains(ms.X, ms.Y))
                    {
                        selectedTileStatus = i;// choose 0 1 2 .. at menu. then use value to set at the matrix.
                        handled = true;
                    }
                }

                // check for click on image "add image"
                if (btnAddImgRect.Contains(ms.X, ms.Y))
                {
                    handled = true;
                    AddImagesForMap();
                }
                // check for click on image "save"
                if (btnSaveImgRect.Contains(ms.X, ms.Y))
                {
                    if (!add_sound_flag)
                        System.Windows.Forms.MessageBox.Show("Error", "Please make sure you've added a soundtrack file for your level", System.Windows.Forms.MessageBoxButtons.OKCancel);
                    else if(!add_assets_flag)
                        System.Windows.Forms.MessageBox.Show("Error", "Please make sure you've pressed the AddAssets button", System.Windows.Forms.MessageBoxButtons.OKCancel);
                    {
                        handled = true;
                        Save();
                    }
                }
                // check for click on image "add music"
                else if (btnAddMusicRect.Contains(ms.X, ms.Y))
                {
                    add_sound_flag = true;
                    AddMusics();
                    handled = true;
                }
                else if (btnDoneAddImgRect.Contains(ms.X, ms.Y) && isDone == false)
                {
                    isDone = true;
                    
                    foreach (Texture2D T in jpegList)
                    {
                        totalWidth += T.Width;
                    }
                    if (totalWidth != 0)
                    {
                        map = new int[totalWidth / 64, jpegList.ElementAt(0).Height / 64];
                    }
                    else isDone = false;

                    handled = true;

                }
                else if (btnAddAssetsRect.Contains(ms.X, ms.Y)&& add_assets_flag == false)
                {
                    AddAssets();
                    add_assets_flag = true;
                    System.Windows.Forms.MessageBox.Show("Built in assets addition completed: Principal_Walk.png,\n Principal_Ranged_Chalk.png,\n Principal_Stand.png,\n Chalk.png,\n Hands_temp.png\n HealthBar.jpg\n HealthBooster.png","Completed" , System.Windows.Forms.MessageBoxButtons.OKCancel);
                }

                if (!handled && xIndex >= 0 && xIndex < map.GetLength(0) && yIndex >= 0 && yIndex < map.GetLength(1))
                {
                    if (selectedTileStatus == 2)
                    {
                        if(map[xIndex+1,yIndex]== 1||map[xIndex-1,yIndex]==1||map[xIndex,yIndex+1]==1||map[xIndex,yIndex-1]==1||map[xIndex-1,yIndex-1]==1||map[xIndex+1,yIndex+1]==1||map[xIndex+1,yIndex-1]==1||map[xIndex-1,yIndex+1]==1)
                        {
                        System.Windows.Forms.MessageBox.Show("Cant locate texture too close to a colision, please make sure to have another tile between texture and collision", "Error", System.Windows.Forms.MessageBoxButtons.OKCancel);
                        }
                        else
                        map[xIndex, yIndex] = 9;
                    }
                    else if (selectedTileStatus == 3)
                    {

                        if (map[xIndex + 1, yIndex] == 1 || map[xIndex - 1, yIndex] == 1 || map[xIndex, yIndex + 1] == 1 || map[xIndex, yIndex - 1] == 1 || map[xIndex - 1, yIndex - 1] == 1 || map[xIndex + 1, yIndex + 1] == 1 || map[xIndex + 1, yIndex - 1] == 1 || map[xIndex - 1, yIndex + 1] == 1)
                        {
                            System.Windows.Forms.MessageBox.Show("Cant locate texture too close to a colision, please make sure to have another tile between texture and collision", "Error", System.Windows.Forms.MessageBoxButtons.OKCancel);
                        }
                        else
                            map[xIndex, yIndex] = 10;
                    }
                    else if (selectedTileStatus == 4)
                    {
                        map[xIndex, yIndex] = 11;
                    }
                    else
                    {
                        map[xIndex, yIndex] = selectedTileStatus;
                    }

                }

            }
          
            prevKeyState = keyState;
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
       
            //offset is needed for scrolling
            //load map images chosen by user
            for (int i = 0; i < jpegList.Count; i++)
            {
                Texture2D element = jpegList.ElementAt(i);
                spriteBatch.Draw(element, new Rectangle(i * element.Width + offset.X, -700, element.Width, element.Height), Color.White);
            }

            DrawGrid(map, spriteBatch, font);

            // draw menu only if is active
            if (isMenuActive)
            {
                DrawMenu();
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected void AddImagesForMap()
        {  
            var ofg = new System.Windows.Forms.OpenFileDialog();
            ofg.Title = "Add an image for the game map";
            if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                imagesToAdd.Add(new AssetInfo()
                {
                   
                    path = Path.GetFullPath(ofg.FileName)

                });
                var name_ = Path.GetFullPath(ofg.FileName);
                FileStream fileStream = new FileStream(name_, FileMode.Open);
                Texture2D jpegForMap = Texture2D.FromStream(GraphicsDevice, fileStream);
                jpegList.Add(jpegForMap);
                fileStream.Dispose();
            }
        }
        protected void AddAssets()
        {
            
            String[] Assets = new String[] { "Principal_Walk.png","Principal_Ranged_Chalk.png","Principal_Stand.png","Chalk.png","Hands_temp.png","HealthBar.jpg","HealthBooster.png" };                                     
            String root_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//get full path
            var regex = new Regex(Regex.Escape("bin\\Windows\\x86\\Debug"));
            root_path = regex.Replace(root_path, "Content\\Assets\\", 1);
            foreach (String str in Assets)
            {
                String root_path_temp = String.Copy(root_path);
                root_path_temp += str;
                Console.WriteLine(str);
                  Console.WriteLine(root_path_temp);
                assetsToAdd.Add(new AssetInfo()
                {
                    name = str.Substring(0,str.IndexOf(".")),
                    path = root_path_temp

                });
            }
        }
        protected void AddMusics()
        {
            var ofg = new System.Windows.Forms.OpenFileDialog();
            ofg.Title = "Add a music";
            if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                musicsToAdd.Add(new AssetInfo()
                {
                    name = Path.GetFileNameWithoutExtension(ofg.FileName),
                    path = Path.GetFullPath(ofg.FileName)

                });
                
            }
        }

        protected void CopyFiles()
        {
            foreach (var ai in imagesToAdd)
            {
                var fn = ai.path;
                var f = Path.GetFileName(fn);
                Console.WriteLine(f);
                try
                {
                    File.Copy(fn, "");
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(
                           "Error",
                        "There was an error copying file " + fn + "\n" + e.Message,
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        protected void Save()
        {

            string xml = "<Data></Data>";

            XDocument doc = XDocument.Parse(xml);
            XElement data = doc.Root;

            for (int row = 0; row <= map.GetUpperBound(1); row++)
            { 
                XElement xRow = new XElement("Row");
                data.Add(xRow);
                for (int col = 0; col <= map.GetUpperBound(0); col++)
                {
                    Console.Write(map[col, row]);
                    XElement xCol = new XElement("Column", map[col, row]);
                    xRow.Add(xCol);
                }
                Console.WriteLine("");
            }

            
            XElement images = new XElement("images");
            data.Add(images);
            foreach (var ii in imagesToAdd)
            {
                XElement img = new XElement("image");
                img.SetValue(ii.path);
                images.Add(img);
            }
            
       
            doc.Save(FILENAME_Map);

            string xmlSound = "<Data></Data>";

            XDocument docSound = XDocument.Parse(xmlSound);
            XElement dataSound = docSound.Root;
            XElement Sound = new XElement("Sound");
            dataSound.Add(Sound);
            foreach (var ii in musicsToAdd)
            {
                XElement snd = new XElement(ii.name.Replace(" ",""));
                snd.Value = ii.path;
                Sound.Add(snd);
            }

            docSound.Save(FILENAME_Sound);

            string xmlAssets = "<Data></Data>";
            XDocument docAssets = XDocument.Parse(xmlAssets);
            XElement dataAssets = docAssets.Root;
            XElement assets = new XElement("Assets");
            dataAssets.Add(assets);
            foreach (var ii in assetsToAdd)
            {
                XElement Ass = new XElement(ii.name);
                Ass.Value=(ii.path);
                assets.Add(Ass);
            }
            docAssets.Save(FILENAME_Assets);
        }
        
        protected int[,] LoadMap()
        {
            XDocument newDoc = XDocument.Load("C:\\Users\\danie\\source\\repos\\RemGame\\LevelDesignerGui\\bin\\Windows\\x86\\Debug\\levelMap.xml");
            int[][] newGrid = newDoc.Descendants("Row").Select(x => x.Elements("Column").Select(y => (int)y).ToArray()).ToArray();
            int[,] newArray = new int[newGrid.Length, newGrid[0].Length];

            for (int i = 0; i < newGrid.Length; i++)
            {
                int[] innerArray = newGrid[i];

                for (int j = 0; j < innerArray.Length; j++)

                {
                    newArray[i, j] = innerArray[j];
                }
            }
            int w = newArray.GetLength(0);
            int h = newArray.GetLength(1);

            int[,] result = new int[h, w];


            //check that Matrix read works well.Problem is at DrawGrid...
            for (int i=0;i<newArray.GetLength(0);i++)
            {
                for (int j = 0; j < newArray.GetLength(1); j++)
                    Console.Write(newArray[i, j]);
                Console.WriteLine("");
            }           
            Console.Read();
            return result;
            
        }
        protected bool IsKeyPressed(KeyboardState ks, Keys key)
        {
            return ks.IsKeyDown(key) && prevKeyState.IsKeyUp(key);

        }
    }
}
 