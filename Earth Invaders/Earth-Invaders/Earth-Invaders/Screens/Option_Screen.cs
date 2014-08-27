using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Earth_Invaders;

namespace Earth_Invaders
{
    public class Option_Screen:MenuScreen
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

        MainMenuEntry sound,help,back;

        public Option_Screen()
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

            //Initialize the entries
            sound = new MainMenuEntry(this, "Sound");
            help = new MainMenuEntry(this, "Help");
            back = new MainMenuEntry(this, "Back");

            //Set up the screen events
            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            //Set up the entry events, and load a submenu.
            sound.Selected += new EventHandler(Sound_Select);
            help.Selected += new EventHandler(Help_Select);
            back.Selected += new EventHandler(Back_Select);

            //Finally, add all entries to the list
            MenuEntries.Add(sound);
            MenuEntries.Add(help);
            MenuEntries.Add(back);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"menu");

            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            sound.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200), true);
            help.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), sound, true);
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), help, true);
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

        void Sound_Select(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new Sound_Option());
        }

        void Help_Select(object sender, EventArgs e)
        {
            ExitScreen();
            Game1.gameState = Game1.GameState.HelpScreen;
            ScreenSystem.AddScreen(new Help_Option());
        }

        void Back_Select(object sender, EventArgs e)
        {
            Game1.gameState = Game1.GameState.TitleScreen;
            MenuCancel();  
            ScreenSystem.AddScreen(new PauseScreen(null));
        }
        


        void MainMenuRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}