using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Security.Cryptography;

namespace Earth_Invaders
{
    public class GameMenuScreen : MenuScreen
    {
        string prevEntry, nextEntry, selectedEntry, cancelMenu;
        public override string PreviousEntryActionName
        {
            get { return prevEntry; }
        }

        public override string NextEntryActionName
        {
            get { return nextEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return selectedEntry; }
        }

        public override string MenuCancelActionName
        {
            get { return cancelMenu; }
        }

        MainMenuEntry NewGame, LoadGame, quit;

        public GameMenuScreen()
        {
            //Set up the action names
            prevEntry = "MenuUp";
            nextEntry = "MenuDown";
            selectedEntry = "MenuAccept";
            cancelMenu = "MenuCancel";

            //Allow transitions
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            //Customize the text colors.
            Selected = Color.Yellow;
            Highlighted = Color.Green;
            Normal = Color.White;
        }

        public override void Initialize()
        {
            //Get a reference to the input system
            InputSystem input = ScreenSystem.InputSystem;

            //Load the actions
            input.NewAction(PreviousEntryActionName, Keys.Up);
            input.NewAction(NextEntryActionName, Keys.Down);
            input.NewAction(SelectedEntryActionName, Keys.Enter);
            input.NewAction(MenuCancelActionName, Keys.Add);

            //Initialize the entries
            NewGame = new MainMenuEntry(this, "New Game");
            LoadGame = new MainMenuEntry(this, "Load Game");
            quit = new MainMenuEntry(this, "Quit Game");

            //Set up the screen events
            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            //Set up the entry events, and load a submenu.
            NewGame.Selected += new EventHandler(NewGame_Select);
            LoadGame.Selected += new EventHandler(LoadGame_Select);
            quit.Selected += new EventHandler(QuitSelect);

            //Finally, add all entries to the list
            MenuEntries.Add(NewGame);
            MenuEntries.Add(LoadGame);
            MenuEntries.Add(quit);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            if (PauseScreen._test != 0.3f)
            {
                Song BG;
                BG = content.Load<Song>(@"Sound\intro");
                MediaPlayer.Play(BG);
            }
            SpriteFont = content.Load<SpriteFont>(@"menu");
            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            NewGame.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200), true);
            LoadGame.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), NewGame, true);
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), LoadGame, true);
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
        }

        void MainMenuScreen_Entering(object sender, TransitionEventArgs tea)
        {
            //Slide effect from left to right
            float effect = (float)Math.Pow(tea.percent - 1, 2) * -100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.InitialPosition + entry.Acceleration;
            }
        }

        void MainMenuScreen_Exiting(object sender, TransitionEventArgs tea)
        {
            //Slide effect from right to left
            float effect = (float)Math.Pow(tea.percent - 1, 2) * 100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.InitialPosition - entry.Acceleration;
            }
        }

        void NewGame_Select(object sender, EventArgs e)
        {
            ExitScreen();
            //try
            //{
            //    DecryptFile("C:\\data1.txt", "C:\\data.txt");
            //}
            //catch
            //{
            //    FileStream FS1 = new FileStream("C:\\data.txt", FileMode.Append);
            //    FS1.Close();
            //    FileStream FS2 = new FileStream("C:\\data1.txt", FileMode.Append);
            //    FS2.Close();
            //}
            try
            {
                FileStream FS = new FileStream("C:\\data.txt", FileMode.Append);
                StreamWriter SW = new StreamWriter(FS);

                string input = "0@0@100@0";

                SW.WriteLine(input);
                SW.Close();
                FS.Close();
                EncryptFile("C:\\data.txt", "C:\\data1.txt");
                System.IO.File.WriteAllText(@"C:\\data.txt", string.Empty);   
            }
            catch
            {
                ;
            }
            Game1.LoadLevelNumber = 0;
            Game1.LoadLevelScore = 0;
            Game1.LoadHealth = 100;
            LevelManager.HighLevelNumber = 0;
            ScreenSystem.AddScreen(new New_Game_Screen());
            Game1.gameState = Game1.GameState.StartGame;
        }

        void LoadGame_Select(object sender, EventArgs e)
        {
            ExitScreen();

            try
            {
                DecryptFile("C:\\data1.txt", "C:\\data.txt");
                FileStream FS = new FileStream("C:\\data.txt", FileMode.Open);
                StreamReader SR = new StreamReader(FS);
                string output = SR.ReadLine();
                char[] myDelimiter = new char[] { '@' };
                string[] arr = output.Split(myDelimiter);
                Game1.LoadLevelNumber = int.Parse(arr[0]);
                if (Game1.LoadLevelNumber % 2 != 0)
                    Game1.LoadLevelNumber--;

                Game1.LoadLevelScore = int.Parse(arr[1]);

                Game1.LoadHealth = int.Parse(arr[2]);

                LevelManager.HighLevelNumber = int.Parse(arr[3]);

                FS.Close();
                System.IO.File.WriteAllText(@"C:\\data.txt", string.Empty);
            }
            catch
            {
                Game1.LoadLevelNumber = 0;
                Game1.LoadLevelScore = 0;
                Game1.LoadHealth = 100;
                LevelManager.HighLevelNumber = 0;
            }

            ScreenSystem.AddScreen(new New_Game_Screen());
            Game1.gameState = Game1.GameState.StartGame;
        }

        void QuitSelect(object sender, EventArgs e)
        {
            ExitScreen();
        }


        void MainMenuRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
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
    }
}
