using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tile_Engine;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Earth_Invaders
{
    /// <summary>
    /// manages the levels of the games and transition between them
    /// it load levels and contains the player instance and all lists 
    /// of game objects (enemies, gemstones, health items, fires)
    /// </summary>
    public static class LevelManager
    {


        #region Declarations
        private static ContentManager Content;
        public static Player player;
        private static int currentLevel;
        private static Vector2 respawnLocation;
        private static int Arena = 0;

        public static int HighLevelNumber = 0,HighLevelNumber2=0, LVLComplement=0;
        public static string LevelName;
        private static string[] arrLevelName = { "Arena", "Egypt", "Australia", "Germany", "Hawaii", "Japan", "Sahara", "AUE", "Moon 1", "Moon 2", "NORMANDY SR1" };
        private static List<Gemstone> gemstones = new List<Gemstone>();
        private static List<Health> health = new List<Health>();
        public static List<EnemyA> enemiesA = new List<EnemyA>();
        public static List<EnemyB> enemiesB = new List<EnemyB>();
        public static List<EnemyC> enemiesC = new List<EnemyC>();
        public static List<EnemyD> enemiesD = new List<EnemyD>();
        public static List<EnemyE> enemiesE = new List<EnemyE>();
        public static List<EnemyF> enemiesF = new List<EnemyF>();
        public static List<float> maxXList = new List<float>();
        public static List<float> minYList = new List<float>();
        public static List<float> maxYList = new List<float>();
        public static List<Fire> fires = new List<Fire>();
        #endregion

        #region Properties
        public static int CurrentLevel
        {
            get { return currentLevel; }
        }

        public static Vector2 RespawnLocation
        {
            get { return respawnLocation; }
            set { respawnLocation = value; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// it runs only once and sets the player and the content
        /// </summary>
        /// <param name="content">to load images form disk into memory</param>
        /// <param name="gamePlayer">the player object</param>
        public static void Initialize(
            ContentManager content,
            Player gamePlayer)
        {
            Content = content;
            player = gamePlayer;
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// check the code of the cell of the player
        /// when the code of the cell is "DEAD" this causes the player to die
        /// </summary>
        private static void checkCurrentCellCode()
        {
            string code = TileMap.CellCodeValue(
                TileMap.GetCellByPixel(player.WorldCenter));

            if (code == "DEAD")
            {
                player.Kill();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// loads the map of the level
        /// it clears all the lists of enemies and fires and gemstones
        /// the it adds the lists' items of the new level by checking the code value of the cells
        /// it also connects to the DB to update the data of the user 
        /// and plays the music of the level and the background
        /// </summary>
        /// <param name="levelNumber">the number of the level to be loaded</param>
        public static void LoadLevel(int levelNumber)
        {
            LVLComplement = ~levelNumber;
            Loading Load = new Loading();
           // Load.Show();
            //DialogResult Loading = System.Windows.Forms.MessageBox.Show("Loading");
            
            if (levelNumber != ~LVLComplement)
                levelNumber = ~LVLComplement;
            int savelevel = levelNumber;
            if (savelevel % 2 != 0)
            {
                if (levelNumber > HighLevelNumber)
                    savelevel--;
                else
                    savelevel = HighLevelNumber;
            }
            else
            {
                if (levelNumber > HighLevelNumber)
                    HighLevelNumber = levelNumber;
                else
                    levelNumber = HighLevelNumber;
                Arena = (levelNumber / 2) + 1;
                savelevel = levelNumber;
            }
            TileMap.LoadMap((System.IO.FileStream)TitleContainer.OpenStream(
                @"Content\Maps\MAP" +
                levelNumber.ToString().PadLeft(3, '0') + ".MAP"));

            gemstones.Clear();
            health.Clear();
            enemiesA.Clear();
            enemiesB.Clear();
            enemiesC.Clear();
            enemiesD.Clear();
            enemiesE.Clear();
            enemiesF.Clear();
            maxXList.Clear();
            minYList.Clear();
            maxYList.Clear();
            fires.Clear();
            Player.Ani.Clear();
            Player.Vel.Clear();
            Player.Flip.Clear();
            Player.Worl.Clear();

            for (int x = 0; x < TileMap.MapWidth; x++)
            {
                for (int y = 0; y < TileMap.MapHeight; y++)
                {
                    if (TileMap.CellCodeValue(x, y) == "START")
                    {
                        player.WorldLocation = new Vector2(
                            x * TileMap.TileWidth,
                            y * TileMap.TileHeight);
                    }

                    if (TileMap.CellCodeValue(x, y) == "GEM")
                    {
                        gemstones.Add(new Gemstone(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "health")
                    {
                        health.Add(new Health(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_A")
                    {
                        enemiesA.Add(new EnemyA(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_B")
                    {
                        enemiesB.Add(new EnemyB(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_C")
                    {
                        enemiesC.Add(new EnemyC(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_D")
                    {
                        enemiesD.Add(new EnemyD(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_E")
                    {
                        enemiesE.Add(new EnemyE(Content, x, y));
                    }
                    if (TileMap.CellCodeValue(x, y) == "ENEMY_F")
                    {
                        enemiesF.Add(new EnemyF(Content, x, y));
                    }
                }
            }


            currentLevel = levelNumber;
            //

            Song BG;
            
            //
            
            //FileStream FS = new FileStream("C:\\data.txt", FileMode.Append);
            ////FS.Close();
            //FS = new FileStream("C:\\data1.txt", FileMode.Append);
            //FS.Close();
            try
            {
                DecryptFile("C:\\data1.txt", "C:\\data.txt");
            }
            catch
            {
                FileStream FS1 = new FileStream("C:\\data.txt", FileMode.Append);
                FS1.Close();
                FileStream FS2 = new FileStream("C:\\data1.txt", FileMode.Append);
                FS2.Close();
            }
            try
            {
                FileStream FS3 = new FileStream("C:\\data.txt", FileMode.Open);
                StreamReader SR = new StreamReader(FS3);
                string output = SR.ReadLine();
                SR.Close();
                FS3.Close();

                char[] myDelimiter = new char[] { '@' };
                string[] arr = output.Split(myDelimiter);
                if (levelNumber - int.Parse(arr[0]) > 4)
                    levelNumber = int.Parse(arr[0]);
                if (player.Score - int.Parse(arr[1]) > 4000)
                    player.Score = int.Parse(arr[1]);
                if (player._health - int.Parse(arr[2]) > 100 || player._health > 150)
                    player._health = int.Parse(arr[2]);

                HighLevelNumber2 = int.Parse(arr[3]);
                EncryptFile("C:\\data.txt", "C:\\data1.txt");
                System.IO.File.WriteAllText(@"C:\\data.txt", string.Empty);
            }
            catch
            {
                HighLevelNumber2 = 0;
            }
            ////
            
            ////

            FileStream FS = new FileStream("C:\\data.txt", FileMode.Append);
            StreamWriter SW = new StreamWriter(FS);

            string input = levelNumber.ToString();
            input += "@";

            input += player.Score.ToString();
            input += "@";

            input += (player._health.ToString());
            input += "@";

            if (HighLevelNumber2 > HighLevelNumber)
                HighLevelNumber = HighLevelNumber2;

            input += HighLevelNumber.ToString();

            SW.WriteLine(input);
            SW.Close();
            FS.Close();
            EncryptFile("C:\\data.txt", "C:\\data1.txt");
            System.IO.File.WriteAllText(@"C:\\data.txt", string.Empty);
            ////////////////////////

            if (currentLevel % 2 == 0)
                currentLevel = 0;
            else
                currentLevel = (currentLevel + 1) / 2;
            if (currentLevel == 0)
            {
                BG = Content.Load<Song>(@"Sound\Levels");
                MediaPlayer.Play(BG);
                MediaPlayer.IsRepeating = true;
            }
            else
            {
                BG = Content.Load<Song>(@"Sound\Level_" + currentLevel);
                MediaPlayer.Play(BG);
                MediaPlayer.IsRepeating = true;
            }
            if (currentLevel == 0)
                LevelName = arrLevelName[currentLevel] + " " + Arena;
            else
                LevelName = arrLevelName[currentLevel];
            for (int i = 0; i < 5; i++)
            {
                Game1.world_background[i] = Content.Load<Texture2D>(@"Textures\worlds background\" + currentLevel + "" + i);
            }
            Game1.rectangle[0] = new Rectangle(0, 0, 2048, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Game1.rectangle[1] = new Rectangle(2048, 0, 4096, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Game1.rectangle[2] = new Rectangle(4096, 0, 6144, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Game1.rectangle[3] = new Rectangle(6144, 0, 8192, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Game1.rectangle[4] = new Rectangle(8192, 0, 10240, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Game1.rectangle[0].Width = Game1.rectangle[1].Width = Game1.rectangle[2].Width = Game1.rectangle[3].Width = Game1.rectangle[4].Width = 2048;

            //
            respawnLocation = player.WorldLocation;
            //

            Load.Close();
            //
        }

        /// <summary>
        /// updates all game objects , check collision of all creatures
        /// detects collision between the player and gemstones and health items
        /// make the enemies shoot fires after a period of time
        /// and make some enemies attack the player when it becomes in a close position to the player
        /// it also removes fires when it exceeds 1000 pixel from start position
        /// </summary>
        /// <param name="gameTime">time elapsed from last call to the method</param>
        public static void Update(GameTime gameTime)
        {
            SoundEffect _gem, _hel;
            float _gameStone_Volume = Sound_Effect_Volume._stoneSound;
            float pitch = 0.0f;
            float pan = 0.0f;
            _gem = Content.Load<SoundEffect>(@"Sound\GameStone");
            _hel = Content.Load<SoundEffect>(@"Sound\Health");
            if (!Player.Dead)
            {
                checkCurrentCellCode();

                //foreach (Fire f in fires)
                //    f.Update(gameTime);
                //check if player was hit with an enemy fire
                player.CheckCollision();

                //check if player get a gemstone
                for (int x = gemstones.Count - 1; x >= 0; x--)
                {
                    gemstones[x].Update(gameTime);
                    if (player.CollisionRectangle.Intersects(
                        gemstones[x].CollisionRectangle))
                    {
                        _gem.Play(_gameStone_Volume, pitch, pan);
                        gemstones.RemoveAt(x);
                        player.Score += 20;
                    }
                }
                //check if player get a health
                for (int x = health.Count - 1; x >= 0; x--)
                {
                    health[x].Update(gameTime);
                    if (player.CollisionRectangle.Intersects(
                        health[x].CollisionRectangle))
                    {
                        _hel.Play(_gameStone_Volume, pitch, pan);
                        health.RemoveAt(x);
                        player._health += 10;
                    }
                }


                //A
                for (int x = enemiesA.Count - 1; x >= 0; x--)
                {

                    enemiesA[x].Update(gameTime);

                    if ((!enemiesA[x].playerSeen) && (Vector2.Distance(player.WorldLocation, enemiesA[x].WorldLocation) <= 500)
                        && Math.Abs(player.WorldLocation.Y - enemiesA[x].WorldLocation.Y) <= 80)
                        enemiesA[x].playerSeen = true;

                    if (enemiesA[x].playerSeen)
                        enemiesA[x].Attack(player.WorldLocation);

                    if (!enemiesA[x].Dead)
                    {
                        enemiesA[x].CheckCollision();
                        enemiesA[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (enemiesA[x].ElapsedTime / 2000.0 >= 1.0 && Vector2.Distance(enemiesA[x].WorldLocation, player.WorldLocation) <= 500)
                        {
                            enemiesA[x].ElapsedTime = 0;
                            //length is the absolute value of the vector between the enemy and the player 
                            float length = Vector2.Distance(enemiesA[x].WorldCenter, player.WorldCenter);
                            Vector2 fireDirection, firePos = enemiesA[x].WorldCenter;
                            //firePos.Y += 10;
                            //firePos.X -= 24;
                            fireDirection = new Vector2((player.WorldCenter.X - enemiesA[x].WorldCenter.X) / length, (player.WorldCenter.Y - enemiesA[x].WorldCenter.Y) / length);
                            fires.Add(new Fire(Content, firePos, fireDirection, "ENEMY_A"));
                        }
                    }
                    else
                    {
                        if (!enemiesA[x].Enabled)
                        {
                            enemiesA.RemoveAt(x);
                            player.Score += 20;
                        }
                    }



                }

                //B
                for (int x = enemiesB.Count - 1; x >= 0; x--)
                {
                    enemiesB[x].Update(gameTime);
                    if (!enemiesB[x].Dead)
                    {
                        enemiesB[x].CheckCollision();
                        if (!enemiesB[x].Dead)
                        {
                            enemiesB[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (enemiesB[x].ElapsedTime / 5000.0 >= 1.0)
                            {
                                enemiesB[x].ElapsedTime = 0;
                                Vector2 fireLocation, firePos = enemiesB[x].WorldCenter;
                                firePos.Y -= 22;
                                if (enemiesB[x].Flipped)
                                {
                                    firePos.X += 24;
                                    fireLocation = new Vector2(1, 0);
                                }
                                else
                                {
                                    firePos.X -= 24;
                                    fireLocation = new Vector2(-1, 0);
                                }

                                fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_B"));
                            }
                        }
                    }
                    else
                    {
                        if (!enemiesB[x].Enabled)
                        {
                            enemiesB.RemoveAt(x);
                            player.Score += 10;
                        }
                    }
                }
                //C
                for (int x = enemiesC.Count - 1; x >= 0; x--)
                {
                    enemiesC[x].Update(gameTime);
                    if (!enemiesC[x].Dead)
                    {
                        enemiesC[x].CheckCollision();
                        if (!enemiesC[x].Dead)
                        {
                            enemiesC[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (enemiesC[x].ElapsedTime / 3000.0 >= 1.0)
                            {
                                enemiesC[x].ElapsedTime = 0;
                                Vector2 fireLocation;
                                if (enemiesC[x].Flipped)
                                {
                                    fireLocation = new Vector2(1, 0);
                                }
                                else
                                {
                                    fireLocation = new Vector2(-1, 0);
                                }
                                float length = Vector2.Distance(enemiesC[x].WorldCenter, player.WorldCenter);
                                Vector2 fireDirection, firePos = enemiesC[x].WorldCenter;
                                // firePos.Y += 10;
                                // firePos.X -= 24;
                                fireDirection = new Vector2((player.WorldCenter.X - enemiesC[x].WorldCenter.X) / length, (player.WorldCenter.Y - enemiesC[x].WorldCenter.Y) / length);
                                fires.Add(new Fire(Content, firePos, fireDirection, "ENEMY_C"));
                            }
                        }
                    }
                    else
                    {
                        if (!enemiesC[x].Enabled)
                        {
                            enemiesC.RemoveAt(x);
                            player.Score += 10;
                        }
                    }
                }
                //D
                for (int x = enemiesD.Count - 1; x >= 0; x--)
                {
                    enemiesD[x].Update(gameTime);
                    if (!enemiesD[x].playerSeen && player.WorldLocation.X >= enemiesD[x].WorldLocation.X)
                        enemiesD[x].playerSeen = true;
                    if (enemiesD[x].playerSeen)
                        enemiesD[x].Attack(player.WorldCenter);
                    else
                    {
                        enemiesD[x].WalkSpeed = enemiesD[x].direction.X = enemiesD[x].direction.Y = 0;
                    }
                    if (!enemiesD[x].Dead)
                    {
                        enemiesD[x].CheckCollision();
                        if (!enemiesD[x].Dead)
                        {
                            enemiesD[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (enemiesD[x].ElapsedTime / 1000.0 >= 1.0)
                            {
                                enemiesD[x].ElapsedTime = 0;
                                Vector2 fireLocation;
                                if (enemiesD[x].Flipped)
                                {
                                    fireLocation = new Vector2(1, 0);
                                }
                                else
                                {
                                    fireLocation = new Vector2(-1, 0);
                                }
                                float length = Vector2.Distance(enemiesD[x].WorldCenter, player.WorldCenter);
                                Vector2 fireDirection, firePos = enemiesD[x].WorldCenter;
                                firePos.Y += 10;
                                firePos.X -= 24;
                                fireDirection = new Vector2((player.WorldCenter.X - enemiesD[x].WorldCenter.X) / length, (player.WorldCenter.Y - enemiesD[x].WorldCenter.Y) / length);
                                fires.Add(new Fire(Content, firePos, fireDirection, "ENEMY_D"));
                                fireLocation = new Vector2(1, 1);
                                fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_D"));
                                fireLocation = new Vector2(-1, 1);
                                fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_D"));
                            }
                        }
                    }
                    else
                    {
                        if (!enemiesD[x].Enabled)
                        {
                            enemiesD.RemoveAt(x);
                            player.Score += 50;
                        }
                    }
                }
                //E
                for (int x = enemiesE.Count - 1; x >= 0; x--)
                {
                    enemiesE[x].Update(gameTime);
                    if (!enemiesE[x].Dead)
                    {
                        enemiesE[x].CheckCollision();
                        if (!enemiesE[x].Dead)
                        {
                            enemiesE[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (enemiesE[x].ElapsedTime / 1000.0 >= 1.0)
                            {
                                enemiesE[x].ElapsedTime = 0;
                                Vector2 fireLocation, firePos = enemiesE[x].WorldCenter;
                                firePos.Y -= 30;
                                // firePos.X -= 24;
                                if (enemiesE[x].Flipped)
                                {
                                    firePos.X += 24;
                                    fireLocation = new Vector2(1, 0);
                                }
                                else
                                {
                                    firePos.X -= 24;
                                    fireLocation = new Vector2(-1, 0);
                                }

                                fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_E"));
                            }
                        }
                    }
                    else
                    {
                        if (!enemiesE[x].Enabled)
                        {
                            enemiesE.RemoveAt(x);
                            player.Score += 10;
                        }
                    }
                }
                //F
                for (int x = enemiesF.Count - 1; x >= 0; x--)
                {
                    enemiesF[x].Update(gameTime);
                    if (!enemiesF[x].Dead)
                    {
                        enemiesF[x].CheckCollision();
                        if (!enemiesF[x].Dead)
                        {
                            enemiesF[x].ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (enemiesF[x].ElapsedTime / 1000.0 >= 1.0)
                            {
                                enemiesF[x].ElapsedTime = 0;
                                Vector2 fireLocation, firePos = enemiesF[x].WorldCenter;
                                firePos.Y -= 10;
                                if (enemiesF[x].Flipped)
                                {
                                    firePos.X += 10;
                                    fireLocation = new Vector2(1, 0);
                                    fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_F2"));
                                }
                                else
                                {
                                    firePos.X -= 40;
                                    fireLocation = new Vector2(-1, 0);
                                    fires.Add(new Fire(Content, firePos, fireLocation, "ENEMY_F1"));
                                }


                            }
                        }
                    }
                    else
                    {
                        if (!enemiesF[x].Enabled)
                        {
                            enemiesF.RemoveAt(x);
                            player.Score += 10;
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////
                for (int i = fires.Count - 1; i >= 0; i--)
                {
                    if (Vector2.Distance(fires[i]._startPosition, fires[i].WorldLocation) > (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)/2 || !fires[i].Enabled)
                    {
                        fires.RemoveAt(i);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////
                foreach (Fire f in fires)
                {
                    f.Update(gameTime);
                }

            }
        }

        /// <summary>
        /// draw all game objects (gemstones, healthItems, enemies, fires)
        /// </summary>
        /// <param name="spriteBatch">the spriteSheet which is used in drawing game objects</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Gemstone gem in gemstones)
                gem.Draw(spriteBatch);
            foreach (Health hel in health)
                hel.Draw(spriteBatch);
            foreach (EnemyA enemy in enemiesA)
                enemy.Draw(spriteBatch);
            foreach (EnemyB enemy in enemiesB)
                enemy.Draw(spriteBatch);
            foreach (EnemyC enemy in enemiesC)
                enemy.Draw(spriteBatch);
            foreach (EnemyD enemy in enemiesD)
                enemy.Draw(spriteBatch);
            foreach (EnemyE enemy in enemiesE)
                enemy.Draw(spriteBatch);
            foreach (EnemyF enemy in enemiesF)
                enemy.Draw(spriteBatch);
            foreach (Fire f in fires)
                f.Draw(spriteBatch);

        }

        public static void EncryptFile(string inputFile, string outputFile)
        {

            try
            {
                string password = @"065_Gm%$"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                
            }
        }
        ///<summary>
        /// Steve Lydford - 12/05/2008.
        ///
        /// Decrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        public static void DecryptFile(string inputFile, string outputFile)
        {

            {
                string password = @"065_Gm%$"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }
        #endregion

    }

}
