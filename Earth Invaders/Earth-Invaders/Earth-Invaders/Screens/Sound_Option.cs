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
namespace Earth_Invaders
{
    class Sound_Option : MenuScreen
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
        public MainMenuEntry _musicVolme, _soundEffect_Volme, back;

        public Sound_Option()
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
            _musicVolme = new MainMenuEntry(this, "Music Volume");
            _soundEffect_Volme = new MainMenuEntry(this, "Sound Effect Volume");
            back = new MainMenuEntry(this, "Back");

            //Set up the screen events
            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            //Set up the entry events, and load a submenu.
            _musicVolme.Selected += new EventHandler(Music_Volume);
            _soundEffect_Volme.Selected += new EventHandler(Sound_Effect);
            back.Selected += new EventHandler(Back);

            //Finally, add all entries to the list
            MenuEntries.Add(_musicVolme);
            MenuEntries.Add(_soundEffect_Volme);
            MenuEntries.Add(back);
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"menu");

            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            _musicVolme.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)/2)-50, 200), true);
            _soundEffect_Volme.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), _musicVolme, true);
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), _soundEffect_Volme, true);
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


        void Music_Volume(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new Music_Volume());
        }
        void Sound_Effect(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new Sound_Effect_Volume());
        }
        void Back(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new Option_Screen());
        }
        void MainMenuRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }


    }
}
