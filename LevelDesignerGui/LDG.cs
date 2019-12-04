using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace LevelDesignerGui
{
    public class LDG
    {
        const string FILENAME = "levelMap.xml";
        const string LEVELASSETS = "levelAssets";   
       
        protected String AddImages()
        {
            String name_ = "";
            var ofg = new System.Windows.Forms.OpenFileDialog();
            ofg.Title = "Choose XML file (level.xml) from RemGame/LevelDesignerGui/bin/windows/x86/Debug";
            if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               name_ = Path.GetFullPath(ofg.FileName);
            }
            return name_;
        }
        //Load Game Matrix
        public int[,] LoadMapMatrix()
        {
            String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//get full path
            path = Regex.Replace(path, @"(?<=RemGame.*)RemGame", "LevelDesignerGui");//replace second occurance of RemGame to LevelDesignerGui
            XDocument newDoc = XDocument.Load(path + "\\levelMap.xml");
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
            Console.Read();   
            return newArray;
        }
  
        //Load Game Map 
        public List<Texture2D> LoadMap(GraphicsDevice graphicsDevice)
        {
            List<Texture2D> Images = new List<Texture2D>();
       
            //Read images from levelDesigner's gui folder
            String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//get full path
            path = Regex.Replace(path, @"(?<=RemGame.*)RemGame", "LevelDesignerGui");//replace second occurance of RemGame to LevelDesignerGui
            XDocument newDoc = XDocument.Load(path + "\\levelMap.xml");
       
            //get paths of map images, convert to Texture2D and add to a List
            foreach (XElement xe in newDoc.Descendants("image"))
            {         
                var name_ = xe.Value;
                FileStream fileStream = new FileStream(name_, FileMode.Open);
                Texture2D jpegForMap = Texture2D.FromStream(graphicsDevice, fileStream);
                Images.Add(jpegForMap);
                fileStream.Dispose();
            }

            Console.Read();
            return Images;
        }
        //Load Game Assets (built in assets)
        //Add to dictionary ("Assetname" : "path")
        public Dictionary<string,Texture2D> LoadAssets(GraphicsDevice graphicsDevice)
        {
            Dictionary<string, Texture2D> dataDictionary = new Dictionary<string, Texture2D>();
            String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//get full path
            path = Regex.Replace(path, @"(?<=RemGame.*)RemGame", "LevelDesignerGui");//replace second occurance of RemGame to LevelDesignerGui
            XDocument newDoc = XDocument.Load(path + "\\levelAssets.xml");

            //get paths of map images, convert to Texture2D and add to a Dictionary
            foreach (XElement xe in newDoc.Descendants().Where(p => p.HasElements==false))
            {
                int keyInt = 0;
                String keyName = xe.Name.LocalName;
                while(dataDictionary.ContainsKey(keyName))
                {
                    keyName = xe.Name.LocalName + "_" + keyInt++;
                }
                
                FileStream fileStream = new FileStream(xe.Value, FileMode.Open);
                Texture2D asset = Texture2D.FromStream(graphicsDevice, fileStream);
                fileStream.Dispose();
                dataDictionary.Add(keyName, asset);
                Console.WriteLine(keyName + " " + xe.Value);
            }    
            Console.Read();
            return dataDictionary;
        }

        //Dictionary is used only for loading several Music Files. Not needed for only one music file...
        public Dictionary<string, string> LoadMusics(GraphicsDevice graphicsDevice)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//get full path
            path = Regex.Replace(path, @"(?<=RemGame.*)RemGame", "LevelDesignerGui");//replace second occurance of RemGame to LevelDesignerGui
            XDocument newDoc;
            newDoc = XDocument.Load(path + "\\levelSound.xml");
            //get paths of game sounds, and add to a Dictionary
            foreach (XElement xe in newDoc.Descendants().Where(p => p.HasElements == false))
            {
                int keyInt = 0;
                String keyName = xe.Name.LocalName;
              
                while (dataDictionary.ContainsKey(keyName))
                {
                    keyName = xe.Name.LocalName + "_" + keyInt++;
                }
                dataDictionary.Add(keyName, xe.Value);
                Console.WriteLine(keyName + " " + xe.Value);
            }
            
            Console.Read();
            return dataDictionary;
        }

    }
}

